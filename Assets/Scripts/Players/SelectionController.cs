using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    [SerializeField]
    List<Transform> selected = new List<Transform>();

    [SerializeField]
    private Color borderColor = Color.green;
    [SerializeField]
    private Color centerColor = new Color(0.8f, 0.8f, 0.95f, 0.1f);

    [SerializeField]
    private bool isDragging = false;
    private Vector3 startingDragPosition;

    private void OnGUI()
    {
        if (isDragging && GameManager.Instance.CursorState == CursorState.Selecting)
        {
            var rect = ScreenHelper.GetScreenRect(startingDragPosition, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, centerColor);
            ScreenHelper.DrawScreenRectBorder(rect, 1, borderColor);
        }
    }

    private void Start()
    {
        InputManager.Instance.Cancel.AddListener(CancelDrag);
        InputManager.Instance.OrderMove.AddListener(OrderSelectedUnitsToMove);
        InputManager.Instance.OrderStop.AddListener(OrderSelectedUnitsToStop);
        InputManager.Instance.OrderAttack.AddListener(OrderSelectedUnitsToAttack);
    }

    void Update()
    {

        CheckBeginDrag();

        CheckEndDrag();
    }


    void CheckBeginDrag()
    {
        if (Input.GetButtonDown("Primary Mouse") && GameManager.Instance.CursorState == CursorState.None)
        {
            bool isMultiSelecting = Input.GetButton("MultiSelect");
            startingDragPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out ISelectable selectable) && hit.collider.TryGetComponent(out Unit unit) && unit.networkObject.IsOwner)
                {
                    if (isMultiSelecting && selectable.IsSelected)
                    {
                        Deselect(hit.transform);
                    }
                    else
                    {
                        SelectUnit(hit.transform, isMultiSelecting);
                    }
                }
                else
                {
                    isDragging = true;
                    GameManager.Instance.CursorState = CursorState.Selecting;
                }
            }
        }
    }

    void CheckEndDrag()
    {
        if (Input.GetButtonUp("Primary Mouse"))
        {
            if (isDragging)
            {
                if (!Input.GetButton("MultiSelect"))
                    DeselectAll();

                foreach (var selectableObject in GameManager.Instance.ControllingPlayer.AllControlledObjects)
                {
                    if (selectableObject.TryGetComponent<ISelectable>(out _) && IsWithinSelectionBounds(selectableObject.transform))
                    {
                        SelectUnit(selectableObject.transform, true);
                    }
                }

                isDragging = false;
                GameManager.Instance.CursorState = CursorState.None;
            }
        }
    }

    void CancelDrag()
    {
        isDragging = false;
        GameManager.Instance.CursorState = CursorState.None;
    }

    void SelectUnit(Transform unit, bool isMultiSelect = false)
    {
        if (!isMultiSelect)
        {
            DeselectAll();
        }

        unit.GetComponent<ISelectable>().Select();
        selected.Add(unit);
    }

    void DeselectAll()
    {
        for (int i = 0; i < selected.Count; i++)
        {
            Deselect(selected[i]);
            i--;
        }
        selected.Clear();
    }

    void Deselect(Transform selectable)
    {
        selectable.GetComponent<ISelectable>().Deselect();
        selected.Remove(selectable);
    }

    bool IsWithinSelectionBounds(Transform transform)
    {
        if (!isDragging)
            return false;

        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera, startingDragPosition, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }

    void OrderSelectedUnitsToMove()
    {
        if (selected.Count == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Vector3 targetPosition = hit.point;
            List<Vector3> points = FormationHelper.GetFormation(targetPosition, selected.Where(x => x.TryGetComponent<Unit>(out _)).Count());

            for (int i = 0; i < selected.Count; i++)
            {
                var current = selected[i];

                if (current.TryGetComponent<Unit>(out Unit unit))
                {
                    unit.SendRpcMoveToPosition(points[i]);
                }
            }
        }
    }

    void OrderSelectedUnitsToStop()
    {
        if (selected.Count == 0)
            return;

        foreach (var item in selected.Where(x => x.TryGetComponent<Unit>(out _)))
        {
            item.GetComponent<Unit>().SendRpcOrderStop();
        }
    }

    void OrderSelectedUnitsToAttack()
    {
        if (selected.Count == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            foreach (var item in selected.Where(x => x.TryGetComponent<Unit>(out _)))
            {
                item.GetComponent<Unit>().MoveIntoAttackRange(hit.point);
                _ = item.GetComponent<Unit>().networkObject;
            }
        }
    }
}

using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager Instance;

    public List<Transform> Selected = new List<Transform>();

    public UnityEvent SelectionChanged;

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

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);

        SelectionChanged = new UnityEvent();
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
        if (Input.GetButtonDown("Primary Mouse") && GameManager.Instance.CursorState == CursorState.None && !EventSystem.current.IsPointerOverGameObject())
        {
            bool isMultiSelecting = Input.GetButton("Shift");
            startingDragPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent(out ISelectable selectable) && hit.collider.TryGetComponent(out Unit unit) && unit.networkObject.IsOwner)
                {
                    if (isMultiSelecting && selectable.IsSelected)
                    {
                        Deselect(hit.transform);
                        SelectionChanged.Invoke();
                    }
                    else
                    {
                        SelectUnit(hit.transform, isMultiSelecting);
                        SelectionChanged.Invoke();
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
                if (!Input.GetButton("Shift"))
                    DeselectAll();

                foreach (var selectableObject in GameManager.Instance.ControllingPlayer.AllControlledObjects)
                {
                    if (selectableObject.TryGetComponent<ISelectable>(out _) && IsWithinSelectionBounds(selectableObject.transform))
                    {
                        SelectUnit(selectableObject.transform, true);
                    }
                }
                SelectionChanged.Invoke();
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
        Selected.Add(unit);
    }

    void DeselectAll()
    {
        for (int i = 0; i < Selected.Count; i++)
        {
            Deselect(Selected[i]);
            i--;
        }
        Selected.Clear();

        SelectionChanged.Invoke();
    }

    void Deselect(Transform selectable)
    {
        if (selectable != null)
            selectable.GetComponent<ISelectable>().Deselect();

        Selected.Remove(selectable);
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
        if (Selected.Count == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            Vector3 targetPosition = hit.point;
            List<Vector3> points = FormationHelper.GetFormation(targetPosition, Selected.Where(x => x != null && x.TryGetComponent<Unit>(out _)).Count());

            for (int i = 0; i < Selected.Count; i++)
            {
                var current = Selected[i];

                if (current.TryGetComponent<Unit>(out Unit unit))
                {
                    unit.SendRpcMoveToPosition(points[i]);
                }
            }
        }
    }

    void OrderSelectedUnitsToStop()
    {
        if (Selected.Count == 0)
            return;

        foreach (var item in Selected.Where(x => x.TryGetComponent<Unit>(out _)))
        {
            item.GetComponent<Unit>().SendRpcOrderStop();
        }
    }

    void OrderSelectedUnitsToAttack()
    {
        if (Selected.Count == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            foreach (var item in Selected.Where(x => x.TryGetComponent<Unit>(out _)))
            {
                item.GetComponent<Unit>().MoveIntoAttackRange(hit.point);
            }
        }
    }
}

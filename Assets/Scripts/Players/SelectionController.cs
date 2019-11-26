﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionController : MonoBehaviour
{
    [SerializeField, ReadOnly]
    List<Transform> selected = new List<Transform>();

    [SerializeField]
    private Color borderColor = Color.green;
    [SerializeField]
    private Color centerColor = new Color(0.8f, 0.8f, 0.95f, 0.1f);

    [SerializeField, ReadOnly]
    private bool isDragging = false;
    private Vector3 mousePositon;

    private void OnGUI()
    {
        if (isDragging)
        {
            var rect = ScreenHelper.GetScreenRect(mousePositon, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, centerColor);
            ScreenHelper.DrawScreenRectBorder(rect, 1, borderColor);
        }
    }

    void Update()
    {

        if (Input.GetButtonDown("Primary Mouse"))
        {
            mousePositon = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.TryGetComponent<ISelectable>(out ISelectable selectable))
                {
                    if (selectable.IsSelected)
                    {
                        Deselect(hit.transform);
                    }
                    else
                    {
                        SelectUnit(hit.transform, Input.GetButton("MultiSelect"));
                    }
                }
                else
                {
                    isDragging = true;
                }
            }

        }

        if (Input.GetButtonUp("Primary Mouse"))
        {
            if (isDragging)
            {
                DeselectAll();
                foreach (var selectableObject in CameraController.Instance.Player.Units)
                {
                    if (IsWithinSelectionBounds(selectableObject.transform))
                    {
                        SelectUnit(selectableObject.transform, true);
                    }
                }

                isDragging = false;
            }

        }

    }

    private void SelectUnit(Transform unit, bool isMultiSelect = false)
    {
        if (!isMultiSelect)
        {
            DeselectAll();
        }

        unit.GetComponent<ISelectable>().Select();
        selected.Add(unit);
    }

    private void DeselectAll()
    {
        for (int i = 0; i < selected.Count; i++)
        {
            Deselect(selected[i]);
        }
        selected.Clear();
    }

    private void Deselect(Transform selectable)
    {
        selectable.GetComponent<ISelectable>().Deselect();
    }

    private bool IsWithinSelectionBounds(Transform transform)
    {
        if (!isDragging)
            return false;

        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera, mousePositon, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }
}

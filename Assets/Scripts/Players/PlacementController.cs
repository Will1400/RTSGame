﻿using UnityEngine;
using System.Collections;

public class PlacementController : MonoBehaviour
{
    private GameObject currentObject;
    private PlacementValidator currentPlacementValidator;

    bool isColliderTrigger;

    void Update()
    {
        if (GameManager.Instance.CursorState == CursorState.None)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currentObject = Instantiate(BuildingManager.Instance.GetBuilding("Building"), GameManager.Instance.ControllingPlayer.BuildingHolder);
                GameManager.Instance.ControllingPlayer.Buildings.Add(currentObject);
                GameManager.Instance.CursorState = CursorState.Building;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currentObject = Instantiate(UnitManager.Instance.GetUnit("Swordmen"), GameManager.Instance.ControllingPlayer.UnitHolder);
                currentObject.GetComponent<Unit>().Owner = GameManager.Instance.ControllingPlayer;
                GameManager.Instance.ControllingPlayer.Units.Add(currentObject);
                GameManager.Instance.CursorState = CursorState.Building;
            }

            if (currentObject == null)
                GameManager.Instance.CursorState = CursorState.None;
            else
            {
                currentPlacementValidator = currentObject.AddComponent<PlacementValidator>();
                currentObject.AddComponent<Rigidbody>().isKinematic = true;

                var rb = currentObject.GetComponent<Collider>();
                isColliderTrigger = rb.isTrigger;
                rb.isTrigger = true;
            }
        }

        if (GameManager.Instance.CursorState == CursorState.Building)
        {
            if (Input.GetMouseButtonDown(1))
                CancelBuild();

            MoveBuildingToMouse();

            if (Input.GetMouseButtonDown(0) && currentPlacementValidator.IsValidPosition())
                PlaceCurrentObject();
        }
    }

    void MoveBuildingToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            currentObject.transform.position = hit.point + new Vector3(0, currentObject.transform.localScale.y / 2);
            currentObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
        }
    }

    void PlaceCurrentObject()
    {
        Destroy(currentPlacementValidator);
        Destroy(currentObject.GetComponent<Rigidbody>());

        currentObject.GetComponent<Collider>().isTrigger = false;

        if (!isColliderTrigger)
            currentObject.GetComponent<Collider>().isTrigger = false;

        currentObject = null;
        GameManager.Instance.CursorState = CursorState.None;
    }

    void CancelBuild()
    {
        Destroy(currentObject);
        GameManager.Instance.CursorState = CursorState.None;
    }
}

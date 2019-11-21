using UnityEngine;
using System.Collections;

public class PlacementManager : MonoBehaviour
{
    private GameObject currentObject;
    private PlacingBuilding currentPlacingBuilding;

    bool isColliderTrigger;

    void Update()
    {
        if (CameraController.Instance.CursorState == CursorState.None && Input.GetKeyDown(KeyCode.Alpha1))
        {
            CameraController.Instance.CursorState = CursorState.Building;
            currentObject = Instantiate(BuildingManager.Instance.GetBuilding("TestBuilding"));

            if (currentObject == null)
                CameraController.Instance.CursorState = CursorState.None;
            else
            {
                currentPlacingBuilding = currentObject.AddComponent<PlacingBuilding>();
                currentObject.AddComponent<Rigidbody>().isKinematic = true;

                var rb = currentObject.GetComponent<Collider>();
                isColliderTrigger = rb.isTrigger;
                rb.isTrigger = true;
            }
        }

        if (CameraController.Instance.CursorState == CursorState.Building)
        {
            if (Input.GetMouseButtonDown(1))
                CancelBuild();

            MoveBuildingToMouse();

            if (Input.GetMouseButtonDown(0) && currentPlacingBuilding.Colliders.Count == 0)
                PlaceBuilding();
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

    void PlaceBuilding()
    {
        Destroy(currentPlacingBuilding);
        Destroy(currentObject.GetComponent<Rigidbody>());

        currentObject.GetComponent<Collider>().isTrigger = false;

        if (!isColliderTrigger)
            currentObject.GetComponent<Collider>().isTrigger = false;

        currentObject = null;
        CameraController.Instance.CursorState = CursorState.None;
    }

    void CancelBuild()
    {
        Destroy(currentObject);
        CameraController.Instance.CursorState = CursorState.None;
    }
}

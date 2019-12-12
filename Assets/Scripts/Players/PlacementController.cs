using UnityEngine;
using System.Collections;

public class PlacementController : MonoBehaviour
{
    private GameObject currentObject;
    private PlacementValidator currentPlacementValidator;

    bool isColliderTrigger;

    private void Start()
    {
        InputManager.Instance.Cancel.AddListener(CancelBuild);
    }

    void Update()
    {
        if (GameManager.Instance.CursorState == CursorState.None)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SpawnBuilding("Building");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SpawnUnit("Swordmen");
            }
        }

        if (GameManager.Instance.CursorState == CursorState.Building)
        {
            if (Input.GetButton("Escape") || Input.GetButton("Secondary Mouse"))
                CancelBuild();

            MoveBuildingToMouse();

            if (Input.GetMouseButtonDown(0) && currentPlacementValidator.IsValidPosition())
                PlaceCurrentObject();
        }
    }

    public void SpawnUnit(string unitName)
    {
        currentObject = Instantiate(UnitManager.Instance.GetUnit(unitName), GameManager.Instance.ControllingPlayer.UnitHolder);
        currentObject.GetComponent<Unit>().Owner = GameManager.Instance.ControllingPlayer;
        GameManager.Instance.ControllingPlayer.Units.Add(currentObject);
        GameManager.Instance.CursorState = CursorState.Building;
        AddValidation();
    }

    public void SpawnBuilding(string buildingName)
    {
        currentObject = Instantiate(BuildingManager.Instance.GetBuilding(buildingName), GameManager.Instance.ControllingPlayer.BuildingHolder);
        GameManager.Instance.ControllingPlayer.Buildings.Add(currentObject);
        GameManager.Instance.CursorState = CursorState.Building;
        AddValidation();
    }

    void AddValidation()
    {
        if (currentObject == null)
            GameManager.Instance.CursorState = CursorState.None;
        else
            currentPlacementValidator = currentObject.AddComponent<PlacementValidator>();
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

        currentObject = null;
        currentPlacementValidator = null;
        GameManager.Instance.CursorState = CursorState.None;
    }

    void CancelBuild()
    {
        if (currentObject == null)
            return;

        Destroy(currentObject);
        currentObject = null;
        currentPlacementValidator = null;
        GameManager.Instance.CursorState = CursorState.None;
    }
}

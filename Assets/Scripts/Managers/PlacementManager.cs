using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine.EventSystems;

public class PlacementManager : MonoBehaviour
{
    public static PlacementManager Instance;

    [SerializeField]
    private GameObject currentObject;
    [SerializeField]
    private PlacementValidator currentPlacementValidator;

    bool isColliderTrigger;

    ObjectType objectType;
    int objectIndex;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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

            if (Input.GetMouseButtonDown(0) && currentPlacementValidator.IsValidPosition() && !EventSystem.current.IsPointerOverGameObject())
                PlaceCurrentObject();
        }
    }

    public void SpawnUnit(string unitName)
    {
        CancelBuild();
        currentObject = Instantiate(UnitManager.Instance.GetUnit(unitName), GameManager.Instance.ControllingPlayer.UnitHolder);
        GameManager.Instance.CursorState = CursorState.Building;
        currentObject.GetComponent<Unit>().enabled = false;
        objectType = ObjectType.Unit;
        objectIndex = UnitManager.Instance.GetIndexOfUnitName(unitName) - 1;
        AddValidation();
    }

    public void SpawnBuilding(string buildingName)
    {
        CancelBuild();
        currentObject = Instantiate(BuildingManager.Instance.GetBuilding(buildingName), GameManager.Instance.ControllingPlayer.BuildingHolder);
        GameManager.Instance.ControllingPlayer.Buildings.Add(currentObject);
        GameManager.Instance.CursorState = CursorState.Building;
        objectType = ObjectType.Building;
        objectIndex = BuildingManager.Instance.GetIndexOfBuildingName(buildingName);
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

        if (objectType == ObjectType.Unit)
        {
           NetworkManager.Instance.InstantiateUnit(objectIndex, currentObject.transform.position).networkStarted += (behavior) =>
            {
                UnitBehavior unit = behavior as UnitBehavior;
                unit.networkObject.SendRpc(2 + 5, Receivers.AllBuffered, GameManager.Instance.ControllingPlayer.PlayerNetworkId);
                PlayerUiManager.Instance.UpdateLocalPlayerInfo();
            };
        }

        Destroy(currentObject);
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

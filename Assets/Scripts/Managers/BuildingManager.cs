using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    private List<GameObject> buildingPrefabs;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetBuilding(string buildingName)
    {
        return buildingPrefabs.First(x => x.name == buildingName);
    }

    public GameObject GetBuilding(int buildingIndex)
    {
        return buildingPrefabs[buildingIndex];
    }

    public int GetIndexOfBuildingName(string buildingName)
    {
        return buildingPrefabs.Select(x => x.name).ToList().IndexOf(buildingName);
    }
}

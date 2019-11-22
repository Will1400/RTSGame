using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerName;
    public Team Team;

    public Transform BuildingHolder;
    public Transform UnitHolder;

    private List<GameObject> Buildings;
    private List<GameObject> Units;

    private void Start()
    {
        if (Team == null)
        {
            Team = transform.parent.GetComponent<Team>();
        }

        if (GameObject.Find("Buildings"))
            BuildingHolder = GameObject.Find("Buildings").transform;
        else
            BuildingHolder = new GameObject("Buildings").transform;

        if (GameObject.Find("Units"))
            UnitHolder = GameObject.Find("Units").transform;
        else
            UnitHolder = new GameObject("Units").transform;
    }
}

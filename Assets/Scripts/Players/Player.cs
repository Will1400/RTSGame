﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerName;
    public Team Team;

    public Transform BuildingHolder;
    public Transform UnitHolder;

    public List<GameObject> Buildings;
    public List<GameObject> Units;

    private GameObject HQ;

    private void Awake()
    {
        if (Team == null)
        {
            Team = transform.parent.GetComponent<Team>();
        }
        Buildings = Buildings ?? new List<GameObject>();
        Units = Units ?? new List<GameObject>();
    }

    public void Initialize(GameObject HQ)
    {
        this.HQ = HQ;

        if (GameObject.Find("Buildings"))
            BuildingHolder = GameObject.Find("Buildings").transform;
        else
            BuildingHolder = new GameObject("Buildings").transform;

        if (GameObject.Find("Units"))
            UnitHolder = GameObject.Find("Units").transform;
        else
            UnitHolder = new GameObject("Units").transform;
    }

    public static bool IsOnSameTeam(IControlledByPlayer first, IControlledByPlayer second)
    {
        return first.Owner.Team == second.Owner.Team;
    }
}

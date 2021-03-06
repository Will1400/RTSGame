﻿using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public string PlayerName;
    public uint PlayerNetworkId;
    public Team Team;
    public int Score;
    public Color Color;

    public Transform BuildingHolder;
    public Transform UnitHolder;

    public List<GameObject> Buildings;
    public List<GameObject> Units;

    public UnityEvent OnPlayerDeath;

    public List<GameObject> AllControlledObjects
    {
        get
        {
            return new List<GameObject>().Concat(Units).Concat(Buildings).ToList();
        }
    }

    private GameObject HQ;

    private void Awake()
    {
        if (Team == null && transform.parent != null && transform.parent.TryGetComponent(out Team team))
            Team = team;

        OnPlayerDeath = new UnityEvent();

        Buildings = Buildings ?? new List<GameObject>();
        Units = Units ?? new List<GameObject>();
    }

    public void Initialize(Vector3 spawnpoint)
    {
        if (transform.Find("Buildings") != null)
            BuildingHolder = transform.Find("Buildings").transform;
        else
            BuildingHolder = new GameObject("Buildings").transform;

        if (transform.Find("Units") != null)
            UnitHolder = transform.Find("Units").transform;
        else
            UnitHolder = new GameObject("Units").transform;

        BuildingHolder.SetParent(transform);
        UnitHolder.SetParent(transform);

        HQ = Instantiate(BuildingManager.Instance.GetBuilding("HQ"), spawnpoint, Quaternion.identity, BuildingHolder);
        var hqComponent = HQ.GetComponent<HQ>();
        hqComponent.OnDeath.AddListener(PlayerDied);
        hqComponent.Owner = this;
    }

    public static bool IsOnSameTeam(IControlledByPlayer first, IControlledByPlayer second)
    {
        if (first.Owner != null && first.Owner.Team != null && second.Owner != null && second.Owner.Team != null)
        {
            return first.Owner.Team == second.Owner.Team;
        }

        return true;
    }

    void PlayerDied()
    {
        var hqComponent = HQ.GetComponent<HQ>();

        hqComponent.OnDeath.RemoveListener(PlayerDied);
    }

    private void OnDestroy()
    {
        HQ.GetComponent<HQ>().OnDeath.RemoveListener(PlayerDied);
    }
}

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour
{

    public static UnitManager Instance;

    [SerializeField]
    private List<GameObject> unitPrefabs;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject GetUnit(string unitName)
    {
        return unitPrefabs.First(x => x.name == unitName);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public string TeamName;
    public List<Player> Players = new List<Player>();

    private Transform TeamHolder;


    private void Start()
    {
        TeamHolder = transform;
        foreach (Transform item in transform)
        {
            if (item.TryGetComponent<Player>(out Player player))
            {
                Players.Add(player);
            }
        }
    }
}

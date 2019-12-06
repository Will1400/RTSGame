using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public string TeamName;
    public List<Player> Players;

    private Transform TeamHolder;


    private void Start()
    {
        TeamHolder = transform;
        Players = new List<Player>();
        foreach (Transform item in transform)
        {
            if (item.TryGetComponent<Player>(out Player player))
            {
                Players.Add(player);
            }
        }
    }
}

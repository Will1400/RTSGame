using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private List<Transform> spawnPoints;

    private void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        int count = 0;
        foreach (Team team in TeamManager.Instance.Teams)
        {
            foreach (Player player in team.Players)
            {
                player.Initialize(Instantiate(BuildingManager.Instance.GetBuilding("HQ"), spawnPoints[count].position, Quaternion.identity , player.transform));
                count++;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CursorState CursorState;

    public Player ControllingPlayer;

    [SerializeField]
    private List<Transform> spawnPoints;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }

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

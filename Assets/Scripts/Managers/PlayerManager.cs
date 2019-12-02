using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public List<NetworkingPlayer> NetworkingPlayers = new List<NetworkingPlayer>();
    public List<Player> Players = new List<Player>();


    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        NetworkManager.Instance.MasterServerNetworker.playerAccepted += PlayerJoined;
        NetworkManager.Instance.MasterServerNetworker.playerDisconnected += (player, sender) =>
        {
            NetworkingPlayers.Remove(player);
        };
    }

    private void PlayerJoined(NetworkingPlayer player, NetWorker sender)
    {
        NetworkingPlayers.Add(player);
        //sender.
    }


}

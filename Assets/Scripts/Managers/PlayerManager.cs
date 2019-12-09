﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using System.Linq;
using BeardedManStudios.Forge.Logging;
using UnityEngine.Events;

public class PlayerManager : PlayerManagerBehavior
{
    public static PlayerManager Instance;

    public List<NetworkingPlayer> NetworkingPlayers = new List<NetworkingPlayer>();
    public List<Player> Players = new List<Player>();
    public List<Team> Teams = new List<Team>();

    public UnityEvent PlayersSetup;


    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);

        PlayersSetup = new UnityEvent();
    }

    public void SetupPlayersFromLobby(List<LobbyPlayer> lobbyPlayers)
    {
        if (!networkObject.IsServer)
            return;

        networkObject.Networker.playerConnected += (player, sender) =>
        {
            networkObject.SendRpc(RPC_CREATE_TEAM, Receivers.AllBuffered, 9999);
            networkObject.SendRpc(RPC_CREATE_PLAYER, Receivers.AllBuffered, player.Name, player.NetworkId);
            networkObject.SendRpc(RPC_ASSIGN_PLAYER_TO_TEAM, Receivers.AllBuffered, player.NetworkId, 9999);
            networkObject.SendRpc(player, RPC_SETUP_LOCAL_PLAYER);
        };

        List<int> teamIds = lobbyPlayers.Select(x => x.TeamID).Distinct().ToList();

        foreach (var item in teamIds)
        {
            Debug.Log("Sending rpc to create team: " + item);
            networkObject.SendRpc(RPC_CREATE_TEAM, Receivers.AllBuffered, item.ToString());
        }

        foreach (var item in lobbyPlayers)
        {
            Debug.Log("Sending rpc to create player: " + item.Name);
            networkObject.SendRpc(RPC_CREATE_PLAYER, Receivers.AllBuffered, item.Name, item.NetworkId);
            Debug.Log("Sending rpc to assign player to team. " + item.Name + " Team: " + item.TeamID);
            networkObject.SendRpc(RPC_ASSIGN_PLAYER_TO_TEAM, Receivers.AllBuffered, item.NetworkId, item.TeamID);
        }

        NetworkManager.Instance.InstantiatePlayerUiManager().networkStarted += (behavior) =>
        {
            PlayerUiManager.Instance.SetupPlayerList();
        };

        networkObject.SendRpc(RPC_SETUP_LOCAL_PLAYER, Receivers.AllBuffered);
    }


    public override void AssignPlayerToTeam(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            Debug.Log("Recived rpc");
            uint playerNetworkId = args.GetNext<uint>();
            int teamId = args.GetNext<int>();
            Player player = Players.Find(x => x.PlayerNetworkId == playerNetworkId);
            Team team = Teams.Find(x => x.TeamName == teamId.ToString());
            Debug.Log($"Assigning player({player.PlayerName}) to team: {team.TeamName}");

            player.transform.SetParent(team.transform);
            player.Team = team;
            team.Players.Add(player);
        });
    }

    public override void AssignNetworkPlayerToPlayer(RpcArgs args)
    {

    }

    public override void CreateTeam(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            Debug.Log("Recived rpc");
            string teamName = args.GetNext<string>();
            Debug.Log("Creating team: " + teamName);

            GameObject teamObj = new GameObject(teamName);
            teamObj.AddComponent<Team>().TeamName = teamName;
            teamObj.name = "Team: " + teamName;

            Teams.Add(teamObj.GetComponent<Team>());
        });
    }

    public override void CreatePlayer(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            Debug.Log("Recived rpc");
            string playerName = args.GetNext<string>();
            uint networkId = args.GetNext<uint>();
            Debug.Log("Creating player: " + playerName);
            BMSLogger.Instance.Log("Spawning player: " + playerName);

            GameObject playerObj = new GameObject(playerName);
            Player player = playerObj.AddComponent<Player>();
            player.PlayerName = playerName;
            player.PlayerNetworkId = networkId;
            player.Initialize();
            Players.Add(player);
        });
    }

    public override void SetupLocalPlayer(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            Player localPlayer = Players.Find(x => x.PlayerNetworkId == NetworkManager.Instance.Networker.Me.NetworkId);
            GameManager.Instance.ControllingPlayer = localPlayer;
            PlayerUiManager.Instance.UpdateLocalPlayerInfo();
            PlayersSetup.Invoke();
        });
    }

    public Player GetPlayer(uint networkId)
    {
        return Players.Find(x => x.PlayerNetworkId == networkId);
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Generated;
using System.Linq;

public class PlayerManager : PlayerManagerBehavior
{
    public static PlayerManager Instance;

    public List<NetworkingPlayer> NetworkingPlayers = new List<NetworkingPlayer>();
    public List<Player> Players = new List<Player>();
    public List<Team> Teams = new List<Team>();


    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    private void Start()
    {
        //NetworkManager.Instance.MasterServerNetworker.playerAccepted += (player, sender) =>
        //{
        //    NetworkingPlayers.Add(player);
        //};
        //NetworkManager.Instance.MasterServerNetworker.playerDisconnected += (player, sender) =>
        //{
        //    NetworkingPlayers.Remove(player);
        //};
    }

    public void SetupPlayersFromLobby(List<LobbyPlayer> lobbyPlayers)
    {
        if (!networkObject.IsServer)
            return;

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

            GameObject playerObj = new GameObject(playerName);
            Player player = playerObj.AddComponent<Player>();
            player.PlayerName = playerName;
            player.PlayerNetworkId = networkId;

            Players.Add(player);
        });
    }
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;
using BeardedManStudios.SimpleJSON;

public class ServerBrowser : MonoBehaviour
{
    [SerializeField]
    private GameObject serverListingPrefab;
    [SerializeField]
    private Transform serverListingHolder;

    private List<ServerListEntry> cachedServers = new List<ServerListEntry>();

    private TCPMasterClient client;

    private void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        DisposeClient();
        ClearCachedServers();

        client = new TCPMasterClient();

        client.serverAccepted += (worker) =>
        {
            try
            {
                string gameId = "myRTSGame";
                string gameType = "any";
                string gameMode = "all";

                JSONNode sendData = JSONNode.Parse("{}");
                JSONClass getData = new JSONClass();

                getData.Add("id", gameId);
                getData.Add("type", gameType);
                getData.Add("mode", gameMode);

                sendData.Add("get", getData);

                client.Send(BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
            }
            catch
            {
                DisposeClient();
            }
        };

        client.textMessageReceived += (player, frame, sender) =>
        {
            try
            {
                JSONNode data = JSONNode.Parse(frame.ToString());
                if (data["hosts"] != null)
                {
                    MasterServerResponse response = new MasterServerResponse(data["hosts"].AsArray);

                    if (response != null && response.serverResponse.Count > 0)
                    {

                        foreach (Server server in response.serverResponse)
                        {
                            Debug.Log("Found server");
                            Debug.Log("Name: " + server.Name);
                            Debug.Log("Address: " + server.Address);
                            Debug.Log("Port: " + server.Port);
                            Debug.Log("Comment: " + server.Comment);
                            Debug.Log("Type: " + server.Type);
                            Debug.Log("Mode: " + server.Mode);
                            Debug.Log("Players: " + server.PlayerCount);
                            Debug.Log("Max Players: " + server.MaxPlayers);
                            Debug.Log("Protocol: " + server.Protocol);

                            AddServerListing(server);
                        }
                    }
                }
            }
            finally
            {
                if (client != null)
                    DisposeClient();
            }
        };
    }


    void AddServerListing(Server server)
    {
        var listing = Instantiate(serverListingPrefab, serverListingHolder).GetComponent<ServerListEntry>();
        listing.Initialize(server.Name, server.Type, server.Mode, server.PlayerCount, server.MaxPlayers);
        cachedServers.Add(listing);
    }

    void ClearCachedServers()
    {
        cachedServers.Clear();
    }

    void DisposeClient()
    {
        client.Disconnect(true);
        client = null;
    }
}

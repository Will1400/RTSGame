using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;
using BeardedManStudios.SimpleJSON;
using BeardedManStudios.Forge.Networking.Unity;

public class ServerBrowser : MonoBehaviour
{
    public string masterServerIp = "10.205.106.21";
    public ushort masterServerPort = 15940;

    [SerializeField]
    private GameObject serverListingPrefab;
    [SerializeField]
    private Transform serverListingHolder;

    private List<ServerListEntry> cachedServersEntries = new List<ServerListEntry>();
    private List<Server> cachedServers = new List<Server>();

    private TCPMasterClient client;

    private void Start()
    {
        MainThreadManager.Create();
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

                Debug.Log("Sending request to master server");
                client.Send(BeardedManStudios.Forge.Networking.Frame.Text.CreateFromString(client.Time.Timestep, sendData.ToString(), true, Receivers.Server, MessageGroupIds.MASTER_SERVER_GET, true));
            }
            catch
            {
                DisposeClient();
            }
        };

        client.textMessageReceived += (player, frame, sender) =>
        {
            Debug.Log("Recived message");
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

                            cachedServers.Add(server);
                        }
                    }
                }
            }
            finally
            {
                if (client != null)
                    DisposeClient();

                cachedServers.ForEach(x => AddServerListing(x));
            }
        };

        Debug.Log("Connecting to master server");
        client.Connect(masterServerIp, masterServerPort);
    }

    void AddServerListing(Server server)
    {
        MainThreadManager.Run(() =>
        {
            Debug.Log("Adding server listing");
            var listingObj = Instantiate(serverListingPrefab, serverListingHolder);
            var listing = listingObj.GetComponent<ServerListEntry>();
            listing.Initialize(server.Name, server.Type, server.Mode, server.PlayerCount, server.MaxPlayers);
            cachedServersEntries.Add(listing);
        });
    }

    void ClearCachedServers()
    {
        cachedServers.Clear();
        cachedServersEntries.ForEach(x => Destroy(x.gameObject)); 
        cachedServersEntries.Clear();
    }

    void DisposeClient()
    {
        if (client != null)
        {
            Debug.Log("Disposing client");
            client.Disconnect(true);
            client = null;
        }
    }
}

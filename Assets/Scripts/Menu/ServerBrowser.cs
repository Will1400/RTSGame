using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BeardedManStudios.Forge.Networking;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;
using BeardedManStudios.SimpleJSON;
using BeardedManStudios.Forge.Networking.Unity;
using TMPro;
using UnityEngine.UI;

public class ServerBrowser : MonoBehaviour
{
    public static ServerBrowser Instance;

    public string masterServerIp = "10.0.75.1";
    public ushort masterServerPort = 15940;

    [SerializeField]
    private GameObject serverListingPrefab;
    [SerializeField]
    private Transform serverListingHolder;
    [SerializeField]
    private MultiplayerConnectionMenu connectionMenu;
    [SerializeField]
    private Button connectButton;

    private List<ServerListEntry> cachedServersEntries = new List<ServerListEntry>();
    private List<Server> cachedServers = new List<Server>();

    private ServerListEntry selectedServer;
    private TCPMasterClient client;

    private float serverClickedTime;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        MainThreadManager.Create();
        connectButton.interactable = false;
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
            listing.Initialize(server.Name, server.Type, server.Mode, server.PlayerCount, server.MaxPlayers, server);
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

    public void ConnectToSelectedServer()
    {
        if (selectedServer != null)
        {
            connectionMenu.ipAddress.text = selectedServer.server.Address;
            connectionMenu.portNumber.text = selectedServer.server.Port.ToString();
            connectionMenu.Connect();
        }
    }

    public void SelectServer(ServerListEntry serverEntry)
    {
        if (selectedServer == serverEntry && serverClickedTime - Time.time <= 0 && serverClickedTime - Time.time >= -1)
        {
            ConnectToSelectedServer();
        }
        else
        {
            DeselectServer();
            selectedServer = serverEntry;
            serverClickedTime = Time.time;
            connectButton.interactable = true;
        }
    }

    void DeselectServer()
    {
        if (selectedServer != null)
        {
            selectedServer.Deselect();
            selectedServer = null;
        }
    }
}

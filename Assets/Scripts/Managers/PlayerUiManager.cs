using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class PlayerUiManager : PlayerUiManagerBehavior
{
    public static PlayerUiManager Instance;

    [SerializeField]
    private GameObject playerListPrefab;
    private Canvas ui;
    private Transform playerListHolder;
    private Transform playerInfo;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);

        ui = GameObject.Find("UI").GetComponent<Canvas>();
        playerListHolder = ui.transform.Find("Player List/Viewport/Content");
        playerInfo = ui.transform.Find("Player Info Panel/");
    }

    public void SetupPlayerList()
    {
        networkObject.SendRpc(RPC_UPDATE_PLAYER_LIST, true, Receivers.AllBuffered);
    }

    public void UpdateLocalPlayerInfo()
    {
        playerInfo.Find("Player Name").GetComponent<TextMeshProUGUI>().text = GameManager.Instance.ControllingPlayer.PlayerName;
        playerInfo.Find("Unit Count").GetComponent<TextMeshProUGUI>().text = "Units: " + GameManager.Instance.ControllingPlayer.Units.Count.ToString();
    }

    public override void UpdatePlayerList(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            List<Player> players = FindObjectsOfType<Player>().ToList();
            players = players.OrderByDescending(x => x.Score).ToList();

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                var obj = Instantiate(playerListPrefab, playerListHolder);
                obj.transform.SetParent(playerListHolder);
                obj.GetComponent<PlayerListEntry>().Initialize(player.PlayerName, player.Team.TeamName, Color.blue);
            }
        });
    }
}

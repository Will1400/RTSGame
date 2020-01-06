using UnityEngine;
using System.Collections;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class PlayerUiManager : PlayerUiManagerBehavior
{
    public static PlayerUiManager Instance;

    [SerializeField]
    private GameObject playerListPrefab;
    private GameObject playerDiedPanel;
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
        playerDiedPanel = ui.transform.Find("Player Died Panel").gameObject;
        playerDiedPanel.SetActive(false);
    }

    public void SetupPlayerList()
    {
        networkObject.SendRpc(RPC_UPDATE_PLAYER_LIST, true, Receivers.AllBuffered);
    }

    public void LocalPlayerDied()
    {
        playerDiedPanel.SetActive(true);
    }

    public void SetupLocalPlayerInfo()
    {
        playerInfo.Find("Player Name").GetComponent<TextMeshProUGUI>().text = GameManager.Instance.ControllingPlayer.PlayerName;
        playerInfo.Find("Avatar Image").GetComponent<Image>().color = GameManager.Instance.ControllingPlayer.Color;
        UpdateLocalPlayerInfo();
    }

    public void UpdateLocalPlayerInfo()
    {
        playerInfo.Find("Unit Count").GetComponent<TextMeshProUGUI>().text = "Units: " + GameManager.Instance.ControllingPlayer.Units.Count.ToString();
    }

    public override void UpdatePlayerList(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            List<Player> players = FindObjectsOfType<Player>().ToList();
            players = players.OrderByDescending(x => x.Score).ToList();

            foreach (Transform item in playerListHolder)
            {
                Destroy(item.gameObject);
            }

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                var obj = Instantiate(playerListPrefab, playerListHolder);
                obj.transform.SetParent(playerListHolder);
                obj.GetComponent<PlayerListEntry>().Initialize(player.PlayerName, player.Team.TeamName, player.Color);
            }
        });
    }
}

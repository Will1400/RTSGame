using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
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
        PlayerManager.Instance.PlayersSetup.AddListener(() =>
        {
            StartCoroutine(UpdateLocalPlayer());
        });
    }

    IEnumerator UpdateLocalPlayer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            ControllingPlayer = PlayerManager.Instance.Players.Find(x => x.PlayerNetworkId == NetworkManager.Instance.Networker.Me.NetworkId);
        }
    }

}

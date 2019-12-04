using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking;

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance;

    [SerializeField]
    public List<LobbyPlayer> LobbyPlayers = new List<LobbyPlayer>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (scene.name.Contains("Map") && NetworkManager.Instance.IsServer)
            {
                NetworkManager.Instance.InstantiatePlayerManager();
                StartCoroutine(SetupPlayers());
            }

            LobbyPlayers.ForEach(x => Debug.Log(x.Name));
        };
    }

    IEnumerator SetupPlayers()
    {
        yield return new WaitForSeconds(1);
        PlayerManager.Instance.SetupPlayersFromLobby(LobbyPlayers);
    }
}

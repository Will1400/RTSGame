using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private MultiplayerConnectionMenu multiplayerConnection;
    [SerializeField]
    private GameObject MainMenuPanel;
    [SerializeField]
    private GameObject serverBrowserPanel;

    public void OnClickHost()
    {
        multiplayerConnection.Host();
    }

    public void OnClickServers()
    {
        GoToPanel(serverBrowserPanel.name);
    }

    void GoToPanel(string panelName)
    {
        serverBrowserPanel.SetActive(serverBrowserPanel.name.Equals(panelName));
        MainMenuPanel.SetActive(MainMenuPanel.name.Equals(panelName));
    }
}

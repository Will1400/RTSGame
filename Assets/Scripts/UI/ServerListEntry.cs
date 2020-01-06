using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BeardedManStudios.Forge.Networking.MasterServerResponse;

public class ServerListEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI typeText;
    [SerializeField]
    private TextMeshProUGUI modeText;
    [SerializeField]
    private TextMeshProUGUI playersText;
    [SerializeField]
    public Server server { get; private set; }


    public void Initialize(string name, string type, string mode, int players, int maxPlayers, Server server)
    {
        nameText.text = name;
        typeText.text = type;
        modeText.text = mode;
        playersText.text = $"{players}/{maxPlayers}";
        this.server = server;
    }

    public void Select()
    {
        gameObject.GetComponent<Image>().enabled = true;
        ServerBrowser.Instance.SelectServer(this);
    }

    public void Deselect()
    {
        gameObject.GetComponent<Image>().enabled = false;
    }

    private void OnMouseDown()
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

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

    public void Initialize(string name, string type, string mode, int players, int maxPlayers)
    {
        nameText.text = name;
        typeText.text = type;
        modeText.text = mode;
        playersText.text = $"{players}/{maxPlayers}";
    }
}

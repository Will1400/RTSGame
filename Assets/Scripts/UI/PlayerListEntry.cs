using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListEntry : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI nameText;
    [SerializeField]
    private TextMeshProUGUI teamText;
    [SerializeField]
    private Image image;

    public void Initialize(string name, string team, Color color)
    {
        nameText.text = name;
        teamText.text = "T: " + team;
        image.color = color;
    }
}

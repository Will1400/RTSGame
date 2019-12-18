using BeardedManStudios.Forge.Networking.Unity;
using BeardedManStudios.Forge.Networking.Unity.Lobby;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyPlayerItem : MonoBehaviour
{
    public Color[] TeamColors;
    public Color[] AvatarColors;
    public GameObject KickButton;
    public Image AvatarBG;
    public TextMeshProUGUI AvatarID;
    public TMP_InputField PlayerName;
    public TextMeshProUGUI PlayerTeamID;
    public TMP_Dropdown ColorPicker;
    public TMP_Dropdown TeamPicker;

    public Button[] Buttons;

    [HideInInspector]
    public Transform ThisTransform;

    [HideInInspector]
    public GameObject ThisGameObject;

    public LobbyPlayer AssociatedPlayer { get; private set; }
    private LobbyManager _manager;

    public void Init(LobbyManager manager)
    {
        ThisGameObject = gameObject;
        ThisTransform = transform;
        _manager = manager;
    }

    public void Setup(LobbyPlayer associatedPlayer, bool interactableValue)
    {
        ToggleInteractables(interactableValue);
        AssociatedPlayer = associatedPlayer;
        ChangeAvatarID(associatedPlayer.AvatarID);
        ChangeName(associatedPlayer.Name);
        ChangeTeam(associatedPlayer.TeamID);
    }

    public void SetParent(Transform parent)
    {
        ThisTransform.SetParent(parent);
        ThisTransform.localPosition = Vector3.zero;
        ThisTransform.localScale = Vector3.one;
    }

    public void KickPlayer()
    {
        _manager.KickPlayer(this);
    }

    public void RequestChangeTeam(int teamId)
    {
        Debug.Log($"{AssociatedPlayer.Name} want to change teams to: {teamId + 1}");
        _manager.ChangeTeam(this, teamId);
    }

    public void RequestChangeAvatarID(int avatarId)
    {

        _manager.ChangeAvatarID(this, avatarId);
    }

    public void RequestChangeName()
    {
        Debug.Log($"{AssociatedPlayer.Name} want to change name to: {PlayerName.text}");
        _manager.ChangeName(this, PlayerName.text);
    }

    public void ChangeAvatarID(int id)
    {
        AvatarID.text = id.ToString();
        AvatarBG.color = ColorHelper.GetColor(id);
        ColorPicker.value = id;
    }

    public void ChangeName(string name)
    {
        Debug.Log($"{AssociatedPlayer.Name} changed name to: {name}");

        PlayerName.text = name;
    }

    public void ChangeTeam(int id)
    {
        Debug.Log($"{AssociatedPlayer.Name} changed teams to: {id + 1}");
        TeamPicker.value = id;
        PlayerTeamID.text = (id + 1).ToString();
    }

    public void ToggleInteractables(bool value)
    {
        for (int i = 0; i < Buttons.Length; ++i)
            Buttons[i].interactable = value;

        AvatarBG.raycastTarget = value;
        PlayerTeamID.raycastTarget = value;
        PlayerName.interactable = value;
        PlayerName.GetComponent<Image>().enabled = value;
        if (value)
        {
            TeamPicker.onValueChanged.AddListener(RequestChangeTeam);
            ColorPicker.onValueChanged.AddListener(RequestChangeAvatarID);
        }
        else
        {
            TeamPicker.onValueChanged.RemoveAllListeners();
            ColorPicker.onValueChanged.RemoveAllListeners();
        }
        TeamPicker.enabled = value;
        ColorPicker.enabled = value;
    }

    public void ToggleObject(bool value)
    {
        ThisGameObject.SetActive(value);
    }
}

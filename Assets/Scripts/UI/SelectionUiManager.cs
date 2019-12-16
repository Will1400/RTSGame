using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SelectionUiManager : MonoBehaviour
{
    public static SelectionUiManager Instance;

    [SerializeField]
    private GameObject selectedInfoPanel;
    [SerializeField]
    private TextMeshProUGUI selectedName;
    [SerializeField]
    private Image selectedImage;
    [SerializeField]
    private Transform selectedStatContent;

    [SerializeField, Space(5)]
    private GameObject actionsPanel;
    [SerializeField, Space(5)]
    private GameObject contextOptionsPanel;
    [SerializeField, Space(5)]
    private GameObject buildUnitsPanel;

    /// <summary>
    /// Helper to get the currently selected transforms from selection manager (SelectionManager.Instance.Selected)
    /// </summary>
    private List<Transform> currentSelections
    {
        get
        {
            return SelectionManager.Instance.Selected;
        }
    }

    private Dictionary<string, TextMeshProUGUI> stats;

    private void Awake()
    {
        if (Instance is null)
            Instance = this;
        else
            Destroy(gameObject);

        stats = new Dictionary<string, TextMeshProUGUI>();
        foreach (Transform stat in selectedStatContent)
        {
            AddNewStat(stat.name, stat.GetComponent<TextMeshProUGUI>());
        }
    }

    private void Start()
    {
        SelectionManager.Instance.SelectionChanged.AddListener(UpdateContext);
        ShowDefaultContext();
    }

    public void UpdateContext()
    {
        if (currentSelections.Count > 1)
        {
            ShowDefaultContext();
        }
        else if (currentSelections.Count == 1)
        {
            if (currentSelections.FirstOrDefault().TryGetComponent<Unit>(out Unit unit))
            {
                ShowUnitContext(unit.UnitName);
            }
            else
            {
                ShowDefaultContext();
            }
        }
        else
        {
            ShowDefaultContext();
        }
    }

    void ShowDefaultContext()
    {
        UpdateInfo("You");
        DisableStatList();
        buildUnitsPanel.SetActive(true);
    }

    void ShowUnitContext(string unitName)
    {
        buildUnitsPanel.SetActive(false);
        actionsPanel.SetActive(true);
        UpdateInfo(unitName);
        UpdateStats();
    }

    void UpdateInfo(string name)
    {
        selectedName.text = name;
    }

    void UpdateStats()
    {
        DisableStatList();
        if (currentSelections.FirstOrDefault() != null && currentSelections.First().TryGetComponent<ISelectable>(out ISelectable selectable))
        {
            foreach (KeyValuePair<string, float> item in selectable.GetStats())
            {
                if (stats.ContainsKey(item.Key))
                {
                    UpdateStatInfo(item.Key, item.Value);
                }
            }
        }
    }

    void DisableStatList()
    {
        foreach (var item in stats.Values)
        {
            item.enabled = false;
        }
    }

    void AddNewStat(string statName, TextMeshProUGUI textObj)
    {
        stats.Add(statName, textObj);
    }

    void UpdateStatInfo(string key, float value)
    {
        stats[key].text = $"{key}: {value.ToString()}";
        stats[key].enabled = true;
    }

    public void DisableUi()
    {
        SelectionManager.Instance.SelectionChanged.RemoveListener(UpdateContext);
        actionsPanel.SetActive(false);
        buildUnitsPanel.SetActive(false);
        selectedInfoPanel.SetActive(false);
        contextOptionsPanel.SetActive(false);
    }
}

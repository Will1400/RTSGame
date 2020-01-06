using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HQ : MonoBehaviour, ISelectable, IControlledByPlayer
{
    public float Health { get { return healthSystem.Health; } }

    public float Defense { get { return healthSystem.Defense; } }

    public DefenseType DefenseType { get { return healthSystem.DefenseType; } }

    public bool IsSelected { get; set; }

    public Player Owner { get; set; }

    public UnityEvent OnDeath;

    [SerializeField]
    private HealthSystem healthSystem;

    private Material material;

    private void Start()
    {
        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();

        material = new Material(GetComponentInChildren<MeshRenderer>().material);

        healthSystem.OnDeath += Die;
    }

    public void Select()
    {
        Renderer renderer = GetComponentInChildren<MeshRenderer>();
        renderer.material.SetColor("_BaseColor", Color.blue);
        IsSelected = true;
    }

    public void Deselect()
    {
        GetComponent<MeshRenderer>().material = material;
        IsSelected = false;
    }

    public Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "Health", Health },
            { "Defense", Defense }
        };
    }

    void Die()
    {
        OnDeath.Invoke();
    }

    private void OnDestroy()
    {
        healthSystem.OnDeath -= Die;

    }
}

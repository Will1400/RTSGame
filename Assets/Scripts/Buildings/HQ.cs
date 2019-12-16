using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class HQ : MonoBehaviour, IDamageable, ISelectable, IControlledByPlayer
{
    public float Health { get; set; } = 100;

    public float Defense { get; set; }
    public DefenseType DefenseType { get; set; } = DefenseType.AllLight;
    public bool IsSelected { get; set; }
    public Player Owner { get; set; }

    public UnityEvent Die;

    private Material material;

    public void Damage(float amount, DamageType damageType)
    {
        amount = DamageHelper.CalculateEffectiveDamage(amount, damageType, Defense, DefenseType);
        Health -= amount;
        Debug.Log("HQ took: " + amount + " Damage");
        if (Health <= 0)
        {
            Die.Invoke();
        }
    }

    private void Start()
    {
        material = new Material(GetComponentInChildren<MeshRenderer>().material);
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
}

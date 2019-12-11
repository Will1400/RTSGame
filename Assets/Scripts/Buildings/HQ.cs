using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HQ : MonoBehaviour, IDamageable, ISelectable
{
    public float Health { get; set; }

    public float Defense { get; set; }
    public DefenseType DefenseType { get; set; } = DefenseType.AllLight;
    public bool IsSelected { get; set; }

    private Material material;

    public void Damage(float amount, DamageType damageType)
    {
    }

    private void Start()
    {
        material = new Material(GetComponent<MeshRenderer>().material);
    }

    public void Select()
    {
        Renderer renderer = GetComponent<MeshRenderer>();
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
        throw new System.NotImplementedException();
    }
}

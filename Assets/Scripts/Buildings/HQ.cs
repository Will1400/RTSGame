using UnityEngine;
using System.Collections;

public class HQ : MonoBehaviour, IDamageable
{
    public float Health { get; set; }

    public float Defense { get; set; }
    public DefenseType DefenseType { get; set; } = DefenseType.AllLight;

    public void Damage(float amount, DamageType damageType)
    {
    }
}

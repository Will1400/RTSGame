using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{

    public float Defense { get; set; }
    public DefenseType DefenseType { get; set; }

    void Damage(float amount, DamageType damageType);
}

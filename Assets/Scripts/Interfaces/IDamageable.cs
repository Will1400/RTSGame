using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{

    float Health { get; set; }
    float Defense { get; set; }
    DefenseType DefenseType { get; set; }

    void Damage(float amount, DamageType damageType);
}

using UnityEngine;
using System.Collections;
using System;

[Flags]
public enum DamageType
{
    Direct,
    LightMelee = 1,
    HeavyMelee = 2,
    LightRanged = 4,
    HeavyRanged = 8,
    Healing
}
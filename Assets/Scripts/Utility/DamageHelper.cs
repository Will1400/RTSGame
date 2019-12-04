using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DamageHelper
{
    public static float CalculateEffectiveDamage(float damage, DamageType damageType, float defense, DefenseType defenseType)
    {
        float amount = 0;
        if (damageType == DamageType.Direct)
            amount -= damage;

        switch (defenseType)
        {
            case DefenseType.All:
                amount -= defense;
                break;
            case DefenseType.AllLight:
                if (damageType.HasFlag(DamageType.LightMelee) || damageType.HasFlag(DamageType.LightRanged))
                    amount -= defense;
                break;
            case DefenseType.AllHeavy:
                if (damageType.HasFlag(DamageType.HeavyMelee) || damageType.HasFlag(DamageType.HeavyRanged))
                    amount -= defense;
                break;
            case DefenseType.LightMelee:
                if (damageType.HasFlag(DamageType.LightMelee))
                    amount -= defense;
                break;
            case DefenseType.HeavyMelee:
                if (damageType.HasFlag(DamageType.HeavyMelee))
                    amount -= defense;
                break;
            case DefenseType.LightRanged:
                if (damageType.HasFlag(DamageType.LightRanged))
                    amount -= defense;
                break;
            case DefenseType.HeavyRanged:
                if (damageType.HasFlag(DamageType.HeavyRanged))
                    amount -= defense;
                break;
            default:
                break;
        }

        return amount;
    }
}

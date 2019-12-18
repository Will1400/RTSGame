using UnityEngine;
using System.Collections;

public class HealingUnit : Unit
{
    [Header("Ranged Specific")]
    [Space(10)]
    [SerializeField]
    private ParticleSystem healingEffect;
    [SerializeField]
    private float healing;

    // Update is called once per frame
    void Update()
    {

    }

    protected virtual void HealTarget()
    {
        if (target == null)
            return;

        target.GetComponent<IDamageable>().Heal(healing);
    }
}

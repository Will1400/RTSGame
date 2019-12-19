using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class HealingUnit : Unit
{
    public Action OnHeal;

    [Header("Healing Unit Specific")]
    [Space(10)]
    [SerializeField]
    private ParticleSystem healingEffect;
    [SerializeField]
    private float healing;
    [SerializeField, Tooltip("Time between heals")]
    private float healRate;

    private float nextHeal;
    private HealthSystem targetHealthSystem;

    protected override void Setup()
    {
        base.Setup();

        healingEffect.Stop();
        OnHeal += HealTarget;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!initialized)
            return;

        if (target == null || (target != null && IsTargetOutOfRange()))
        {
            target = null;
            if (UnitState != UnitState.Walking)
                agent.ResetPath();
        }
        if (UnitState != UnitState.Walking)
        {
            if (target != null)
            {
                if (targetHealthSystem.IsAtMaxHealth)
                {
                    target = null;
                    return;
                }

                if (CanHealTarget())
                {
                    OnHeal.Invoke();

                    if (agent.hasPath)
                        agent.ResetPath();
                }
                else if (UnitState != UnitState.MoveAttacking || target.position != agent.pathEndPosition)
                {
                    MoveIntoAttackRange(target.position);

                }
            }
            else
            {
                TargetNearbyDamagedFriendly();
            }
        }
    }

    protected virtual bool CanHealTarget()
    {
        return target != null && nextHeal < Time.time && IsInAttackRangeOfPosition(target.position);
    }

    protected virtual void HealTarget()
    {
        if (target == null && nextHeal <= Time.time)
            return;

        if (healingEffect.transform.position != target.position - new Vector3(0, .3f))
            healingEffect.transform.position = target.position - new Vector3(0, .3f);
        healingEffect.Emit(20);

        target.GetComponent<IDamageable>().Heal(healing);
        nextHeal = Time.time + healRate;
    }

    protected virtual void TargetNearbyDamagedFriendly()
    {
        if (!initialized)
            return;
        List<GameObject> possibleTargets = new List<GameObject>(owner.Units);
        possibleTargets.RemoveAll(x => x == gameObject);
        possibleTargets.RemoveAll(x => x.TryGetComponent(out HealthSystem healthSystem) && healthSystem.Health >= healthSystem.StartHealth);

        float shortestDistance = Mathf.Infinity;
        float lowestHealth = Mathf.Infinity;
        foreach (var unit in possibleTargets)
        {
            var healthSystem = unit.GetComponent<HealthSystem>();
            float distanceToEnemy = Vector3.Distance(transform.position, unit.transform.position);
            if (distanceToEnemy <= visionRange && (healthSystem.Health < lowestHealth))
            {
                //shortestDistance = distanceToEnemy;
                lowestHealth = healthSystem.Health;
                target = unit.transform;
                targetHealthSystem = healthSystem;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        OnHeal -= HealTarget;
    }

    public override Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "Health", Health },
            { "Healing", healing },
            { "Defense", Defense }
        };
    }
}

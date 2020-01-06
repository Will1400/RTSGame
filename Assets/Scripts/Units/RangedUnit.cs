using UnityEngine;
using System.Collections;

public class RangedUnit : Unit
{
    [Header("Ranged Unit Specific")]
    [Space(10)]
    [SerializeField]
    private ParticleSystem shootEffect;

    private void Awake()
    {
        shootEffect.Stop();
    }

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
                if (CanAttackTarget())
                {
                    OnAttack.Invoke();

                    if (agent.hasPath)
                        agent.ResetPath();
                }
                else if (UnitState != UnitState.MoveAttacking)
                {
                    MoveIntoAttackRange(target.position);
                }
            }
            else
            {
                TargetNearbyEnemy();
            }
        }
    }

    protected override void AttackTarget()
    {
        shootEffect.Emit(1);
        base.AttackTarget();
    }
}

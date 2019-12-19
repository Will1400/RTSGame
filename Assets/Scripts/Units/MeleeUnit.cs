using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    protected void Update()
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
}

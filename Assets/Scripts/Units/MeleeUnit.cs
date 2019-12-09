using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    protected void Update()
    {
        if (target != null && IsTargetOutOfRange())
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
                    AttackTarget();

                    if (agent.hasPath)
                        agent.ResetPath();
                }
                else
                {
                    MoveIntoAttackRange(target.position);
                }
            }
            else
            {
                GetNearbyTarget();
            }
        }
    }
}

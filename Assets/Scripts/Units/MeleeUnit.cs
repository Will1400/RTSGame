using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    protected void Update()
    {

        if (IsTargetOutOfRange())
        {
            target = null;
            if (UnitState != UnitState.Walking)
                agent.ResetPath();
        }

        if (target != null)
        {
            if (CanAttackTarget() && UnitState != UnitState.Walking)
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
        else if (UnitState != UnitState.Walking)
        {
            GetNearbyTarget();
        }
    }
}

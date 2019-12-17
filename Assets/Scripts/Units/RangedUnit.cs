using UnityEngine;
using System.Collections;

public class RangedUnit : Unit
{
    protected virtual void Update()
    {
        if (!initialized)
            return;

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

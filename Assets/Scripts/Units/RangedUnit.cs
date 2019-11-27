using UnityEngine;
using System.Collections;

public class RangedUnit : Unit
{
    protected virtual void Update()
    {
        if (IsTargetOutOfRange())
        {
            target = null;
            agent.ResetPath();
        }

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

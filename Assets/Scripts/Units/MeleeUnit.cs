using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeUnit : Unit
{
    protected  void Update()
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

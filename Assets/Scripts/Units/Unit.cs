using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.AI;

public abstract class Unit : MonoBehaviour, IDamageable, IControlledByPlayer
{
    #region Fields
    [SerializeField]
    protected Player owner;

    [SerializeField]
    protected string unitName;

    [SerializeField]
    protected float cost;

    [SerializeField]
    protected float health;

    [SerializeField]
    protected float damage;

    [SerializeField]
    protected DamageType damageType;

    [SerializeField]
    protected float range;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float defense;

    [SerializeField]
    protected DefenseType defenseType;

    protected Transform target;

    protected NavMeshAgent agent;
    #endregion

    public Player Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public float Defense
    {
        get { return defense; }
        set { defense = value; }
    }

    public DefenseType DefenseType
    {
        get { return defenseType; }
        set { defenseType = value; }
    }

    public virtual void Damage(float amount, DamageType damageType)
    {

    }

    protected void GetNearbyTarget()
    {
        // Gets all possible targets not controlled by the team
        var possibleTargets = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Units", "Buildings")).Where(x => x.GetComponent<IControlledByPlayer>() != null).ToList();
        possibleTargets.RemoveAll(x => Player.IsOnSameTeam(this, x.GetComponent<IControlledByPlayer>()));

        float shortestDistance = Mathf.Infinity;
        foreach (var target in possibleTargets)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, target.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                this.target = target.transform;
            }
        }
    }

    protected virtual void Start()
    {
        Setup();
    }

    protected virtual void Setup()
    {
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent _agent))
        {
            agent = _agent;
            agent.speed = speed;
        }
    }

    protected bool IsTargetOutOfRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) > range;
    }
}

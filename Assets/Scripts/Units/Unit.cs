using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using UnityEngine.Events;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

public abstract class Unit : UnitBehavior, IDamageable, IControlledByPlayer, ISelectable
{
    #region Fields
    [SerializeField]
    protected Player owner;

    [SerializeField]
    private Image minimapIcon;

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
    protected float visionRange;

    [SerializeField]
    protected float attackRange;

    [SerializeField]
    protected float speed;

    [SerializeField]
    protected float attackRate;

    [SerializeField]
    protected float defense;

    [SerializeField]
    protected DefenseType defenseType;

    protected Transform target;

    protected float nextAttack;

    protected NavMeshAgent agent;

    protected LineRenderer lineRenderer;

    protected bool initialized;

    private Material Material;

    #endregion

    #region Properties
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

    public bool IsSelected { get; set; }

    public string UnitName
    {
        get
        {
            return unitName;
        }
    }

    public UnitState UnitState
    {
        get
        {
            return (UnitState)networkObject.UnitState;
        }
        set
        {
            networkObject.UnitState = (byte)value;
        }
    }

    #endregion

    public virtual void Damage(float amount, DamageType damageType)
    {
        amount = DamageHelper.CalculateEffectiveDamage(amount, damageType, defense, defenseType);

        if (amount < 0)
            amount = 0;

        health -= amount;

        if (health <= 0)
        {
            if (networkObject.IsOwner)
                networkObject.SendRpc(RPC_DIE, Receivers.AllBuffered);
        }
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();
        Setup();

        networkObject.Health = health;
        initialized = true;
    }

    protected virtual void Setup()
    {
        if (TryGetComponent(out NavMeshAgent _agent))
        {
            agent = _agent;
            agent.speed = speed;
        }
        Material = new Material(GetComponent<Renderer>().material);
    }

    public virtual Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "Health", health },
            { "Damage", damage },
            { "Defense", defense }
        };
    }

    protected void GetNearbyTarget()
    {
        if (!initialized)
            return;

        // Gets all possible targets not controlled by the team
        var possibleTargets = Physics.OverlapSphere(transform.position, visionRange, LayerMask.GetMask("Units", "Buildings")).Where(x => x.GetComponent<IControlledByPlayer>() != null).ToList();
        possibleTargets.RemoveAll(x => Player.IsOnSameTeam(this, x.GetComponent<IControlledByPlayer>()));
        possibleTargets.RemoveAll(x => x.gameObject.CompareTag("Units") && x.GetComponent<Unit>().owner == null);

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

    protected bool IsTargetOutOfRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) > visionRange;
    }

    /// <summary>
    /// Moves into attacking range of a specific position
    /// </summary>
    /// <param name="position">Position to move within attack range of</param>
    public virtual void MoveIntoAttackRange(Vector3 position)
    {
        if (!IsInAttackRangeOfPosition(position))
        {
            Vector3 targetPosition = position + ((transform.position - position).normalized * attackRange);
            UnitState = UnitState.MoveAttacking;
            SendRpcMoveToPosition(targetPosition, true);
        }
    }

    /// <summary>
    /// Checks if the current target can be attacked from current position
    /// </summary>
    /// <returns>True if the target can be attacked</returns>
    protected bool CanAttackTarget()
    {
        return target != null && nextAttack < Time.time && IsInAttackRangeOfPosition(target.position);
    }

    protected bool IsInAttackRangeOfPosition(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= attackRange;
    }

    public virtual void OrderStop()
    {
        target = null;
        agent.ResetPath();
    }

    /// <summary>
    /// Attacks the current target without any checks
    /// </summary>
    protected void AttackTarget()
    {
        if (target == null)
            return;

        target.GetComponent<IDamageable>().Damage(damage, damageType);
        nextAttack = Time.time + attackRate;
    }

    public void Select()
    {
        Renderer renderer = GetComponent<MeshRenderer>();
        renderer.material.SetColor("_BaseColor", Color.green);
        IsSelected = true;
    }

    public void Deselect()
    {
        GetComponent<MeshRenderer>().material = Material;
        IsSelected = false;
    }

    public virtual void SendRpcMoveToPosition(Vector3 position, bool keepState = false)
    {
        if (!keepState)
        {
            UnitState = UnitState.Walking;
        }

        networkObject.SendRpc(RPC_MOVE_TO_POSITION, Receivers.All, position);
    }

    public virtual void SendRpcOrderStop()
    {
        UnitState = UnitState.Idle;
        networkObject.SendRpc(RPC_ORDER_STOP, Receivers.All);
    }
    protected void FixedUpdate()
    {
        if (!initialized)
            return;

        if (!agent.hasPath)
            UnitState = UnitState.Idle;

        SyncObject();
    }

    protected void SyncObject()
    {
        if (networkObject.IsOwner)
        {
            networkObject.Position = transform.position;
            networkObject.Rotation = transform.rotation;
        }
        else
        {
            transform.position = networkObject.Position;
            transform.rotation = networkObject.Rotation;
        }
    }

    public override void MoveToPosition(RpcArgs args)
    {
        agent.SetDestination(args.GetNext<Vector3>());
    }

    public override void Die(RpcArgs args)
    {
        if (networkObject.IsOwner)
        {
            SelectionManager.Instance.Selected.Remove(transform);
            PlayerUiManager.Instance.UpdateLocalPlayerInfo();
        }
        PlayerManager.Instance.GetPlayer(owner.PlayerNetworkId).Units.Remove(gameObject);

        Destroy(gameObject);
    }

    public override void OrderStop(RpcArgs args)
    {
        target = null;
        agent.ResetPath();
    }

    public override void AssignToPlayer(RpcArgs args)
    {
        Player player = PlayerManager.Instance.GetPlayer(args.GetNext<uint>());
        owner = player;
        minimapIcon.color = owner.Color;
        player.Units.Add(gameObject);
        transform.SetParent(player.UnitHolder);
    }
}

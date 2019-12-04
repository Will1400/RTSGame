﻿using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.AI;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking;
using UnityEngine.Events;

public abstract class Unit : UnitBehavior, IDamageable, IControlledByPlayer, ISelectable
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

    #endregion

    private Material Material;

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


    public UnityEvent UnitDied;




    public virtual void Damage(float amount, DamageType damageType)
    {
        amount = DamageHelper.CalculateEffectiveDamage(amount, damageType, defense, defenseType);

        if (amount < 0)
            amount = 0;

        health -= amount;

        if (health <= 0)
        {
            UnitDied.Invoke();
        }
    }

    protected void GetNearbyTarget()
    {
        // Gets all possible targets not controlled by the team
        var possibleTargets = Physics.OverlapSphere(transform.position, visionRange, LayerMask.GetMask("Units", "Buildings")).Where(x => x.GetComponent<IControlledByPlayer>() != null).ToList();
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

    protected override void NetworkStart()
    {
        base.NetworkStart();

        networkObject.Health = health;
    }

    protected virtual void Setup()
    {
        if (TryGetComponent(out NavMeshAgent _agent))
        {
            agent = _agent;
            agent.speed = speed;
        }

        UnitDied = new UnityEvent();

        Material = new Material(GetComponent<Renderer>().material);
    }


    protected bool IsTargetOutOfRange()
    {
        return target != null && Vector3.Distance(transform.position, target.position) > visionRange;
    }

    public virtual void MoveIntoAttackRange(Vector3 position)
    {
        if (!IsInAttackRangeOfPosition((position)))
        {
            Vector3 targetPosition = position + ((transform.position - position).normalized * attackRange);
            SendRpcMoveToPosition(targetPosition);
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

    public virtual void SendRpcMoveToPosition(Vector3 position)
    {
        networkObject.SendRpc(RPC_MOVE_TO_POSITION, Receivers.All, position);
    }

    public virtual void SendRpcOrderStop()
    {
        networkObject.SendRpc(RPC_ORDER_STOP, Receivers.All);
    }

    // RPC
    public override void MoveToPosition(RpcArgs args)
    {
        agent.SetDestination(args.GetNext<Vector3>());
    }

    // RPC
    public override void OrderStop(RpcArgs args)
    {
        target = null;
        agent.ResetPath();
    }

    protected void FixedUpdate()
    {
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


}

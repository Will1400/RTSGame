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
using System;

[RequireComponent(typeof(HealthSystem))]
public abstract class Unit : UnitBehavior, IControlledByPlayer, ISelectable
{
    #region Fields
    [SerializeField]
    protected Player owner;

    [SerializeField]
    private Image minimapIcon;

    [SerializeField]
    private GameObject selectedIndicator;

    [SerializeField]
    protected string unitName;

    [SerializeField]
    protected float cost;

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

    [SerializeField] // to show in editor
    protected Transform target;

    protected float nextAttack;

    protected NavMeshAgent agent;

    protected LineRenderer lineRenderer;

    protected bool initialized;

    protected HealthSystem healthSystem;

    #endregion

    #region Properties
    public Player Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    public float Health
    {
        get { return healthSystem.Health; }
    }

    public float Defense
    {
        get { return healthSystem.Defense; }
    }

    public DefenseType DefenseType
    {
        get { return healthSystem.DefenseType; }
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

    #region Events
    public Action OnAttack;
    #endregion

    protected override void NetworkStart()
    {
        base.NetworkStart();
        Setup();

        initialized = true;
    }

    protected virtual void Setup()
    {
        if (TryGetComponent(out NavMeshAgent _agent))
        {
            agent = _agent;
            agent.speed = speed;
        }
        healthSystem = GetComponent<HealthSystem>();

        networkObject.Health = Health;
        healthSystem.OnDeath += Die;
        OnAttack += AttackTarget;
    }

    protected virtual void ChangeColors()
    {
        Color mainColor = owner.Color;
        Renderer renderer;
        if (transform.Find("Goggles"))
            renderer = transform.Find("Goggles").GetComponent<Renderer>();
        else
            renderer = transform.Find("Model/Goggles").GetComponent<Renderer>();

        renderer.material.SetColor("_BaseColor", mainColor);
    }

    public virtual Dictionary<string, float> GetStats()
    {
        return new Dictionary<string, float>
        {
            { "Health", Health },
            { "Damage", damage },
            { "Defense", Defense }
        };
    }

    protected virtual void TargetNearbyEnemy()
    {
        if (!initialized)
            return;

        // Gets all possible targets not controlled by the team
        var possibleTargets = Physics.OverlapSphere(transform.position, visionRange, LayerMask.GetMask("Units", "Buildings")).ToList();
        possibleTargets.RemoveAll(x => x.GetComponent<IControlledByPlayer>() == null);
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
            Vector3 targetPosition = position + ((transform.position - position).normalized * (attackRange + .2f));
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
        return target != null && nextAttack <= Time.time && IsInAttackRangeOfPosition(target.position);
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
    protected virtual void AttackTarget()
    {
        if (target == null)
            return;

        transform.LookAt(target);
        target.GetComponent<IDamageable>().Damage(damage, damageType);
        nextAttack = Time.time + attackRate;
    }

    public void Select()
    {
        selectedIndicator.SetActive(true);
        IsSelected = true;
    }

    public void Deselect()
    {
        selectedIndicator.SetActive(false);
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
            networkObject.Health = Health;
            networkObject.Position = transform.position;
            networkObject.Rotation = transform.rotation;
        }
        else
        {
            healthSystem.Health = networkObject.Health;
            transform.position = networkObject.Position;
            transform.rotation = networkObject.Rotation;
        }
    }

    protected virtual void OnDestroy()
    {
        OnAttack -= AttackTarget;
        //healthSystem.OnDeath -= Die;

    }

    protected void Die()
    {
        healthSystem.OnDeath -= Die;
        networkObject.SendRpc(RPC_DIE, Receivers.AllBuffered);
    }

    public override void MoveToPosition(RpcArgs args)
    {
        if (agent != null)
            agent.SetDestination(args.GetNext<Vector3>());
    }

    public override void Die(RpcArgs args)
    {
        if (networkObject.IsOwner)
        {
            SelectionManager.Instance.Selected.Remove(transform);
            PlayerUiManager.Instance.UpdateLocalPlayerInfo();
        }
        if (gameObject != null)
        {
            PlayerManager.Instance.GetPlayer(owner.PlayerNetworkId).Units.Remove(gameObject);
        }

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
        if (minimapIcon != null)
            minimapIcon.color = owner.Color;

        ChangeColors();
        owner.Units.Add(gameObject);
        transform.SetParent(player.UnitHolder);
    }
}

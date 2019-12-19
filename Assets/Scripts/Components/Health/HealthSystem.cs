using UnityEngine;
using System.Collections;
using System;

public class HealthSystem : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float health;
    [SerializeField]
    private float defense;
    [SerializeField]
    private DefenseType defenseType;
    [SerializeField]
    private bool destroyObjectOnDeath;

    private float startHealth;

    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public float StartHealth
    {
        get { return startHealth; }
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

    public bool IsAtMaxHealth { get { return health >= startHealth; } }

    public Action HealthChanged;
    public Action OnDeath;

    private void Start()
    {
        startHealth = health;

        if (destroyObjectOnDeath)
            OnDeath += () => { Destroy(gameObject, 1); };
    }

    public void Damage(float amount, DamageType damageType)
    {
        amount = DamageHelper.CalculateEffectiveDamage(amount, damageType, defense, defenseType);

        if (amount < 0)
            amount = 0;

        health -= amount;

        HealthChanged.Invoke();

        if (health <= 0)
            OnDeath.Invoke();
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > startHealth)
            health = startHealth;

        HealthChanged.Invoke();
    }
}

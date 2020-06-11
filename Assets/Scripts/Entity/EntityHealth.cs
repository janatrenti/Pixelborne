using UnityEngine;

/// <summary>Manages the health of an entity.</summary>
public class EntityHealth : MonoBehaviour
{
    [SerializeField]
    private int m_maxHealth;

    /// <summary>Gets or sets a value indicating whether this <see cref="EntityHealth"/> is invincible.</summary>
    /// <value>
    ///     <c>true</c> if invincible; otherwise, <c>false</c>.</value>
    public bool Invincible { get; set; }
    /// <summary>Gets the current health.</summary>
    /// <value>The current health.</value>
    public int CurrentHealth { get; private set; }

    /// <summary>Gets a value indicating whether this instance has zero health.</summary>
    /// <value>
    ///     <c>true</c> if this instance has zero health; otherwise, <c>false</c>.</value>
    public bool IsZero 
    {
        get
        { 
            return CurrentHealth <= 0;
        }
    }

    /// <summary>Gets the maximum health.</summary>
    /// <value>The maximum health.</value>
    public int MaxHealth
    {
        get
        {
            return m_maxHealth;
        }
        private set { }
    }

    void Start()
    {
        Revive();
    }

    /// <summary>Revives the entity by resetting its m_currentHealth.</summary>
    public void Revive()
    {
        CurrentHealth = MaxHealth;
        Invincible = false;
    }

    /// <summary>Ensures that the entity has 0 health.
    ///     It is used to ensure that the health are zero when dying by a death zone.</summary>
    public void Die() 
    {
        TakeDamage(m_maxHealth);
    }

    /// <summary>Deals damage to the entity by reducing its m_currentHealth.</summary>
    /// <param name="damage">The damage that the entity should take.</param>
    public void TakeDamage(int damage)
    {
        if (!Invincible)
        {
            CurrentHealth -= damage;
            // Ensure that health points cannot be negative.
            CurrentHealth = CurrentHealth < 0 ? 0 : CurrentHealth;
        }
    }
}

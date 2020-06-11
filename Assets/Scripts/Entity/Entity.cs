using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

/// <summary>Is the base class for all entities that can execute actions like walking or attacking.
///     It unifies the duplicate state and behaviour from <see cref="EnemyActions"/> and <see cref="PlayerActions"/>.</summary>
public abstract class Entity : MonoBehaviour, IAttack
{
    /// <summary>The weapon collider from the entity.</summary>
    [SerializeField]
    protected BoxCollider2D m_weaponCollider;
    /// <summary>Is the entity facing in right.</summary>
    [SerializeField]
    protected bool m_isFacingRight;
    /// <summary>The jump force of the entity.</summary>
    [SerializeField]
    protected float m_jumpForce = 22.0f;
    /// <summary>The move speed of the entity.</summary>
    [SerializeField]
    protected float m_moveSpeed = 10.0f;
    /// <summary>The attack damage of the entity.</summary>
    [SerializeField]
    protected int m_attackDamage = 1;
    [SerializeField]
    private LayerMask m_whatIsGround;

    /// <summary>The animator of the entity.</summary>
    protected Animator m_animator;
    /// <summary>The rigidbody of the entity.</summary>
    protected Rigidbody2D m_rigidbody2D;
    /// <summary>The collider of the entity.</summary>
    protected BoxCollider2D m_collider;
    /// <summary>The entity health of the entity. Can be null in order to create an invincible entity.</summary>
    protected EntityHealth m_entityHealth;

    /// <summary>Is the entity grounded.</summary>
    protected bool m_isGrounded = false;
    /// <summary>The current attacking direction.</summary>
    protected int m_currentAttackingDirection = 0;
    /// <summary>The horizontal is grounded distance. Is used to smooth jumping.</summary>
    protected static readonly float HORIZONTAL_IS_GROUNDED_DISTANCE = 0.1f;
    /// <summary>The vertical is grounded distance. Is used to smooth jumping.</summary>
    protected static readonly float VERTICAL_IS_GROUNDED_DISTANCE = 0.2f;
    /// <summary>The attack animator parameter names.</summary>
    protected static readonly string[] ATTACK_ANIMATOR_PARAMETER_NAMES = { "AttackingUp", "Attacking", "AttackingDown" };
    /// <summary>The jumping animator parameter name.</summary>
    protected static readonly string JUMPING_ANIMATOR_PARAMETER_NAME = "IsJumping";
    /// <summary>The speed animator parameter name.</summary>
    protected static readonly string SPEED_ANIMATOR_PARAMETER_NAME = "Speed";

    /// <summary>The death zones name.</summary>
    public static readonly string DEATH_ZONES_NAME = "DeathZones";
    /// <summary>Gets or sets a value indicating whether this instance is input locked.</summary>
    /// <value>
    ///   <c>true</c> if this instance is input locked; otherwise, <c>false</c>.</value>
    public bool IsInputLocked { get; set; } = false;
    /// <summary>Gets or sets a value indicating whether this instance is attacking.</summary>
    /// <value>
    ///   <c>true</c> if attacking; otherwise, <c>false</c>.</value>
    public bool IsAttacking { get; protected set; }
    /// <summary>Gets or sets a value indicating whether this instance is rolling.</summary>
    /// <value>
    ///   <c>true</c> if this instance is rolling; otherwise, <c>false</c>.</value>
    public bool IsRolling { get; protected set; } = false;

    /// <summary>Awakes this instance by setting all referenced components.</summary>
    protected virtual void Awake()
    {
        m_animator = gameObject.GetComponent<Animator>();
        m_rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        m_collider = gameObject.GetComponent<BoxCollider2D>();
        m_entityHealth = gameObject.GetComponent<EntityHealth>();
        m_weaponCollider = gameObject.transform.GetChild(0).GetComponent<BoxCollider2D>();
    }

    /// <summary>
    ///     Starts this instance. Initially flips the enemy if it is not facing right.
    ///     It also ensures that the entity is currently not attacking and disables the weapon collider.
    /// </summary>
    protected virtual void Start()
    {
        if (!m_isFacingRight)
        {
            FlipEntity();
        }
        m_weaponCollider.enabled = false;
        IsAttacking = false;
    }

    /// <summary>Updates this instance by setting the grounded status.</summary>
    protected virtual void Update() 
    {
        UpdateIsGrounded();
        m_animator.SetBool(JUMPING_ANIMATOR_PARAMETER_NAME, !m_isGrounded);
    }

    /// <summary>Updates the grounded status.</summary>
    protected void UpdateIsGrounded()
    {
        m_isGrounded = Physics2D.OverlapArea((Vector2) m_collider.bounds.min - new Vector2(HORIZONTAL_IS_GROUNDED_DISTANCE, 0.0f),
                        (Vector2)m_collider.bounds.min + new Vector2(m_collider.bounds.size.x + HORIZONTAL_IS_GROUNDED_DISTANCE, 
                        -VERTICAL_IS_GROUNDED_DISTANCE), m_whatIsGround);
    }

    /// <summary>Flips the entity sprite.</summary>
    protected virtual void FlipEntity()
    {
        m_isFacingRight = !m_isFacingRight;
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
    }

    /// <summary>Executes the common behavior when an entity is dying.</summary>
    protected virtual void Die()
    {
        StopAttacking();
    }

    /// <summary>Called when entered the trigger.</summary>
    /// <param name="collider">The collider that entered the entity collider.</param>
    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        // Undefeatable objects cannot be damaged. e.g. princess.
        if(m_entityHealth == null)
        {
            return;
        }
        if (!IsInputLocked)
        {
            if (collider.gameObject.name == DEATH_ZONES_NAME)
            {
                Die();
            }

            Vector2 colliderDirection = gameObject.transform.position - collider.gameObject.transform.position;
            bool attackerNeedsToFaceRight = colliderDirection.x > 0.0f ? true : false;
            IAttack enemyAttack = collider.gameObject.transform.parent.GetComponent<IAttack>();

            // Take damage if the collider comes from an attacker and the attacks are not cancelling each other.
            if (enemyAttack != null && (!IsAttackCancelling(enemyAttack.GetAttackDirection(), enemyAttack.IsFacingRight()
                && attackerNeedsToFaceRight == enemyAttack.IsFacingRight()))
                && collider.enabled)
            {
                m_entityHealth.TakeDamage(enemyAttack.GetAttackDamage());
                if (m_entityHealth.IsZero)
                {
                    Die();
                }
            }
        }
    }

    /// <summary>Called when the entity should jump e.g. the player presses the jump button.</summary>
    /// <param name="value">The input value which is not used but necessary to fit the PlayerInput-Interface from Unity.</param>
    protected void OnJump(InputValue value)
    {
        if (!IsInputLocked && !IsRolling && m_isGrounded)
        {
            m_animator.SetBool(JUMPING_ANIMATOR_PARAMETER_NAME, true);
            m_rigidbody2D.velocity = new Vector2(m_rigidbody2D.velocity.x, m_jumpForce);
        }
    }

    /// <summary>Resets the attack animations.</summary>
    protected void ResetAttackAnimation()
    {
        IsAttacking = false;
        m_weaponCollider.enabled = false;
        foreach (string parameter in ATTACK_ANIMATOR_PARAMETER_NAMES)
        {
            m_animator.SetBool(parameter, false);
        }
    }

    /// <summary>Resets the entity animations.</summary>
    public virtual void ResetEntityAnimations()
    {
        m_animator.SetBool(JUMPING_ANIMATOR_PARAMETER_NAME, false);
        m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, 0);
        ResetAttackAnimation();
    }

    /// <summary>Resets the movement. The vertical movement is ignored.</summary>
    public virtual void ResetMovement()
    {
        m_rigidbody2D.velocity = new Vector2(0, m_rigidbody2D.velocity.y);
    }

 
    /// <summary> StartAttacking is triggered by the attack animations
    ///     in order to mark the time window where the attack deals damage.</summary>
    public void StartAttacking()
    {
        m_weaponCollider.enabled = true;
        m_weaponCollider.isTrigger = true;
    }

    /// <summary> StopAttacking is triggered by the attack animations
    ///     in order to mark the time window where the attack deals damage.</summary>
    public virtual void StopAttacking()
    {
        m_weaponCollider.enabled = false;
        m_weaponCollider.isTrigger = false;
    }

    /// <summary>Resets the entity actions.</summary>
    public void ResetEntityActions()
    {
        m_entityHealth.Revive();
        ResetEntityAnimations();
        ResetMovement();
    }

    /// <summary>Determines whether the entity [is facing right].</summary>
    /// <returns>
    ///     <c>true</c> if [is facing right]; otherwise, <c>false</c>.</returns>
    public bool IsFacingRight()
    {
        return m_isFacingRight;
    }

    /// <summary>Determines whether the attack in cancelling.
    ///     Attacks cancel each other if they are on the same height, both are currently in the deal damage window
    ///     and the facing direction is not the same.</summary>
    /// <param name="attackDirectionFromOtherEntity">The attack direction from the other entity.</param>
    /// <param name="otherEntityIsFacingRight">If the other entity is facing right.</param>
    /// <returns>
    ///     <c>true</c> if the attack is canceled; otherwise, <c>false</c>.</returns>
    public bool IsAttackCancelling(int attackDirectionFromOtherEntity, bool otherEntityIsFacingRight)
    {
        return (attackDirectionFromOtherEntity == m_currentAttackingDirection) && m_weaponCollider.enabled
            && (otherEntityIsFacingRight != m_isFacingRight);
    }

    /// <summary>Gets the attack direction.</summary>
    /// <returns>The current attacking direction.</returns>
    public int GetAttackDirection()
    {
        return m_currentAttackingDirection;
    }

    /// <summary>Gets the attack damage.</summary>
    /// <returns>Returns the attack damage.</returns>
    public int GetAttackDamage()
    {
        return m_attackDamage;
    }
}

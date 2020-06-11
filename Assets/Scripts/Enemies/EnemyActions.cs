using System.Diagnostics;
using UnityEngine;

/// <summary>Is attached to enemy game objects in order to simply let them execute actions that are defined in <see cref="IEnemyActions"/>.
///     It takes care of the animations, physics and health.
///     It is intended to be used with an <see cref="ActionPatternExecutor"/> attached to the same game object.
///     Though it can be used without it.</summary>
/// <example>
/// <code>
///     m_princessActions = gameobjectFind("princess").GetComponent<IEnemyActions>();
///     m_princess.StartFollowPlayer();
/// </code>
/// </example>
public class EnemyActions : Entity, IEnemyActions
{
    [SerializeField]
    private bool m_isFriendlyFireActive = false;
    [SerializeField]
    private bool m_bodyShouldDisappear = true;
    [SerializeField]
    private float m_attackRange = 10.0f;
    [SerializeField]
    private float m_minPlayerDistance = 0.25f;
    [SerializeField]
    private float m_sightRange = 10.0f;

    private bool m_isAttackChained = false;
    private bool m_isAutoJumping = false;
    private bool m_isFollowingPlayer = false;
    private Stopwatch m_stopwatchForRevivePositionTiming = new Stopwatch(); 
    private string m_playerSwordName;
    private Vector2 m_lastPosition = new Vector2();
    protected Rigidbody2D m_playerRigidbody2D;

    // Time in milliseconds.
    private static readonly float INTERVAL_FOR_POSITION_CHECK = 200;
    private static readonly float AUTO_JUMPING_ACTIVATION_DISTANCE = 0.001f;
    private static readonly string[] ATTACK_ANIMATION_NAMES = { "attack_up", "attack_mid", "attack_down" };

    /// <summary>The animator parameter name for the dying animation.</summary>
    protected static readonly string DYING_ANIMATOR_PARAMETER_NAME = "IsDying";
    protected static readonly string DEAD_ANIMATOR_PARAMETER_NAME = "IsDead";

    /// <summary>Awakes this instance and registers to enemy in the singleplayer instance.</summary>
    protected override void Awake()
    {
        base.Awake();
        Singleplayer.Instance.ActiveEnemies.Add(gameObject);
    }

    protected override void Start()
    {
        base.Start();
        m_stopwatchForRevivePositionTiming.Start();
    }

    /// <summary>Updates this instance every frame.
    ///     It takes care of casing the player and automatic jumping.</summary>
    protected override void Update()
    {
        base.Update();

        if (Singleplayer.Instance.Player == null)
        {
            return; 
        } 
        else if (m_playerRigidbody2D == null || m_playerSwordName == null)
        {
            GameObject player = Singleplayer.Instance.Player;
            m_playerRigidbody2D = player.GetComponent<Rigidbody2D>();
            m_playerSwordName = player.GetComponent<PlayerActions>().PlayerSword.name;
        }
        else
        {
            UpdateMovementAndAutoJumping();
        }
    }

    private void UpdateMovementAndAutoJumping()
    {
        float movementDirection = m_playerRigidbody2D.position.x - m_rigidbody2D.position.x;
        // Only walk if we chase the player, the input is not locked and the player is not too close.
        if (m_isFollowingPlayer && !IsInputLocked && Mathf.Abs(movementDirection) > m_minPlayerDistance)
        {
            // Normalize the movementDirection.
            movementDirection = movementDirection < 0 ? -1 : 1;
            m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, Mathf.Abs(movementDirection));

            // Flip enemy direction if player now walks in opposite direction.
            if (movementDirection < 0.0f && m_isFacingRight || movementDirection > 0.0f && !m_isFacingRight)
            {
                FlipEntity();
            }
            // Apply the movement to the physics.
            m_rigidbody2D.velocity = new Vector2(movementDirection * m_moveSpeed, m_rigidbody2D.velocity.y);

            if (m_isAutoJumping)
            {
                // If jumping is turned on then jump if the current position is almost equal to the last position.
                // It is only checked every INTERVALL_FOR_POSITION_CHECK.
                if (m_stopwatchForRevivePositionTiming.ElapsedMilliseconds >= INTERVAL_FOR_POSITION_CHECK)
                {
                    if (Vector2.Distance(m_lastPosition, gameObject.transform.position) < AUTO_JUMPING_ACTIVATION_DISTANCE)
                    {
                        OnJump(null);
                    }
                    m_lastPosition = gameObject.transform.position;
                    m_stopwatchForRevivePositionTiming.Restart();
                }
            }
        }
        else
        {
            // Stop the walking animation if the enemy is too close to the player.
            m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, 0);
        }
    }

    /// <summary>Initiates the entity dying animation and ensures that the enemy does nothing else.</summary>
    protected override void Die(){
        base.Die();
        m_animator.SetBool(DYING_ANIMATOR_PARAMETER_NAME, true);
        IsInputLocked = true;
    }

    // This method starts the new attack.
    // This rather inconvenient approach is needed in order to avoid a problem
    // that takes place when attacks are directly chained by the AttackPatternExecutor.
    private void StartAttackIfPossible(int attackDirectionIndex)
    {
        if (!IsInputLocked)
        {
            m_currentAttackingDirection = attackDirectionIndex;
            if (!m_isAttackChained)
            {
                m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], true);
            }
            m_isAttackChained = true;
        }
    }

    /// <summary>Called when a trigger-collider enters the collider of the enemy.
    ///     It is used to determine if the enemy got hit by a weapon and if that weapon is allowed to deal damage 
    ///     e.g. the attack is not canceled.
    ///     When no <see cref="EntityHealth"/> is attached to the game object the entity counts as not defeatable.
    ///     This is used for the princess.</summary>
    /// <param name="collider">The collider that entered the collider of the entity.</param>
    protected override void OnTriggerEnter2D(Collider2D collider) {
        // We abort if the collider is not from a player when friendly fire is off.
        if (!m_isFriendlyFireActive && collider.gameObject.name != m_playerSwordName && collider.gameObject.name != DEATH_ZONES_NAME)
        {
            return;
        }
        base.OnTriggerEnter2D(collider);
    }

    // These methods implement the IEnemyAttackAndMovement that is needed by the AttackPatternExecutor.

    /// <summary>Starts an upper attack when possible from the current enemy state.</summary>
    public void AttackUp()
    {
        StartAttackIfPossible(0);
    }

    /// <summary>Starts a middle attack when possible from the current enemy state.</summary>
    public void AttackMiddle()
    {
        StartAttackIfPossible(1);
    }

    /// <summary>Starts a down attack when possible from the current enemy state.</summary>
    public void AttackDown()
    {
        StartAttackIfPossible(2);
    }

    /// <summary>Starts a jump.</summary>
    public void Jump()
    {
        OnJump(null);
    }

    /// <summary>Starts following the player.</summary>
    public void StartFollowPlayer()
    {
        m_isFollowingPlayer = true;
    }

    /// <summary>Stops following the player.</summary>
    public void StopFollowPlayer()
    {
        m_isFollowingPlayer = false;
        m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, 0);
    }

    /// <summary>Starts automatic jumping.</summary>
    public void StartAutoJumping(){
        m_isAutoJumping = true;
    }

    /// <summary>Stops automatic jumping.</summary>
    public void StopAutoJumping(){
        m_isAutoJumping = false;
    }

    /// <summary>Gets the duration of the attack up animation.</summary>
    /// <returns>The attack up animation length.</returns>
    public float GetAttackUpDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, ATTACK_ANIMATION_NAMES[0]);
    }

    /// <summary>Gets the duration of the attack middle animation.</summary>
    /// <returns>The attack middle animation length.</returns>
    public float GetAttackMiddleDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, ATTACK_ANIMATION_NAMES[1]);
    }

    /// <summary>Gets the duration of the attack down animation.</summary>
    /// <returns>The attack down animation length.</returns>
    public float GetAttackDownDuration()
    {
        return Toolkit.GetAnimationLength(m_animator, ATTACK_ANIMATION_NAMES[2]);
    }

    /// <summary>Determines whether [is player in attack range].</summary>
    /// <returns>
    ///     <c>true</c> if [is player in attack range]; otherwise, <c>false</c>.</returns>
    public bool IsPlayerInAttackRange()
    {
        if (m_playerRigidbody2D == null)
        {
            return false;
        }
        return m_attackRange >= Vector2.Distance(m_playerRigidbody2D.transform.position, gameObject.transform.position);
    }

    /// <summary>Determines whether [is player in sight range].</summary>
    /// <returns>
    ///     <c>true</c> if [is player in sight range]; otherwise, <c>false</c>.</returns>
    public bool IsPlayerInSightRange()
    {
        if (m_playerRigidbody2D == null)
        {
            return false;
        }
        return m_sightRange >= Vector2.Distance(m_playerRigidbody2D.transform.position, gameObject.transform.position);
    }

    /// <summary>Determines whether [is enemy on ground].</summary>
    /// <returns>
    ///     <c>true</c> if [is enemy on ground]; otherwise, <c>false</c>.</returns>
    public bool IsEnemyOnGround()
    {
        UpdateIsGrounded();
        return m_isGrounded;
    }

    /// <summary>Stops the attacking.</summary>
    public override void StopAttacking()
    {
        base.StopAttacking();
        m_isAttackChained = false;
    }

    /// <summary>Is called at the end of the attack animation
    ///     and turns the attack off animation when no other attack is already registered.
    ///     This is part of the attack chaining problem.</summary>
    /// <param name="previousAttackingDirection">The attacking direction from the attack animation that called this function.</param>
    public void StopAttackingAnimation(int previousAttackingDirection)
    {
        if (!m_isAttackChained)
        {
            // Stop the ended attack.
            m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[previousAttackingDirection], false);
        }
        else if (previousAttackingDirection != m_currentAttackingDirection) 
        {
            // Stop the ended attack.
            m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[previousAttackingDirection], false);
            // Start the new attack that has a different direction.
            m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], true);
        }
        // Reset the attribute.
        m_isAttackChained = false;
    }

    // This method destroys the gameObject if the body should disappear. 
    // Otherwise it changes the layer to disabled collision layer and plays the dead animation.
    void DestroySelf()
    {
        if(m_bodyShouldDisappear)
        {
            Destroy(gameObject);
        }
        else
        {
            int disabledCollisionLayer = LayerMask.NameToLayer("DisabledCollisionLayer");
            gameObject.layer = disabledCollisionLayer;
            ResetEntityAnimations();
            m_animator.SetBool(DYING_ANIMATOR_PARAMETER_NAME, false);
            m_animator.SetBool(DEAD_ANIMATOR_PARAMETER_NAME, true);
            Singleplayer.Instance.ActiveEnemies.Remove(gameObject);
        }
    }

    void OnDestroy()
    {
        Singleplayer.Instance.ActiveEnemies.Remove(gameObject);
    }
}

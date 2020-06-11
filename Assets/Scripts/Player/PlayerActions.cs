using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Experimental.Input.Plugins.PlayerInput;

/// <summary>Handles the player input and executes these actions. 
///     It adds the user input dependent code, rolling and revive position functionality.</summary>
public class PlayerActions : Entity
{
    [SerializeField]
    private GameObject m_playerSword;
    [SerializeField]
    private int m_playerIndex = 1;
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_playerPositionsTransform;

    private bool m_hasStablePosition = false; 
    private float m_attackDuration; 
    private float m_attackDirection;
    private float m_rollingDuration;
    private float m_rollingMovementX;
    private IGame m_activeGame;
    private SpriteRenderer m_swordRenderer;
    private Stopwatch m_stopwatchRevive = new Stopwatch();
    private Stopwatch m_stopwatchAttack = new Stopwatch();
    private Stopwatch m_stopwatchRolling = new Stopwatch();
    private Vector2 m_nonRollingColliderSize;
    private Vector2 m_rollingColliderSize;
    private Vector2 m_lastCheckedPosition;

    private static readonly float CONTROLLER_DEADZONE = 0.3f;
    private static readonly float ROLLING_INVINCIBILITY_TIME_SPAN_START = 0.2f;
    private static readonly float ROLLING_INVINCIBILITY_TIME_SPAN_END = 0.8f;
    private static readonly string PLAYER_ATTACK_ANIMATION_NAME = "Player_1_attack";
    private static readonly string PLAYER_ROLLING_ANIMATION_NAME = "Player_1_roll";
    private static readonly string ROLLING_ANIMATOR_NAME = "Rolling";
    // Time in milliseconds.
    private static readonly float INTERVAL_FOR_POSITION_CHECK = 400;

    /// <summary>Gets the player sword.</summary>
    /// <value>The player sword.</value>
    public GameObject PlayerSword { get { return m_playerSword; } }

    /// <summary>Gets or sets the positions as a list of vectors which is used by the multiplayer.</summary>
    /// <value>The positions as a list of vectors.</value>
    public IList<Vector2> Positions { get; set; }

    /// <summary>Gets the revive position.</summary>
    /// <value>The revive position.</value>
    public Vector2 RevivePosition { get; private set; }

    /// <summary>Gets the player index which is used by the multiplayer to differentiate between the two players.</summary>
    /// <value>The player index.</value>
    public int Index
    {
        get
        {
            return m_playerIndex;
        }
    }

    /// <summary>Awakes this instance and sets all resources that need to be acquired.</summary>
    protected override void Awake()
    {
        base.Awake();
        m_nonRollingColliderSize = m_collider.size;
        m_rollingColliderSize = m_nonRollingColliderSize / 2;

        Positions = new List<Vector2>();
        if (m_playerPositionsTransform != null)
        {
            foreach (Transform positionsTransform in m_playerPositionsTransform)
            {
                Positions.Add(positionsTransform.position);
            }
        }
        m_swordRenderer = PlayerSword.GetComponent<SpriteRenderer>();
    }

    /// <summary>Starts this instance by acquiring resources that are now available.</summary>
    protected override void Start()
    {
        base.Start();
        // Registration of player on start for safety reasons.
        m_activeGame = Game.Current;
        m_activeGame.RegisterPlayer(gameObject);
        // Times 1000 to get the duration in milliseconds.
        m_attackDuration = Toolkit.GetAnimationLength(m_animator, PLAYER_ATTACK_ANIMATION_NAME) * 1000;
        // Times 1000 to get the duration in milliseconds.
        m_rollingDuration = Toolkit.GetAnimationLength(m_animator, PLAYER_ROLLING_ANIMATION_NAME) * 1000;
        m_stopwatchRevive.Start();
    }

    /// <summary>Updates this instance by executing necessary steps for the revive position, rolling and attacking.</summary>
    protected override void Update()
    {
        base.Update();
        UpdateRevivePosition();
        UpdateRolling();
        UpdateAttacking();
    }

    // This method updates the revive position of the player. The revive position has to be stable. 
    // A position is stable when the player continuously stays on ground for INTERVAL_FOR_POSITION_CHECK
    // milliseconds.
    private void UpdateRevivePosition()
    {
        if (!m_isGrounded)
        {
            m_hasStablePosition = false;
        }
        else if (!m_hasStablePosition || m_stopwatchRevive.ElapsedMilliseconds >= INTERVAL_FOR_POSITION_CHECK)
        {
            // As soon as the player hits ground again or the time between checks is up, this part is executed.

            if (m_hasStablePosition)
            {
                // Since the player was continiously grounded the last position is stable.
                RevivePosition = m_lastCheckedPosition;
            }
            m_lastCheckedPosition = gameObject.transform.position;
            m_hasStablePosition = true;
            m_stopwatchRevive.Restart();
        }
    }

    // Updates the rolling by setting the colider size and invincibility when rolling and we are in the invincible time window.
    private void UpdateRolling()
    {
        if (IsRolling)
        {
            var elapsedTime = m_stopwatchRolling.ElapsedMilliseconds;

            // Since to the ground is not slippery, we need to reapply the velocity.
            Vector2 manipulatedVelocity = m_rigidbody2D.velocity;
            manipulatedVelocity.x = m_rollingMovementX;
            m_rigidbody2D.velocity = manipulatedVelocity;

            float currentAnimationLengthPercentage = elapsedTime / m_rollingDuration;
            bool playerIsCurrentlyInvincible = ROLLING_INVINCIBILITY_TIME_SPAN_START <= currentAnimationLengthPercentage 
                && currentAnimationLengthPercentage <= ROLLING_INVINCIBILITY_TIME_SPAN_END;

            // Adjust the invincibility and collider size according to the invincibility window.
            if (playerIsCurrentlyInvincible)
            {
                m_entityHealth.Invincible = true;
                m_collider.size = m_rollingColliderSize;
                m_activeGame.DisableEntityCollision(gameObject);
            }
            else
            {
                m_entityHealth.Invincible = false;
                m_collider.size = m_nonRollingColliderSize;
                m_activeGame.EnableEntityCollision(gameObject);
            }

            if (elapsedTime > m_rollingDuration)
            {
                IsRolling = false;
                m_stopwatchRolling.Reset();
                m_animator.SetBool(ROLLING_ANIMATOR_NAME, IsRolling);
            }
        }
    }

    // Stops the attacking if the animation is over.
    private void UpdateAttacking()
    {
        // Set the player as not attacking when the time that the attack animation needs is over.
        // Set the Animator variable as well.
        if (IsAttacking)
        {
            if (m_stopwatchAttack.ElapsedMilliseconds > m_attackDuration)
            {
                IsAttacking = false;
                m_stopwatchAttack.Reset();
                m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], IsAttacking);
            }
        }
    }

    /// <summary>Flips the entity.</summary>
    protected override void FlipEntity()
    {
        base.FlipEntity();
        // Flip the layer of the sword.
        ChangeOrderInLayer();
    }

    /// <summary>Triggers all necessary actions when the player dies by telling the active game that the player died.</summary>
    protected override void Die()
    {
        base.Die();
        m_entityHealth.Die();
        m_activeGame.HandleDeath(gameObject);
    }

    /// <summary>Resets the player animations.</summary>
    public override void ResetEntityAnimations()
    {
        base.ResetEntityAnimations();
        m_animator.SetBool(ROLLING_ANIMATOR_NAME, false);
        IsRolling = false;
        m_stopwatchRolling.Reset();
        m_collider.size = m_nonRollingColliderSize;
        IsAttacking = false;
        m_stopwatchAttack.Reset();
    }

    /// <summary>Sets the position of the player by an index. Used by the multiplayer to set the spawn positions.</summary>
    /// <param name="index">The index.</param>
    public void SetPosition(int index)
    {
        Vector2 position = Positions[index];
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
    }

    /// <summary>Sets the position of the player when reviving.</summary>
    /// <param name="revivePosition">The revive position.</param>
    public void SetPositionForRevive(Vector2 revivePosition)
    {
        RevivePosition = revivePosition;
        gameObject.transform.position = new Vector3(revivePosition.x, revivePosition.y, gameObject.transform.position.z);
    }

    void OnRoll(InputValue value)
    {
        if (!IsInputLocked && !IsAttacking && !IsRolling && m_isGrounded)
        {
            m_animator.SetBool(ROLLING_ANIMATOR_NAME, true);
            m_rollingMovementX = m_rigidbody2D.velocity.x;
            IsRolling = true;
            m_stopwatchRolling.Start();
        }
    }

    /// <summary>Called when the pause game button is pressed. It causes the game to stop.</summary>
    public void OnPauseGame()
    {
        if (!IsInputLocked)
        {
            Game.Pause();
        }
    }

    /// <summary> Changes the weapon of the entity to alternate between these two states:
    ///     Weapon rendered before player, weapon rendered behind player.</summary>
    public void ChangeOrderInLayer()
    {
        m_swordRenderer.sortingOrder *= -1;
    }
    
    void OnAttack(InputValue value)
    {
        if (!IsInputLocked && !IsRolling)
        {
            if (!IsAttacking || m_stopwatchAttack.ElapsedMilliseconds > m_attackDuration)
            {
                IsAttacking = true;
                DetermineAttackingParameter(m_attackDirection);
                m_animator.SetBool(ATTACK_ANIMATOR_PARAMETER_NAMES[m_currentAttackingDirection], IsAttacking);
                m_stopwatchAttack.Start();
            }
        }
    }

    void OnAttackDirection(InputValue value)
    {
        if (!IsInputLocked)
        {
            m_attackDirection = value.Get<float>();
        }
    }

    void OnMovement(InputValue value)
    {
        if (!IsInputLocked && !IsRolling)
        {
            // Controls.
            float moveX = value.Get<float>();

            if (Math.Abs(moveX) < CONTROLLER_DEADZONE)
            {
                moveX = 0.0f;
            }

            // Animation.
            m_animator.SetFloat(SPEED_ANIMATOR_PARAMETER_NAME, Mathf.Abs(moveX));

            // Player direction.
            if (moveX < 0.0f && m_isFacingRight)
            {
                FlipEntity();
            }
            else if (moveX > 0.0f && !m_isFacingRight)
            {
                FlipEntity();
            }

            // Physics.
            m_rigidbody2D.velocity = new Vector2((float)Math.Round(moveX) * m_moveSpeed, m_rigidbody2D.velocity.y);
        }
    }

    void OnRecord(InputValue value)
    {
        Recorder.Instance.Record();
    }

    // This method determines the attack direction.
    private void DetermineAttackingParameter(float attackDirectionAxisValue)
    {
        if (attackDirectionAxisValue > CONTROLLER_DEADZONE)
        {
            m_currentAttackingDirection = 0;
        }
        else if (attackDirectionAxisValue > -CONTROLLER_DEADZONE)
        {
            m_currentAttackingDirection = 1;
        }
        else 
        {
            m_currentAttackingDirection = 2;
        }
    }

    void OnDestroy()
    {
        m_activeGame.UnregisterPlayer(gameObject);
    }
}

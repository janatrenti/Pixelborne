/// <summary>Is implemented by enemies in order
/// to be compatible with the <see cref="AttackPatternExecutor"/>.</summary>
public interface IEnemyActions 
{
    /// <summary>Determines whether [is player in attack range].</summary>
    /// <returns>
    ///   <c>true</c> if [is player in attack range]; otherwise, <c>false</c>.</returns>
    bool IsPlayerInAttackRange();
    /// <summary>Determines whether [is player in sight range].</summary>
    /// <returns>
    ///   <c>true</c> if [is player in sight range]; otherwise, <c>false</c>.</returns>
    bool IsPlayerInSightRange();
    /// <summary>Determines whether [is enemy on the ground].</summary>
    /// <returns>
    ///   <c>true</c> if [is enemy on the ground]; otherwise, <c>false</c>.</returns>
    bool IsEnemyOnGround();

    /// <summary>Gets the duration of the down attack.</summary>
    /// <returns>The down attack duration as a float.</returns>
    float GetAttackDownDuration();
    /// <summary>Gets the duration of the middle attack.</summary>
    /// <returns>The middle attack duration as a float.</returns>
    float GetAttackMiddleDuration();
    /// <summary>Gets the duration of the up attack.</summary>
    /// <returns>The up attack duration as a float.</returns>
    float GetAttackUpDuration();

    /// <summary>Starts the down attack.</summary>
    void AttackDown();
    /// <summary>Starts the middle attack.</summary>
    void AttackMiddle();
    /// <summary>Starts the up attack.</summary>
    void AttackUp();

    /// <summary>Starts following the player.</summary>
    void StartFollowPlayer();
    /// <summary>Stops following the player.</summary>
    void StopFollowPlayer();

    /// <summary>Executes a jump.</summary>
    void Jump();

    /// <summary>Starts the automatic jumping.</summary>
    void StartAutoJumping();
    /// <summary>Stops the automatic jumping.</summary>
    void StopAutoJumping();
}

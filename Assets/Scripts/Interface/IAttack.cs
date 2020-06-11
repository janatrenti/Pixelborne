
/// <summary>Defines all necessary methods to determine if an entity got hit
///     and what damage the hit deals in the OnTriggerEnter2D-methods.</summary>
public interface IAttack
{
    /// <summary>Determines whether [is facing right].</summary>
    /// <returns>
    ///   <c>true</c> if [is facing right]; otherwise, <c>false</c>.</returns>
    bool IsFacingRight();
    /// <summary>Gets the attack damage.</summary>
    /// <returns>The attack damage.</returns>
    int GetAttackDamage();
    /// <summary>Gets the attack direction.</summary>
    /// <returns>The attack direction: Up = 0, Middle = 1, Down = 2.</returns>
    int GetAttackDirection();
}

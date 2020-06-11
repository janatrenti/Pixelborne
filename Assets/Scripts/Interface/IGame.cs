using UnityEngine;

/// <summary>Is implemented by Singleplayer and Multiplayer class and defines the common methods.
///     It is important to note that the game-instances <see cref="Singleplayer"/> and <see cref="Multiplayer"/>
///     work closely together with the camera.
///     When a player dies it tells the active game that it dies. This initiates a fade out in the camera. 
///     When the camera finished the fade out it notifies the game and the game can take further actions 
///     e.g. changing the multiplayer stage and fading in again.
///     Fading in has the same communication structure between the camera and the game.</summary>
public interface IGame
{
    /// <summary>Gets the winner.</summary>
    /// <returns>The winner as a string.</returns>
    string GetWinner();

    /// <summary>Registers the player to the game.</summary>
    /// <param name="player">The player.</param>
    void RegisterPlayer(GameObject player);
    /// <summary>Unregisters the player from the game.</summary>
    /// <param name="player">The player.</param>
    void UnregisterPlayer(GameObject player);
    /// <summary>Locks the player input.</summary>
    /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
    void LockPlayerInput(bool isLocked);
    /// <summary>Handles the death of an entity.</summary>
    /// <param name="entity">The entity.</param>
    void HandleDeath(GameObject entity);
    /// <summary>Prepares the stage.</summary>
    void PrepareStage();
    /// <summary>Disables the entity collision.</summary>
    /// <param name="callingEntity">The calling entity.</param>
    void DisableEntityCollision(GameObject callingEntity);
    /// <summary>Enables the entity collision.</summary>
    /// <param name="callingEntity">The calling entity.</param>
    void EnableEntityCollision(GameObject callingEntity);
    /// <summary>Swaps the sprite symbol in the hud.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    void SwapHudSymbol(GameObject gameObject, Sprite sprite);
}

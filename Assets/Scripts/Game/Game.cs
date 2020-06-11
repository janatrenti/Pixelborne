using UnityEngine;

/// <summary>Provides basic global game functionality and holds a reference to the currently running <see cref="IGame"/>.
///     It has only static methods and attributes.</summary>
public class Game : ScriptableObject
{
    /// <summary>Gets or sets the currently active game instance.</summary>
    /// <value>The currently active game.</value>
    public static IGame Current { get; set; }
    /// <summary>Gets or sets the game mode.</summary>
    /// <value>The game mode.</value>
    public static GameMode Mode { get; set; }

    /// <summary>Pauses the game.</summary>
    public static void Pause()
    {
        SceneChanger.LoadPauseMenuAdditive();
    }

    /// <summary>Finishes the game and changes to the winning screen.</summary>
    public static void Finish()
    {
        if (Mode == GameMode.Singleplayer)
        {
            SceneChanger.SetMainMenuAsActiveScene();
        }
        else
        {
            SceneChanger.SetWinningScreenAsActiveScene();
        }
    }

    /// <summary>Freezes the game.</summary>
    public static void Freeze()
    {
        Time.timeScale = 0;
        Current.LockPlayerInput(true);
    }

    /// <summary>Unfreezes the game.</summary>
    public static void Unfreeze()
    {
        Time.timeScale = 1;
        Current.LockPlayerInput(false);
    }

    /// <summary>Swaps the sprite symbol in the hud.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    public static void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        Current.SwapHudSymbol(gameObject, sprite);
    }
}

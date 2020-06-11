using UnityEngine;

/// <summary>Handles the behaviour of the buttons in the main menu.</summary>
public class MainMenu : MonoBehaviour
{
    /// <summary>Starts the singleplayer.</summary>
    public void StartSingleplayer()
    {
        Singleplayer.Instance.Go();
    }

    /// <summary>Starts the multiplayer.</summary>
    public void StartMultiplayer()
    {
        Multiplayer.Instance.Go();
    }

    /// <summary>Quits the game.</summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}

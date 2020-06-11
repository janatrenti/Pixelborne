using UnityEngine;

/// <summary>Handles the behaviour of the buttons in the pause menu.</summary>
public class PauseMenu : MonoBehaviour
{
    void Start()
    {
        Game.Freeze();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }

    /// <summary>Resumes this instance.</summary>
    public void Resume()
    {
        SceneChanger.UnloadPauseMenuAdditive();
        Game.Unfreeze();
    }

    /// <summary>Opens the main menu.</summary>
    public void OpenMainMenu()
    {
        Singleplayer.Instance.ResetGame();
        SceneChanger.SetMainMenuAsActiveScene();
        Game.Unfreeze();
    }
}

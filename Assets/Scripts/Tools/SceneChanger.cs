using UnityEngine.SceneManagement;

/// <summary>Provides static methods that implement various scene changing behaviour.</summary>
public class SceneChanger
{
    // The following indices represent the build index of the corresponding scene.
    // The index must be taken from the build settings.
    static readonly int MAIN_MENU_SCENE_INDEX = 0;
    static readonly int PAUSE_MENU_SCENE_INDEX = 1;
    static readonly int MULTIPLAYER_SCENE_INDEX = 2;
    static readonly int SELLING_SCREEN_SCENE_INDEX = 3;
    static readonly int WINNING_SCREEN_SCENE_INDEX = 4;
    static readonly int[] SINGLEPLAYER_STAGES_INDICES = { 5, 6, 7, 8 , 9, 10 };

    // This method checks if a scene has already been loaded to avoid loading scenes several times.
    private static bool IsSceneAlreadyLoaded(int index)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).buildIndex == index)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>Loads the singleplayer stage as active scene.</summary>
    /// <param name="index">The index.</param>
    /// <returns><c>true><c> if loading the singleplayer scene was successful; otherwise, <c>false<c></returns>
    public static bool LoadSingleplayerStageAsActiveScene(int index)
    {
        if (index > SINGLEPLAYER_STAGES_INDICES.Length - 1)
        {
            return false;
        }
        LoadSceneAsActiveScene(SINGLEPLAYER_STAGES_INDICES[index]);
        return true;
    }

    /// <summary>Sets the Multiplayer as active scene.</summary>
    public static void SetMultiplayerAsActiveScene()
    {
        LoadSceneAsActiveScene(MULTIPLAYER_SCENE_INDEX);
    }

    /// <summary>Sets the main menu as active scene.</summary>
    public static void SetMainMenuAsActiveScene()
    {
        LoadSceneAsActiveScene(MAIN_MENU_SCENE_INDEX);
    }

    /// <summary>Sets the winning screen as active scene.</summary>
    public static void SetWinningScreenAsActiveScene()
    {
        LoadSceneAsActiveScene(WINNING_SCREEN_SCENE_INDEX);
    }

    /// <summary>Loads the scene as active scene.</summary>
    /// <param name="index">The build index of the scene.</param>
    public static void LoadSceneAsActiveScene(int index)
    {
        if (!IsSceneAlreadyLoaded(index))
        {
            SceneManager.LoadScene(index);
        }
    }

    // This method loads a scene with the index additive to the current scene.
    /// <summary>Loads the scene additive.</summary>
    /// <param name="index">The index.</param>
    public static void LoadSceneAdditive(int index)
    {
        // Check if the scene has already been loaded additive.
        if (!IsSceneAlreadyLoaded(index))
        {
            SceneManager.LoadScene(index, LoadSceneMode.Additive);
        }
    }

    /// <summary>Loads the pause menu from the scene.</summary>
    public static void LoadPauseMenuAdditive()
    {
        LoadSceneAdditive(PAUSE_MENU_SCENE_INDEX);
    }

    /// <summary>Unloads the pause menu from the scene.</summary>
    public static void UnloadPauseMenuAdditive()
    {
        SceneManager.UnloadSceneAsync(PAUSE_MENU_SCENE_INDEX);
    }

    /// <summary>Loads the selling screen from scene.</summary>
    public static void LoadSellingScreenAdditive()
    {
        LoadSceneAdditive(SELLING_SCREEN_SCENE_INDEX);
    }

    /// <summary>Unloads the selling screen from the scene.</summary>
    public static void UnloadSellingScreenAdditive()
    {
        SceneManager.UnloadSceneAsync(SELLING_SCREEN_SCENE_INDEX);
    }
}

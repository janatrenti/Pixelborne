using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>Contains the Singleplayer game mode logic and implements the <see cref="IGame"/> interface
///     for the singleplayer mode. It is a singleton.</summary>
public class Singleplayer : ScriptableObject, IGame
{
    private HashSet<GameObject> m_entitiesThatRequestedDisableEntityCollision = new HashSet<GameObject>();
    private int m_currentStageIndex = m_START_STAGE_INDEX;
    private int m_enemyLayer;
    private PlayerActions m_playerMovement;
    private Vector2 m_playerRevivePosition;
    private static Singleplayer s_instance = null;

    private static readonly int m_START_STAGE_INDEX = 0;

    /// <summary>Gets or sets the camera.</summary>
    /// <value>The camera.</value>
    public CameraSingleplayer Camera { get; set; }
    /// <summary>Gets or sets the price to pay for a revive.</summary>
    /// <value>The price to pay.</value>
    public float PriceToPay { get; set; }
    /// <summary>Gets or sets the player.</summary>
    /// <value>The player.</value>
    public GameObject Player { get; set; } = null;
    /// <summary>Gets or sets the active enemies.</summary>
    /// <value>The active enemies.</value>
    public List<GameObject> ActiveEnemies { get; set; } = new List<GameObject>();

    /// <summary>Gets the instance.</summary>
    public static Singleplayer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            if (s_instance == null) 
            {
                s_instance = CreateInstance<Singleplayer>();
                s_instance.m_enemyLayer = LayerMask.NameToLayer("Enemy");
            }
            return s_instance;
        }
    }

    // This is for testing and debugging single stages quicker without having to start from the MainMenu.
    // TODO: Remove later.
    /// <summary>Sets the index of the debug current stage.</summary>
    /// <value>The index of the debug current stage.</value>
    public int DEBUG_currentStageIndex
    {
        set
        {
            m_currentStageIndex = value;
        }
    }

    private Singleplayer()
    {
        s_instance = this;
    }

    /// <summary>Starts the singleplayer.</summary>
    public void Go()
    {
        Game.Current = Instance;
        Game.Mode = GameMode.Singleplayer;

        SellingScreen.GetImportantFiles();

        PrepareStage();
    }

    /// <summary>Gets the winner.</summary>
    /// <returns>"You".</returns>
    public string GetWinner()
    {
        return $"You";
    }

    /// <summary>Registers the player.</summary>
    /// <param name="player">The player.</param>
    /// <exception cref="Exception">Error: Object \"{player.name}\" can not be registered. Player has already been assigned.</exception>
    public void RegisterPlayer(GameObject player)
    {
        if (Player == null)
        {
            Player = player;
            m_playerMovement = player.GetComponent<PlayerActions>();
            ImageManager.Instance.PlayerSpawnPosition = player.transform.position;
        }
        else
        {
            throw new Exception($"Error: Object \"{player.name}\" can not be registered. Player has already been assigned.");
        }
    }

    /// <summary>Unregisters the player.</summary>
    /// <param name="player">The player.</param>
    public void UnregisterPlayer(GameObject player)
    {
        Player = null;
    }

    /// <summary>Revives the player at the revive position.</summary>
    public void RevivePlayer()
    {
        m_playerMovement.SetPositionForRevive(m_playerRevivePosition);
        m_playerMovement.ResetEntityActions();
    }

    /// <summary>Locks the player and enemy input.</summary>
    /// <param name="isLocked">if set to <c>true</c> [is locked].</param>
    public void LockPlayerInput(bool isLocked)
    {
        m_playerMovement.IsInputLocked = isLocked;
        if (Player != null)
        {
            m_playerMovement.ResetEntityAnimations();
        }
        foreach (GameObject enemy in ActiveEnemies)
        {
            enemy.GetComponent<EnemyActions>().IsInputLocked = isLocked;
        }
    }

    /// <summary>Handles the death of a player. Enemies should not call this method. 
    ///     It will throw an exception.</summary>
    /// <param name="entity">The player game object.</param>
    /// <exception cref="ArgumentException">Expected player as argument but got: {entity}</exception>
    public void HandleDeath(GameObject entity)
    {
        if (entity == Player)
        {
            m_playerRevivePosition = m_playerMovement.RevivePosition;
            SceneChanger.LoadSellingScreenAdditive();
        }
        else 
        {
            throw new ArgumentException($"Expected player as argument but got: {entity}");
        }
    }

    /// <summary>Resets the game.</summary>
    public void ResetGame()
    {
        m_currentStageIndex = m_START_STAGE_INDEX;
    }

    /// <summary>Prepares the stage.</summary>
    public void PrepareStage()
    {
        bool isStageExistent = SceneChanger.LoadSingleplayerStageAsActiveScene(m_currentStageIndex);
        if (!isStageExistent)
        {
            Game.Finish();
            ResetGame();
            return;
        }

        // Activate DriveMusicManager.
        DriveMusicManager.Instance.Go();
    }

    /// <summary>Begins the stage.</summary>
    public void BeginStage()
    {
        Camera.FadeIn();
    }

    /// <summary>Ends the stage.</summary>
    public void EndStage()
    {
        m_currentStageIndex++;

        // If no player is registered it is a cutscene which needs no fading.
        if (Player != null)
        {
            LockPlayerInput(true);
            Camera.FadeOut();
        }
        else
        {
            PrepareStage();
        }
    }

    /// <summary>Is invoked when the camera faded out and prepares the stage.</summary>
    public void FadedOut()
    {
        PrepareStage();
    }

    /// <summary>Is invoked when the camera faded in and starts the stage by disabling the input lock.</summary>
    public void FadedIn()
    {
        LockPlayerInput(false);
    }

    /// <summary>Enables the collision between the player and enemy layer.</summary>
    /// <param name="callingEntity">The calling entity.</param>
    public void EnableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Remove(callingEntity);
        if (m_entitiesThatRequestedDisableEntityCollision.Count == 0)
        {
            Physics2D.IgnoreLayerCollision(Player.layer, m_enemyLayer, false);
        }
    }

    /// <summary>Disables the collision between the player and enemy layer.</summary>
    /// <param name="callingEntity">The calling entity.</param>
    public void DisableEntityCollision(GameObject callingEntity)
    {
        m_entitiesThatRequestedDisableEntityCollision.Add(callingEntity);
        Physics2D.IgnoreLayerCollision(Player.layer, m_enemyLayer, true);
    }

    /// <summary>Swaps the hud symbol.</summary>
    /// <param name="gameObject">The game object.</param>
    /// <param name="sprite">The sprite.</param>
    public void SwapHudSymbol(GameObject gameObject, Sprite sprite)
    {
        if (Camera != null)
        {
            Camera.SwapHudSymbol(gameObject, sprite);
        }
    }
}

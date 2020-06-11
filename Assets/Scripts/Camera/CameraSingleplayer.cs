using UnityEngine;

/// <summary>Controls the camera of the singleplayer scene.</summary>
public class CameraSingleplayer : GameCamera
{    
    void Start()
    {
        Singleplayer.Instance.Camera = this;
        // Position the fade image right in front of the camera.
        m_fadeImage.transform.position = gameObject.transform.position + new Vector3(0, 0, 1);
    }

    /// <summary>Updates this instance by setting the position to the player position.</summary>
    protected override void Update()
    {
        base.Update();

        if (Singleplayer.Instance.Player != null)
        {
            // Follow the player.
            gameObject.transform.position = new Vector3(Singleplayer.Instance.Player.transform.position.x,
                                                        Singleplayer.Instance.Player.transform.position.y,
                                                        gameObject.transform.position.z);
        }
    }

    /// <summary>Is called when the camera faded out and 
    ///     invokes the FadedOut-Methdod on the current <see cref="Singleplayer"/> instance.</summary>
    protected override void FadedOut()
    {
        Singleplayer.Instance.FadedOut();
    }

    /// <summary>Is called when the camera faded in and 
    ///     invokes the FadedIn-Methdod on the current <see cref="Singleplayer"/> instance.</summary>
    protected override void FadedIn()
    {
        Singleplayer.Instance.FadedIn();
    }
}

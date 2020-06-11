using UnityEngine;

/// <summary>Handles the volume of the default background music.</summary>
public class BackgroundMusic : MonoBehaviour
{
    private static AudioSource s_player;

    /// <summary>Sets the volume.</summary>
    /// <param name="value">The volume.</param>
    public static void SetVolume(float value)
    {
        if (s_player != null)
        {
            s_player.volume = value;
        }
    }

    void Start()
    {
        s_player = gameObject.GetComponent<AudioSource>();
        s_player.volume = SettingsContainer.Instance.BackgroundMusicVolume;
    }
}

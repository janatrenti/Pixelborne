using UnityEngine;

/// <summary>Contains the volume settings that is needed by the <see cref="BackgroundMusicVolumeSlider"/> 
///     and sets the background music volume. It is a Singleton.</summary>
public class SettingsContainer : ScriptableObject
{
    private static SettingsContainer s_instance = null;
    private float m_backgroundMusicVolume = 1.0f;

    /// <summary>Gets or sets the background music volume.</summary>
    /// <value>The background music volume.</value>
    public float BackgroundMusicVolume
    {
        get
        {
            return m_backgroundMusicVolume;
        }
        set
        {
            m_backgroundMusicVolume = value;
            BackgroundMusic.SetVolume(value);
        }
    }

    /// <summary>Gets the instance.</summary>
    /// <value>The instance.</value>
    public static SettingsContainer Instance
    {
        get
        {
            // A ScriptableObject should not be instanciated directly,
            // so we use CreateInstance instead.
            return s_instance == null ? CreateInstance<SettingsContainer>() : s_instance;
        }
        private set { }
    }

    private SettingsContainer()
    {
        s_instance = this;
    }
}

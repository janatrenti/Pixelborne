using UnityEngine;
using UnityEngine.UI;

/// <summary>Sets the value of the volume slider in the settings.
///     Gets this value from the <see cref="SettingsContainer"/>.</summary>
public class BackgroundMusicVolumeSlider : MonoBehaviour
{
    void Start()
    {
        Slider slider = gameObject.GetComponent<Slider>();
        // Set value of slider to value of volume.
        slider.value = SettingsContainer.Instance.BackgroundMusicVolume;
        slider.onValueChanged.AddListener(value => SettingsContainer.Instance.BackgroundMusicVolume = value);
    }
}

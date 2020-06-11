using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Highlights the volume slider in the settings of the menu screen.</summary>
public class SliderHighlightColor : MonoBehaviour
{
    [SerializeField]
    private Image m_sliderFillImage;

    private const float m_HIGHLIGHT = 0.8f;
    private const float m_UNHIGHLIGHT = 0.6f;

    // A slider in Unity can only set a highlight color when it is dragged or moved.
    // Since we only use the keyboard and gamepad for menu navigation we want it highlighted when it is selected.
    void Update()
    {
        // Check if this slider is selected.
        if (EventSystem.current.currentSelectedGameObject == gameObject)
        {
            // Highlight the slider when it is selected.
            if (m_sliderFillImage.color.r != m_HIGHLIGHT)
            {
                Color color = m_sliderFillImage.color;
                color.r = m_HIGHLIGHT;
                m_sliderFillImage.color = color;
            }
            
        }
        else if (m_sliderFillImage.color.r != m_UNHIGHLIGHT)
        {
            // Unhighlight the Slider when it is not selected.
            Color color = m_sliderFillImage.color;
            color.r = m_UNHIGHLIGHT;
            m_sliderFillImage.color = color;
        }
    }
}

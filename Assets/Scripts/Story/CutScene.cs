using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>Implements basic functionality of a cutscene.</summary>
public abstract class Cutscene : MonoBehaviour
{
    [SerializeField]
    protected float m_fadeTime = 3000;
    [SerializeField]
    protected Image m_backgroundImage;
    [SerializeField]
    protected TextMeshProUGUI m_story;

    protected enum CutSceneMode
    {
        FadeImage,
        DisplayText,
        AnimateImages,
        Done,
        Nothing
    }

    protected CutSceneMode m_mode;
    protected int m_storyPart = 0;
    protected int m_textPart = 0;
    protected Stopwatch m_stopwatch = new Stopwatch();

    protected virtual string[][] StoryHolder { get; set; }

    protected virtual void Start()
    {
        m_stopwatch.Start();
    }

    protected virtual void Update()
    {
        float elapsedTime = m_stopwatch.ElapsedMilliseconds * 1.0f;

        if (m_mode == CutSceneMode.FadeImage)
        {
            // Fade the colors darker.
            float percentage = elapsedTime / m_fadeTime;
            float colorValue = (1.0f - percentage) + 0.3f;
            m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);

            // Complete the fade when enough time has passed.
            if (elapsedTime >= m_fadeTime)
            {
                colorValue = 0.3f;
                m_backgroundImage.color = new Color(colorValue, colorValue, colorValue);
                m_story.gameObject.SetActive(true);
                m_mode = CutSceneMode.DisplayText;
                m_stopwatch.Restart();
            }
        }
        else if (m_mode == CutSceneMode.DisplayText)
        {
            m_story.text = StoryHolder[m_storyPart][m_textPart];
            if (elapsedTime >= m_fadeTime)
            {
                m_textPart++;

                if (m_textPart == StoryHolder[m_storyPart].Length)
                {
                    m_textPart = 0;
                    CutSceneMode nextMode = ChangeStoryPart();
                    m_mode = nextMode;
                    m_stopwatch.Restart();
                    return;
                }
                m_stopwatch.Restart();
            }
        }
    }

    protected abstract CutSceneMode ChangeStoryPart();
}

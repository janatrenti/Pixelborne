using UnityEngine;

/// <summary>Manages the displaying of images and text in the intro scene of the singleplayer mode.</summary>
public class IntroScene : Cutscene
{
    private string[] m_imageHolder =
    {
        "IntroImages/peaceful",
        "IntroImages/war",
        "IntroImages/castle_gates"
    };

    protected override string[][] StoryHolder { get; set; } =
    {
        new string[] { "Prologue\n\nDarkness" },
        new string[] {
            "Once upon a time, there was a peaceful kingdom, full of light and happiness.",
            "The people lived content lives under the rule of a just king.",
            "And everything was bright and colorful."
        },
        new string[] {
            "Until one day, darkness erupted.",
            "A dark energy claimed the land, and with it came darker creatures, ancient and full of malice.",
            "They burnt the towns. They slaughtered the people."
        },
        new string[] {
            "And eventually, they reached the castle gates.",
            "The kingdom was weak, and the gates could not be held.",
            "But a few brave knights remained, and they fought back with everything they had."
        }
    };

    protected override void Start()
    {
        m_story.text = StoryHolder[m_storyPart][m_textPart];
        m_mode = CutSceneMode.DisplayText;
        base.Start();
    }

    protected override CutSceneMode ChangeStoryPart()
    {
        m_storyPart++;

        if (m_storyPart == StoryHolder.Length)
        {
            Singleplayer.Instance.EndStage();
            return CutSceneMode.Nothing;
        }

        // Disable the text.
        m_story.gameObject.SetActive(false);
        m_story.text = string.Empty;

        // Change the background image.
        // m_storyPart - 1 because the StoryHolder's first element ist the prologue screen that does not have an image.
        // So the images are a  litte shifted.
        m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_imageHolder[m_storyPart - 1]);
        m_backgroundImage.color = Color.white;
        return CutSceneMode.FadeImage;
    }
}

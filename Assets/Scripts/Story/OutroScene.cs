using UnityEngine;

/// <summary>Manages the displaying of images and text in the outro scene of the singleplayer mode.</summary>
public class OutroScene : Cutscene
{
    [SerializeField]
    private float m_animationTime = 500;

    private int m_animationPart = 0;
    private string[][] m_imageHolder =
    {
        new string[] { "OutroImages/dark_crown_destroyed" },
        new string[] { "OutroImages/possessed_land" },
        new string[] {
            "OutroImages/possessed_land",
            "OutroImages/retreating_shadows",
            "OutroImages/shadows_almost_gone",
            "OutroImages/free_once_more"
        }
    };

    protected override string[][] StoryHolder { get; set; } =
    {
        new string[] {
            "And as the crown splintered, a terrible screech rang out as its Dark Crystal cracked and dulled.",
            "A cold wind filled the world, a whisper of hate and ancient darkness.",
            "And then... silence."
        },
        new string[] {
            "As one, the demons all over the land froze.",
            "As one, they started wailing and screeching and turning on one another.",
            "And then, they disappeared as if sucked through a crack in the world, leaving not a trace."
        },
        new string[] {
            "And what remained was a beautiful kingdom, battered, but unbroken.",
            "What remained were triumphant people under the wise rule of a just queen.",
            "What remained was light and warmth and hope."
        },
        new string[] { THE_END },
        new string[] { $"{THE_END}\n\nThank you for playing Pixelborne.\nPress space to return to the main menu." }
    };

    private static readonly string THE_END = "THE END";

    protected override void Start()
    {
        m_story.text = StoryHolder[m_storyPart][m_textPart];
        m_story.gameObject.SetActive(false);
        m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_imageHolder[m_storyPart][m_animationPart]);
        m_mode = CutSceneMode.AnimateImages;
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (m_mode == CutSceneMode.AnimateImages)
        {
            if (m_stopwatch.ElapsedMilliseconds >= m_animationTime)
            {
                m_animationPart++;
                if (m_animationPart == m_imageHolder[m_storyPart].Length)
                {
                    m_animationPart = 0;
                    m_mode = CutSceneMode.FadeImage;
                    return;
                }
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_imageHolder[m_storyPart][m_animationPart]);
                m_stopwatch.Restart();
            }
        }
        else if (m_mode == CutSceneMode.Done && Input.GetKeyDown("space"))
        {
            Singleplayer.Instance.EndStage();
        }
    }

    protected override CutSceneMode ChangeStoryPart()
    {
        m_storyPart++;

        if (m_storyPart == StoryHolder.Length)
        {
            return CutSceneMode.Done;
        }
        else if (m_storyPart >= StoryHolder.Length - 2)
        {
            m_backgroundImage.color = Color.black;
            return CutSceneMode.DisplayText;
        }

        // Disable the text.
        m_story.gameObject.SetActive(false);
        m_story.text = string.Empty;

        // Change the background image.
        m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_imageHolder[m_storyPart][m_animationPart]);
        m_backgroundImage.color = Color.white;
        return CutSceneMode.AnimateImages;
    }
}

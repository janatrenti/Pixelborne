using UnityEngine;
using UnityEngine.UI;

/// <summary>Manages the displaying of the dialog in stage 4 of the singleplayer mode.</summary>
public class DialogueStage4 : Dialogue
{
    [SerializeField]
    private int m_animationDuration = 500;
    [SerializeField]
    private int m_flashDuration = 100;
    [SerializeField]
    private SpriteRenderer m_filterImage;
    [SerializeField]
    private Image m_backgroundImage;
    [SerializeField]
    private GameObject m_demonKing;
    [SerializeField]
    private GameObject m_endboss;
    [SerializeField]
    private GameObject m_princess;
    [SerializeField]
    private GameObject m_king;

    private enum Mode
    {
        NotStarted,
        Displaying,
        WaitingForTrigger,
        Flashing,
        Animating
    }

    private int m_animationPart;
    private Mode m_mode;
    private string m_activeCharacter;
    private string[] m_dialogueText;
    private string[] m_animationImages;
    private string[] m_animationImages0 = { "OutroImages/spilled_stones_blood_3",
                                            "OutroImages/awakening" };
    private string[] m_animationImages1 = { "OutroImages/dark_crown",
                                            "OutroImages/hit_animation",
                                            "OutroImages/dark_crown_destroyed" };

    protected override string[][] DialogueHolder { get; set; } =
    {
        new string[] { "Father!" },
        new string[] {
            "My child! You are here!",
            $"Knight {DEFAULT_KNIGHT}! My thanks as both father and king.",
            "My daughter and my kingdom are safe once more.",
            "...",
            "Is that the Dark King's Crown you're carrying with you?"
        },
        new string[] { "Yes, Father. We found it in his lair." },
        new string[] {
            $"Give it to me, Knight {DEFAULT_KNIGHT}!",
            "And give me the shards of Dark Crystal you recovered."
        },
        new string[] {
            "Marvelous!",
            "Let it be known that from this day, our kingdom shall bow to no demon!"
        },
        new string[] { "Let it be known that today will mark the beginning of a new era!" },
        new string[] { "Ah..!" },
        new string[] { "Father! Are you well?" },
        new string[] { "I... huh... just... dizzy... huh... huh..." },
        new string[] { "Aaaaargh!" },
        new string[] { "Father!" },
        new string[] { "F-Father?" },
        new string[] {
            "Haha... your father is gone, child.",
            "His body is mine now."
        },
        new string[] { "No..!" },
        new string[] {
            "Don't worry, child. I will have plenty of time for you later.",
            "But first..!"
        },
        new string[] { $"First, maggot {DEFAULT_KNIGHT}, it is time for you to die!" },
        new string[] { "O Father..." },
        new string[] { "Please forgive me..." },
        new string[] { "I love you, Father..." },
        new string[] { "Hah... Hahaha..." },
        new string[] { "What..?" },
        new string[] { "Hahahaha!" },
        new string[] {
            "Who..?",
            "...",
            "No..! The stones..!"
        },
        new string[] { "At last..!" },
        new string[] { "NO!" },
        new string[] { "At last, I'm free once more!" },
        new string[] { "The Dark King!" },
        new string[] { "Indeed. The only king left standing, it seems." },
        new string[] {
            "I curse you, Dark King!",
            "You will die for what you have done!"
        },
        new string[] {
            "Really? Hahaha... You are mistaken, I fear.",
            "YOU will die.",
            "You will all die.",
            "Your whole kingdom will perish!",
            "And then, it shall be mine!"
        },
        new string[] { "Not this day, Dark King. And not on any other.",
            "Let it be known that from this day, our kingdom shall bow to no demon.",
            $"Knight {DEFAULT_KNIGHT}!",
            "Destroy his crown, so that he shall stay in Hell, where he belongs, for all eternity!"
        }
    };
    
    private string[] m_characterHolder =
    {
        PRINCESS,
        KING,
        PRINCESS,
        KING,
        KING,
        KING,
        KING,
        PRINCESS,
        KING,
        KING,
        PRINCESS,
        PRINCESS,
        KING,
        PRINCESS,
        KING,
        KING,
        PRINCESS,
        KING,
        PRINCESS,
        UNKNOWN,
        PRINCESS,
        UNKNOWN,
        PRINCESS,
        UNKNOWN,
        PRINCESS,
        DARK_KING,
        PRINCESS,
        DARK_KING,
        DARK_KING,
        DARK_KING,
        PRINCESS
    };

    private static readonly string PRINCESS = "Princess";
    private static readonly string KING = "King";
    private static readonly string DARK_KING = "Dark King";
    private static readonly string UNKNOWN = "???";

    protected override void Start()
    {
        m_demonKing.SetActive(false);
        Singleplayer.Instance.ActiveEnemies.Remove(m_demonKing);
        m_endboss.SetActive(false);
        Singleplayer.Instance.ActiveEnemies.Remove(m_endboss);

        Singleplayer.Instance.ActiveEnemies.Remove(m_princess);
        m_princess.GetComponent<EnemyActions>().StartFollowPlayer();
        m_princess.GetComponent<EnemyActions>().IsInputLocked = false;

        InsertName();
        m_dialogueText = DialogueHolder[0];
        m_activeCharacter = m_characterHolder[0];
        m_mode = Mode.NotStarted;
        m_dialogueBackground.color = Color.black;
        SetDialogueVisibility(false);
    }

    void Update()
    {
        bool enemiesKilled = Singleplayer.Instance.ActiveEnemies.Count == 0;

        switch (m_mode)
        {
            case Mode.NotStarted:
                if (HasPlayerProgressed && enemiesKilled  )
                {
                    Singleplayer.Instance.LockPlayerInput(true);
                    m_mode = Mode.Displaying;
                    SetDialogueVisibility(true);
                    m_stopwatch.Restart();
                }
                break;

            case Mode.WaitingForTrigger:
                if (HasPlayerProgressed && enemiesKilled)
                {
                    Singleplayer.Instance.LockPlayerInput(true);
                    SetDialogueVisibility(true);
                    m_mode = Mode.Displaying;
                    ChangePart();
                }
                break;

            case Mode.Displaying:
                m_dialogue.text = m_dialogueText[m_textPart];
                m_nameTag.text = m_activeCharacter;

                if (m_stopwatch.ElapsedMilliseconds >= m_displayTime || Input.GetKeyDown("space"))
                {
                     m_textPart++;
                    if (m_textPart == m_dialogueText.Length)
                    {
                        m_textPart = 0;
                        ChangePart();
                    }
                    m_stopwatch.Restart();
                }
                break;

            case Mode.Flashing:
                if (m_stopwatch.ElapsedMilliseconds >= m_flashDuration)
                {
                    m_filterImage.enabled = false;
                    m_mode = Mode.Displaying;
                    SetDialogueVisibility(true);
                    ChangePart();
                }
                break;

            case Mode.Animating:
                if (m_stopwatch.ElapsedMilliseconds >= m_animationDuration || m_animationPart < 0)
                {
                    m_animationPart++;
                    if (m_animationPart == m_animationImages.Length)
                    {
                        m_animationPart = 0;
                        ChangePart();
                        return;
                    }
                    m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_animationImages[m_animationPart]);
                    m_stopwatch.Restart();
                }
                break;
        }
    }

    private void FlashViolet()
    {
        SetDialogueVisibility(false);
        m_filterImage.enabled = true;
        m_mode = Mode.Flashing;
        m_stopwatch.Restart();
    }

    private void Animate()
    {
        SetDialogueVisibility(false);
        m_mode = Mode.Animating;
        m_animationPart = -1;
        m_backgroundImage.color = Color.white;
        m_stopwatch.Restart();
    }

    private void ChangePart()
    {
        // Due to some animations, fights etc between dialogue parts the index that refers
        // to the correct dialoguePart gets shifted more and more.
        m_dialoguePart++;
        switch (m_dialoguePart)
        {
            case 1:
            case 2:
            case 3:
                m_dialogueText = DialogueHolder[m_dialoguePart];
                m_activeCharacter = m_characterHolder[m_dialoguePart];
                break;

            case 4:
                SetDialogueVisibility(false);
                HasPlayerProgressed = false;
                m_mode = Mode.WaitingForTrigger;            //player supposed to walk to king
                Singleplayer.Instance.LockPlayerInput(false);
                break;

            case 5:
                m_dialogueText = DialogueHolder[4];
                m_activeCharacter = m_characterHolder[4];
                break;

            case 6:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("OutroImages/taking_the_crown");
                m_backgroundImage.color = Color.white;
                m_dialogueBackground.color = Color.clear;
                m_dialogueText = DialogueHolder[5];
                m_activeCharacter = m_characterHolder[5];
                break;

            case 7:
                m_backgroundImage.color = Color.clear;
                FlashViolet();
                break;

            case 8:
                m_dialogueBackground.color = Color.black;
                m_dialogueText = DialogueHolder[6];
                m_activeCharacter = m_characterHolder[6];
                break;
            case 9:
            case 10:
                m_dialogueText = DialogueHolder[m_dialoguePart - 2];
                m_activeCharacter = m_characterHolder[m_dialoguePart - 2];
                break;

            case 11:
                m_filterImage.enabled = true;
                m_dialogueText = DialogueHolder[9];
                m_activeCharacter = m_characterHolder[9];
                break;

            case 12:
                m_dialogueText = DialogueHolder[10];
                m_activeCharacter = m_characterHolder[10];
                break;

            case 13:
                m_filterImage.enabled = false;
                m_king.SetActive(false);
                m_demonKing.SetActive(true);
                m_demonKing.GetComponent<EnemyActions>().IsInputLocked = true;
                Singleplayer.Instance.ActiveEnemies.Add(m_demonKing);
                m_dialogueText = DialogueHolder[11];
                m_activeCharacter = m_characterHolder[11];
                break;

            case 14:
            case 15:
            case 16:
            case 17:
                m_dialogueText = DialogueHolder[m_dialoguePart - 2];
                m_activeCharacter = m_characterHolder[m_dialoguePart - 2];
                break;

            case 18:  // Bossfight #1 begins here.
                SetDialogueVisibility(false);
                m_princess.GetComponent<EnemyActions>().StopFollowPlayer();
                Singleplayer.Instance.LockPlayerInput(false);
                m_demonKing.GetComponent<EnemyActions>().IsInputLocked = false;
                // Player is supposed to kill the king.
                m_mode = Mode.WaitingForTrigger;
                break;

            case 19:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("OutroImages/spilled_stones");
                m_backgroundImage.color = Color.white;
                m_dialogueBackground.color = Color.clear;
                m_dialogueText = DialogueHolder[16];
                m_activeCharacter = m_characterHolder[16];
                break;

            case 20:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("OutroImages/spilled_stones_blood_1");
                m_dialogueBackground.color = Color.clear;
                m_dialogueText = DialogueHolder[17];
                m_activeCharacter = m_characterHolder[17];
                break;

            case 21:
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>("OutroImages/spilled_stones_blood_2");
                m_dialogueBackground.color = Color.clear;
                m_dialogueText = DialogueHolder[18];
                m_activeCharacter = m_characterHolder[18];
                break;

            case 22:
                m_animationImages = m_animationImages0;
                Animate();
                break;

            case 23:
                m_mode = Mode.Displaying;
                SetDialogueVisibility(true);
                m_dialogueBackground.color = Color.black;
                m_dialogueText = DialogueHolder[19];
                m_activeCharacter = m_characterHolder[19];
                break;

            case 24:
                m_backgroundImage.color = Color.clear;
                m_dialogueBackground.color = Color.black;
                m_dialogueText = DialogueHolder[20];
                m_activeCharacter = m_characterHolder[20];
                break;

            case 25:
            case 26:
            case 27:
            case 28:
                m_dialogueText = DialogueHolder[m_dialoguePart - 4];
                m_activeCharacter = m_characterHolder[m_dialoguePart - 4];
                break;

            case 29:
                FlashViolet();
                break;

            case 30:
                Singleplayer.Instance.ActiveEnemies.Remove(m_demonKing);
                m_demonKing.SetActive(false);
                m_endboss.SetActive(true);
                Singleplayer.Instance.ActiveEnemies.Add(m_endboss);
                Singleplayer.Instance.LockPlayerInput(true);
                m_dialogueText = DialogueHolder[25];
                m_activeCharacter = m_characterHolder[25];
                break;

            case 31:
            case 32:
            case 33:
            case 34:
                m_dialogueText = DialogueHolder[m_dialoguePart - 5];
                m_activeCharacter = m_characterHolder[m_dialoguePart - 5];
                break;

            case 35: // Bossfight #2 begins here.
                SetDialogueVisibility(false);
                Singleplayer.Instance.LockPlayerInput(false);
                // Player is supposed to kill the dark king.
                m_mode = Mode.WaitingForTrigger;            
                break;

            case 36:
                m_dialogueText = DialogueHolder[30];
                m_activeCharacter = m_characterHolder[30];
                break;

            case 37:
                SetDialogueVisibility(false);
                HasPlayerProgressed = false;
                Singleplayer.Instance.DisableEntityCollision(Singleplayer.Instance.Player);
                Singleplayer.Instance.LockPlayerInput(false);
                // Player is supposed to walk to the crown.
                m_mode = Mode.WaitingForTrigger;            
                break;

            case 38:
                Singleplayer.Instance.LockPlayerInput(true);
                m_animationImages = m_animationImages1;
                m_backgroundImage.overrideSprite = Resources.Load<Sprite>(m_animationImages[0]);
                Animate();
                break;

            case 39:
                Singleplayer.Instance.EndStage();
                break;
        }

        m_stopwatch.Restart();
    }
}

using UnityEngine;

/// <summary>Manages the displaying of the dialog in stage 3 of the singleplayer mode.</summary>
public class DialogueStage3 : Dialogue
{
    private enum Mode
    {
        Displaying,
        WaitingForTrigger
    }

    private Mode m_mode = Mode.WaitingForTrigger;

    protected override string[][] DialogueHolder { get; set; } =
    { 
        new string[] {
            $"Knight {DEFAULT_KNIGHT}! Is that you?",
            "You found me! Thank goodness.",
            "And I started to fear those vile demons might succeed.",
            "You must know, they believe the royal blood holds ancient power.",
            "Power they wanted to use to summon their Dark King from his -",
            "Hold on! Is that the Dark King's crown you have there?",
            "What a relief. We shall take it with us, so that they may never be able to use it.",
            "Come now, let us return to the castle at once!"
        }
    };

    protected override void Start()
    {
        base.Start();
        m_nameTag.text = "Princess";
    }

    void Update()
    {
        bool enemiesKilled = Singleplayer.Instance.ActiveEnemies.Count == 0;

        switch (m_mode)
        {
            case Mode.WaitingForTrigger:
                if (HasPlayerProgressed && enemiesKilled)
                {
                    Singleplayer.Instance.LockPlayerInput(true);
                    m_mode = Mode.Displaying;
                    SetDialogueVisibility(true);
                    m_stopwatch.Start();
                }
                break;

            case Mode.Displaying:
                m_dialogue.text = DialogueHolder[m_dialoguePart][m_textPart];

                if (m_stopwatch.ElapsedMilliseconds >= m_displayTime || Input.GetKeyDown("space"))
                {
                    m_textPart++;

                    if (m_textPart == DialogueHolder[m_dialoguePart].Length)
                    {
                        m_stopwatch.Stop();
                        Singleplayer.Instance.EndStage();
                        Destroy(gameObject);
                        return;
                    }
                    m_stopwatch.Restart();
                }
                break;
        }
    }
}

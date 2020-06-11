using UnityEngine;

/// <summary>Manages the displaying of the dialog in stage 1 of the singleplayer mode.</summary>
public class DialogueStage1 : Dialogue
{
    private enum Mode
    {
        Displaying,
        WaitingForTrigger
    }

    private Mode m_mode = Mode.WaitingForTrigger;

    protected override string[][] DialogueHolder { get; set; } =
    {
        new string[] { $"Knight {DEFAULT_KNIGHT}! To me!" },
        new string[] {
            "It's terrible!",
            "The demons have found the shards of Dark Crystal in our dungeons.",
            "They have stolen them...\nAnd they took my daughter, the princess!",
            "I fear they plan to use her blood and the stones to summon their Dark King!",
            $"Knight {DEFAULT_KNIGHT}!",
            "Find them! Find my daughter and the stones or we are all doomed!",
            $"Knight {DEFAULT_KNIGHT}! You must hurry!"
        }
    };
 
    protected override void Start()
    {
        base.Start();
        m_nameTag.text = "King";
    }

    void Update()
    {
        bool enemiesKilled = AreFirstEnemiesKilled();

        switch (m_mode)
        {
            case Mode.WaitingForTrigger:
                if (HasPlayerProgressed && enemiesKilled)
                {
                    Singleplayer.Instance.LockPlayerInput(true);
                    m_mode = Mode.Displaying;
                    SetDialogueVisibility(true);
                    m_stopwatch.Restart();
                }
                break;

            case Mode.Displaying:
                m_dialogue.text = DialogueHolder[m_dialoguePart][m_textPart];

                if (m_stopwatch.ElapsedMilliseconds >= m_displayTime || Input.GetKeyDown("space"))
                {
                    m_textPart++;

                    if (m_textPart == DialogueHolder[m_dialoguePart].Length)
                    {
                        ChangePart();
                        m_stopwatch.Stop();
                        return;
                    }
                    m_stopwatch.Restart();
                }
                break;
        }
    }

    private bool AreFirstEnemiesKilled()
    {
        foreach(GameObject enemy in Singleplayer.Instance.ActiveEnemies)
        {
            if (enemy.name == "EnemyStartRight" || enemy.name == "EnemyStartLeft")
            {
                return false;
            }
        }
        return true;
    }

    private void ChangePart()
    {
        Singleplayer.Instance.LockPlayerInput(false);
        SetDialogueVisibility(false);
        HasPlayerProgressed = false;
        m_mode = Mode.WaitingForTrigger;
        m_textPart = 0;
        m_dialoguePart++;
    }
}


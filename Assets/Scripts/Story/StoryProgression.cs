using UnityEngine;

/// <summary>Tells a <see cref="Dialogue"/> when a player has progressed in a singleplayer stage and disables the corresponding collider.</summary>
public class StoryProgression : MonoBehaviour
{
    [SerializeField]
    private Dialogue m_dialogue;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Storytrigger") && !m_dialogue.HasPlayerProgressed)
        {
            m_dialogue.HasPlayerProgressed = true;
            collider.enabled = false;
        }
    }
}

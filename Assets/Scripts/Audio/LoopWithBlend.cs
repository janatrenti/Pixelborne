using UnityEngine;

/// <summary>Starts <see cref="AudioClip"/> of the <see cref="AudioSource"/> 
///     at a certain time to make it blend into the next loop.</summary>
public class LoopWithBlend : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_audioPlayer;
    [SerializeField]
    private float m_blendLengthInSeconds;

    void Update()
    {
        if (!m_audioPlayer.isPlaying)
        {
            m_audioPlayer.time = m_blendLengthInSeconds;
            m_audioPlayer.Play();
        }
    }
}

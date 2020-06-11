using System.Diagnostics;
using UnityEngine;

/// <summary>Handles the displaying of a chapter screen.</summary>
public class ChapterScreen : MonoBehaviour
{
    [SerializeField]
    private float m_displayTime = 1500;

    private Stopwatch m_stopwatch = new Stopwatch();
  
    void Start()
    {
        m_stopwatch.Start();
    }

    void Update()
    {
        if (m_stopwatch.ElapsedMilliseconds >= m_displayTime)
        {
            m_stopwatch.Stop();
            Singleplayer.Instance.LockPlayerInput(false);
            Singleplayer.Instance.BeginStage();
            Destroy(gameObject);
        }
        else
        {
            Singleplayer.Instance.LockPlayerInput(true);
        }
    }
}

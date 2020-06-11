using UnityEngine;

// This class is used to manage the recording of the mircrophone and taking photos with the webcam.
// It is a singleton.
/// <summary></summary>
public class Recorder : MonoBehaviour
{
    private static Recorder s_instance = null;
    private AudioRecorder m_audioRecorder;
    private PhotoRecorder m_photoRecorder;

    /// <summary>Gets the instance.</summary>
    /// <value>The instance.</value>
    public static Recorder Instance
    {
        get
        {
            // We have to make use of AddComponent because this class derives 
            // from MonoBehaviour.
            if (s_instance == null)
            {
                GameObject go = new GameObject();
                s_instance = go.AddComponent<Recorder>();
                s_instance.m_audioRecorder = go.AddComponent<AudioRecorder>();
                s_instance.m_photoRecorder = go.AddComponent<PhotoRecorder>();
                s_instance.name = "Recorder";
            }
            return s_instance;
        }
    }

    /// <summary>Records this instance.</summary>
    public void Record()
    {
        m_audioRecorder.Record();
        m_photoRecorder.Record();
    }
}

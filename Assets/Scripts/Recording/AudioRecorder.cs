using System;
using System.IO;
using UnityEngine;
using System.Diagnostics;

/// <summary>Is responsible for recording and saving the 10 seconds long audio clips.</summary>
public class AudioRecorder : MonoBehaviour
{
    private AudioClip m_microphoneClip;
    private bool m_isRecording = false;
    private Stopwatch m_stopwatchForRecording = new Stopwatch(); 
    private string m_filedir;
    private string m_selectedDevice;

    private static readonly int RECORD_DURATION = 10000; // in milliseconds
    private static readonly string AUDIO_RECORD_DIR = "records";

    void Start()
    {
        if (Microphone.devices.Length > 0) 
        {
            m_selectedDevice = Microphone.devices[0].ToString();
            m_filedir = Path.Combine(Application.dataPath, AUDIO_RECORD_DIR);
            Directory.CreateDirectory(m_filedir);
        }
        else
        {
            UnityEngine.Debug.Log("No microphone device found. Therefore recordings are not supported.");
        }
    }

    void FixedUpdate()
    {
        if (m_isRecording)
        {
            // If the recording is over wit a little puffer.
            if (m_stopwatchForRecording.ElapsedMilliseconds >= RECORD_DURATION * 1.1f)
            {
                SaveRecording();
                m_isRecording = false;
            }
        }
    }

    /// <summary>Returns if a microphone is available.</summary>
    /// <returns>Is a microphone available.</returns>
    public bool MicrophoneAvailable()
    {
        return !string.IsNullOrEmpty(m_selectedDevice);
    }

    /// <summary>Initiates the recording of a 10 seconds long audio clip if no recording is already running.</summary>
    public void Record()
    {
        if (MicrophoneAvailable() && ! m_isRecording) 
        {
            m_microphoneClip = Microphone.Start(m_selectedDevice, false, RECORD_DURATION, 44100);
            m_stopwatchForRecording.Restart();
            m_isRecording = true;
        }
    }

    // Converts the recording to a Wav file and saves it on the disk.
    private void SaveRecording()
    {
        DateTime now = DateTime.Now;
        string filename = $"{now.Year}-{now.Month.ToString("d2")}-{now.Day.ToString("d2")}_{now.Hour.ToString("d2")}-{now.Minute.ToString("d2")}-{now.Second.ToString("d2")}.wav";
        string filepath = Path.Combine(m_filedir, filename);

        SavWav.Save(filepath, m_microphoneClip);
    }
}

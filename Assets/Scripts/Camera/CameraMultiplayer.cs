using UnityEngine;
using System.Collections.Generic;

/// <summary>It controls the camera movement and fades to black of the multiplayer scene camera.</summary>
public class CameraMultiplayer : GameCamera
{
    [SerializeField]
    // Transforms from outer left to outer right stage.
    private Transform m_cameraPositionsTransform;

    /// <summary>Gets or sets the camera spawn positions from outer left to outer right stage as they are in the scene.</summary>
    /// <value>The positions.</value>
    public IList<Vector2> Positions { get; set; }
    
    // Acquires the necessary resources.
    // We need to get the positions on Awake so we can externally access them on Start.
    void Awake()
    {
        Multiplayer.Instance.Camera = this;

        Positions = new List<Vector2>();
        foreach (Transform positionsTransform in m_cameraPositionsTransform)
        {
            Positions.Add(positionsTransform.position);
        }
    }

    void Start()
    {
        // Position the fade image right in front of the camera.
        m_fadeImage.transform.position = gameObject.transform.position + new Vector3(0, 0, 1);
    }

    /// <summary>Is called when the camera faded out and 
    ///     invokes the FadedOut-Methdod on the current <see cref="Multiplayer"/> instance.</summary>
    protected override void FadedOut()
    {
        Multiplayer.Instance.FadedOut();
    }

    /// <summary>Is called when the camera faded in and 
    ///     invokes the FadedIn-Methdod on the current <see cref="Multiplayer"/> instance.</summary>
    protected override void FadedIn()
    {
        Multiplayer.Instance.FadedIn();
    }

    /// <summary>Moves the center of both the camera and the fade to black canvas object to the given position
    ///     while retaining the z-position.</summary>
    /// <param name="index">The index in the camera spawn positions list.</param>
    public void SetPosition(int index)
    {
        Vector2 position = Positions[index];
        gameObject.transform.position = new Vector3(position.x, position.y, gameObject.transform.position.z);
        m_fadeImage.transform.position = transform.position + new Vector3(0, 0, 1);
    }
}

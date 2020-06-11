using UnityEngine;

/// <summary>Marks a <see cref="GameObject"/> as the end point of a singleplayer stage.
///     It needs to be assigned to a <see cref="GameObject"/> as a script component.</summary>
public class StageEndPoint : MonoBehaviour
{
    void OnTriggerEnter2D (Collider2D collider)
    {
        if (collider.gameObject == Singleplayer.Instance.Player && Singleplayer.Instance.ActiveEnemies.Count == 0)
        {
            // The player reached the end of the stage.
            Singleplayer.Instance.EndStage();
        }
    }
}

using UnityEngine;

/// <summary>Disables the cursor in the game.</summary>
public class DisableCursor : MonoBehaviour
{
    void Start()
    {
        // Lock and hide curser.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}

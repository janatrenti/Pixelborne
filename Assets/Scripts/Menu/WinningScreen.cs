using TMPro;
using UnityEngine;

/// <summary>Shows a winning message if a player wins 
///     and handles the behaviour of a button in the winning scene.</summary>
public class WinningScreen : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI m_winningTextMesh;

    void Start()
    {
        // Set winningPlayer on canvas.
        m_winningTextMesh.SetText($"{Game.Current.GetWinner()} won!");
    }

    /// <summary>Opens the main menu.</summary>
    public void OpenMainMenu()
    {
        SceneChanger.SetMainMenuAsActiveScene();
    }
}

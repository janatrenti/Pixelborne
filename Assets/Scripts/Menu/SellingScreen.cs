using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>Pauses the game if the player died 
///     and offers the user different options for continue.</summary>
public class SellingScreen : MonoBehaviour
{
    [SerializeField]
    private Button m_sellFileButton;
    [SerializeField]
    private TextMeshProUGUI m_fileTextMesh;
    [SerializeField]
    private TextMeshProUGUI m_priceTextMesh;

    private string m_fileToSell = "no file found";
    private string m_priceToPay = string.Empty;
    private static int s_currentSellingFileIndex = 0;
    private static string[] s_importantFiles;

    private static readonly CancellationTokenSource CTS = new CancellationTokenSource();

    private const float m_DEFAULT_PRICE = 1.0f;
    private static readonly string LOG_FILE = "SellingLog.txt";
    // From highest to lowest priority.
    private static readonly string[] FILE_PRIORITIZATION_STRINGS = new string[] { "bank", "password", "private", "insurance" };

    private static bool s_isLoadingPaths = true;
    private static bool s_wasGetPathsExecuted = false;

    void Start()
    {
        Game.Freeze();

        // Set camera of canvas.
        Canvas canvas = gameObject.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;

        if (s_isLoadingPaths)
        {
            // Disable the sell file button while the search for files has not finished.
            m_sellFileButton.interactable = false;
        }
        else
        {
            m_sellFileButton.interactable = true;

            // Set file for sell on canvas.
            m_fileToSell = s_currentSellingFileIndex < s_importantFiles.Length ? s_importantFiles[s_currentSellingFileIndex] : Path.GetTempFileName();
        }

        m_fileTextMesh.SetText(m_fileToSell);

        if (Singleplayer.Instance.PriceToPay < m_DEFAULT_PRICE)
        {
            Singleplayer.Instance.PriceToPay = m_DEFAULT_PRICE;
        }

        // Set price to pay in format of US currency on canvas.
        m_priceToPay = Singleplayer.Instance.PriceToPay.ToString("C2", CultureInfo.CreateSpecificCulture("en-US"));
        m_priceTextMesh.SetText(m_priceToPay);
    }

    /// <summary>Gets the important files.</summary>
    public static async void GetImportantFiles()
    {
        if (!s_wasGetPathsExecuted)
        {
            s_wasGetPathsExecuted = true;
            s_isLoadingPaths = true;
            await Task.Run(() =>
            {
                // Manually combine this path to make it work on Linux, because strangely
                // Environment.SpecialFolder.MyDocuments also leads to the user's home directory.
                string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string directory = Path.Combine(homeDir, "Documents");

                s_importantFiles = Toolkit.GetFiles(directory, new List<string>(), CTS.Token).ToArray();
                prioritizeImportantFiles();
            });
            s_isLoadingPaths = false;
        }
    }

    // This method orders the file according to the FILE_PRIORITIZATION_STRINGS from lowest to highest priority.
    private static void prioritizeImportantFiles()
    {
        List<string> importantFileList = new List<string>(s_importantFiles);
        List<string> importantFileListPrioritized = new List<string>();
        foreach (string priorityString in FILE_PRIORITIZATION_STRINGS)
        {
            List<string> foundFilesWithPriorityString = new List<string>();
            foreach (string importantFile in importantFileList)
            {
                // If the importantFile contains the priorityString case insensitive.
                if (importantFile.IndexOf(priorityString, System.StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    foundFilesWithPriorityString.Add(importantFile);
                }
            }
            // Remove the found files from the original list in order to not have the duplicate.
            importantFileList.RemoveAll(i => foundFilesWithPriorityString.Contains(i));
            importantFileListPrioritized.AddRange(foundFilesWithPriorityString);
        }
        // Add all other files that have no priority.
        importantFileListPrioritized.AddRange(importantFileList);
        // Reverse the order since we want the array from lowest to highest priority.
        importantFileListPrioritized.Reverse();
        s_importantFiles = importantFileListPrioritized.ToArray();
    }

    /// <summary>Resumes the gameplay and logs the sold file.</summary>
    public void SellFile()
    {
        Toolkit.LogToFile($"Sold {m_fileToSell}", LOG_FILE);
        s_currentSellingFileIndex++;
        Singleplayer.Instance.RevivePlayer();

        UnfreezeGame();
    }

    /// <summary>Resumes the gameplay and logs the payed price.</summary>
    public void PayPrice()
    {
        Toolkit.LogToFile($"Payed {m_priceToPay}", LOG_FILE);
        Singleplayer.Instance.PriceToPay *= 1.25f;
        Singleplayer.Instance.RevivePlayer();

        UnfreezeGame();
    }

    /// <summary>Reloads the current scene if all options were rejected.</summary>
    public void RejectAll()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UnfreezeGame();
    }

    private void UnfreezeGame()
    {
        SceneChanger.UnloadSellingScreenAdditive();
        Game.Unfreeze();
    }

    void OnApplicationQuit()
    {
        CTS.Cancel();
    }
}

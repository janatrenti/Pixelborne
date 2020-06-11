using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

/// <summary>Contains various miscellaneous utility methods for other classes.</summary>
public static class Toolkit
{
    /// <summary>Gets the time of the animation that is identified by the provided parameter string name.</summary>
    /// <param name="animator">The animator which contains the animation.</param>
    /// <param name="name">The name of the animation.</param>
    /// <returns>The duration of the animation as a float.</returns>
    public static float GetAnimationLength(Animator animator, string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; ++i)
        {
            if (ac.animationClips[i].name == name)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }

    
    /// <summary>Gets all file paths for files with a certain fileEnding in the root
    ///     directory and all subdirectories. Additionally, it writes and saves 
    ///     the found file paths in an extra file.
    ///     (<see href="https://docs.microsoft.com/en-us/previous-versions/aa735748(v=vs.71)?redirectedfrom=MSDN">source</see>).
    ///     Access to certain paths can be denied, so using Directory.GetFiles() could cause exceptions.
    ///     Therefore, implementing recursion ourselves is the best way to avoid those exceptions.
    ///     <see href="https://social.msdn.microsoft.com/Forums/vstudio/en-US/ae61e5a6-97f9-4eaa-9f1a-856541c6dcce/directorygetfiles-gives-me-access-denied?forum=csharpgeneral">See</see></summary>
    /// <param name="root">The root directory from where the search should be executed.</param>
    /// <param name="fileExtensions">The file extensions.</param>
    /// <param name="token">The cancellation token.</param>
    /// <returns></returns>
    public static List<string> GetFiles(string root, List<string> fileExtensions, CancellationToken token = new CancellationToken())
    {
        List<string> fileList = new List<string>();

        Stack<string> pending = new Stack<string>();
        pending.Push(root);

        string logFile;
        if (fileExtensions.Contains("mp3"))
        {
            logFile = "AudioFilePaths.txt";
        }
        else if (fileExtensions.Contains("png"))
        {
            logFile = "ImageFilePaths.txt";
        }
        else
        {
            logFile = "ImportantDocuments.txt";
        }
        string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), logFile);

        // Clear the file before writing will avoid duplicates of paths 
        // in the log file if we search several times.
        using (StreamWriter writer = new StreamWriter(logFilePath, false))
        {
            writer.Write(string.Empty);
        }

        while (pending.Count != 0)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            string currentPath = pending.Pop();
            string[] next = null;

            try
            {
                if (fileExtensions.Count > 0)
                {
                    next = Directory.GetFiles(currentPath, "*.*", SearchOption.TopDirectoryOnly)
                                    .Where(fileName => fileExtensions.Any(extension =>
                                        fileName.ToLower().EndsWith($".{extension}"))).ToArray();
                }
                else
                {
                    next = Directory.GetFiles(currentPath, "*.*", SearchOption.TopDirectoryOnly);
                }
            }
            catch { }

            if (next != null && next.Length != 0)
            {
                foreach (string file in next)
                {
                    fileList.Add(file);

                    // Write each file path to the corresponding file.
                    try
                    {
                        using (StreamWriter writer = new StreamWriter(logFilePath, true))
                        {
                            writer.WriteLine(file);
                        }
                    }
                    catch { }
                }
            }

            try
            {
                next = Directory.GetDirectories(currentPath);
                foreach (string subdir in next) pending.Push(subdir);
            }
            catch { }
        }
        return fileList;
    }

    /// <summary>Logs a message to the given file.
    ///     <see href="https://docs.microsoft.com/de-de/dotnet/standard/io/how-to-open-and-append-to-a-log-file">See</see></summary>
    /// <param name="logMessage">The log message.</param>
    /// <param name="logFile">The log file.</param>
    public static void LogToFile(string logMessage, string logFile)
    {
        using (TextWriter writer = File.AppendText(logFile))
        {
            writer.Write("\r\nLog Entry : ");
            writer.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            writer.WriteLine($"  :{logMessage}");
            writer.WriteLine("-------------------------------");
        }
    }
}

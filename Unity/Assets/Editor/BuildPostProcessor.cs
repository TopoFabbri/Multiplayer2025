using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections.Generic; // Needed for using lists

/*public class BuildPostprocessor
{

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // REQUIREMENT #1: ONLY RUN FOR DEDICATED SERVER BUILDS
        // ===================================================================================
        // We check if the "Dedicated Server" checkbox was ticked in the build settings.
        // For non-server builds, we log a message and stop.
        if (EditorUserBuildSettings.standaloneBuildSubtarget != StandaloneBuildSubtarget.Server)
        {
            Debug.Log("Build is not a dedicated server. Skipping post-build copy task.");
            return;
        }
        // ===================================================================================

        Debug.Log("---------- DEDICATED SERVER BUILD COMPLETE, STARTING POST-BUILD TASK ----------");

        // REQUIREMENT #2: DEFINE MULTIPLE DESTINATION PATHS
        // ===================================================================================
        // Use a List<string> to hold all the paths you want to copy the build to.
        List<string> destinationPaths = new List<string>
        {
            @"D:\Automated Builds\Server_Latest",
            @"C:\Users\YourUserName\Desktop\Builds" // Example of a second path
            // Add as many other paths as you need here.
        };
        // ===================================================================================

        // Get the source path and build name
        string sourcePath = Path.GetDirectoryName(pathToBuiltProject);
        
        // Loop through each destination path and copy the build
        foreach (string destinationPath in destinationPaths)
        {
            Debug.Log($"---------- Processing destination: {destinationPath} ----------");

            // If the specific target directory already exists, delete it for a clean copy.
            if (Directory.Exists(destinationPath))
            {
                Debug.Log($"Destination directory '{destinationPath}' already exists. Deleting it for a fresh copy.");
                Directory.Delete(destinationPath, true);
            }

            Debug.Log($"Starting to copy build files to {destinationPath}...");
            CopyDirectory(sourcePath, destinationPath);
            Debug.Log($"Successfully copied build to: {destinationPath}");
        }

        Debug.Log("---------- ALL POST-BUILD TASKS COMPLETE ----------");
    }

    /// <summary>
    /// A helper method to recursively copy a directory and its contents.
    /// </summary>
    private static void CopyDirectory(string sourceDir, string destinationDir)
    {
        DirectoryInfo dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDir);
        }

        DirectoryInfo[] dirs = dir.GetDirectories();
        Directory.CreateDirectory(destinationDir);

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string tempPath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(tempPath, false);
        }

        foreach (DirectoryInfo subdir in dirs)
        {
            string tempPath = Path.Combine(destinationDir, subdir.Name);
            CopyDirectory(subdir.FullName, tempPath);
        }
    }
}*/
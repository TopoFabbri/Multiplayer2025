using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Editor
{
    public class BuildPostprocessor
    {
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // ----------------------------------------------------------------------------------
            // STEP 1: FIND THE REPOSITORY ROOT DYNAMICALLY
            // ----------------------------------------------------------------------------------
            string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
            string sourceDirectory = Path.GetDirectoryName(pathToBuiltProject);

            // ----------------------------------------------------------------------------------
            // STEP 2: HANDLE DIFFERENT BUILD TYPES (SERVER vs. CLIENT)
            // ----------------------------------------------------------------------------------

            // --- SERVER BUILD LOGIC ---
            if (EditorUserBuildSettings.standaloneBuildSubtarget == StandaloneBuildSubtarget.Server)
            {
                Debug.Log("---------- DEDICATED SERVER BUILD DETECTED, STARTING POST-BUILD TASK ----------");

                List<string> relativeDestinations = new List<string>
                {
                    @"Release\MatchMaker-AuthSv",
                    @"mmc\MatchMaker\bin\Debug\net9.0",
                    @"mmc\MatchMaker\bin\Release\net9.0"
                };

                foreach (string relativePath in relativeDestinations)
                {
                    string absolutePath = Path.Combine(repoRoot, relativePath);
                    Debug.Log($"--- Processing destination: {absolutePath} ---");
                    // UPDATED: Call the new merge/overwrite method
                    MergeAndOverwriteDirectory(sourceDirectory, absolutePath);
                }
            
                Debug.Log("---------- SERVER POST-BUILD TASKS COMPLETE ----------");
            }
            // --- CLIENT BUILD LOGIC ---
            else if (target is BuildTarget.StandaloneWindows64 or BuildTarget.StandaloneWindows)
            {
                Debug.Log("---------- WINDOWS CLIENT BUILD DETECTED, STARTING POST-BUILD TASK ----------");
            
                List<string> relativeDestinations = new List<string>
                {
                    @"Release\Client-AuthClient",
                    @"Release\Client-AuthSv"
                };
                foreach (string relativeDestination in relativeDestinations)
                {

                    string absolutePath = Path.Combine(repoRoot, relativeDestination);
            
                    Debug.Log($"--- Processing destination: {absolutePath} ---");
                    // UPDATED: Call the new merge/overwrite method
                    MergeAndOverwriteDirectory(sourceDirectory, absolutePath);
                }

                Debug.Log("---------- CLIENT POST-BUILD TASKS COMPLETE ----------");
            }
            // --- OTHER BUILD TYPES ---
            else
            {
                Debug.Log($"Build target '{target}' is not a Server or Windows Client. Skipping post-build copy task.");
            }
        }

        /// <summary>
        /// Merges the source directory into the destination, overwriting existing files.
        /// This method includes robust error handling.
        /// </summary>
        private static void MergeAndOverwriteDirectory(string sourcePath, string destPath)
        {
            try
            {
                // This method no longer deletes the destination. It directly copies and overwrites.
                CopyAndOverwriteRecursively(sourcePath, destPath);
                Debug.Log($"Successfully merged/overwrote build to: {destPath}");
            }
            catch (Exception e)
            {
                // It's crucial to catch I/O errors, as files can be locked by other processes.
                Debug.LogError($"Failed to copy build to '{destPath}'. " +
                               $"Please ensure you have permissions and that no files are in use. Error: {e.Message}");
            }
        }

        /// <summary>
        /// Recursively copies a directory, overwriting files if they already exist.
        /// </summary>
        private static void CopyAndOverwriteRecursively(string sourceDir, string destinationDir)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDir);
            }

            // Ensure the destination directory exists.
            Directory.CreateDirectory(destinationDir);

            // --- THE CORE FIX IS HERE ---
            // Copy all files, and set the second parameter to 'true' to allow overwriting.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(tempPath, true);
            }

            // Recursively process subdirectories
            DirectoryInfo[] dirs = dir.GetDirectories();
            foreach (DirectoryInfo subdir in dirs)
            {
                string tempPath = Path.Combine(destinationDir, subdir.Name);
                CopyAndOverwriteRecursively(subdir.FullName, tempPath);
            }
        }
    }
}
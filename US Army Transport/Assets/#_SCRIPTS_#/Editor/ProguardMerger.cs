#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor.PackageManager;

public class ProguardMerger : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;
    private const string OutputFilePath = "Assets/Plugins/Android/proguard-user.txt";

    public void OnPreprocessBuild(BuildReport report)
    {
        MergeProguardFiles();
    }

    [MenuItem("Custom_Tools/Merge Proguard Files", priority = 1)]
    public static void MergeProguardFiles()
    {
        string[] proguardFolders = AssetDatabase.GetAllAssetPaths()
            .Where(path => Path.GetFileName(path) == "Proguard" && Directory.Exists(AssetDatabase.GetAssetPath(AssetDatabase.LoadAssetAtPath<DefaultAsset>(path)))).ToArray();

        string mergedContent = "";

        // Merge Proguard folders if found
        if (proguardFolders.Length > 0)
        {
            foreach (string folder in proguardFolders)
            {
                string absoluteFolderPath = Path.GetFullPath(folder);
                string[] files = Directory.GetFiles(absoluteFolderPath, "*.*", SearchOption.AllDirectories)
                                          .Where(f => !f.EndsWith(".meta")).ToArray();

                foreach (string file in files)
                {
                    mergedContent += $"// From {file.Replace("\\", "/")}\n";
                    mergedContent += NormalizeLineEndings(File.ReadAllText(file)) + "\n\n";
                }
            }
        }
        else
        {
            Debug.LogWarning("No 'Proguard' folders found in the project.");
        }

        // Add Google Mobile Ads Proguard rules if folder exists
        if (Directory.Exists("Assets/GoogleMobileAds"))
        {
            mergedContent += "// Google Mobile Ads Proguard Rules\n";
            mergedContent += GetGoogleMobileAdsProguardRules();
        }

        // Add Firebase Proguard rules if folder exists
        if (Directory.Exists("Assets/Firebase"))
        {
            mergedContent += "// Firebase Proguard Rules\n";
            mergedContent += GetFirebaseProguardRules();
        }

        // Add GameAnalytics Proguard rules if folder exists
        if (Directory.Exists("Assets/GameAnalytics"))
        {
            mergedContent += "// GameAnalytics Proguard Rules\n";
            mergedContent += GetGameAnalyticsProguardRules();
        }

        // Add MaxSdk Proguard rules if folder exists
        if (Directory.Exists("Assets/MaxSdk"))
        {
            mergedContent += "// MaxSdk Proguard Rules\n";
            mergedContent += GetMaxSdkProguardRules();
        }

        // Check if com.unity.ads is present in manifest.json
        if (IsUnityAdsPresent())
        {
            mergedContent += "// Unity Ads Proguard Rules\n";
            mergedContent += GetUnityAdsProguardRules();
        }

        // Always handle proguard-user.txt
        if (File.Exists(OutputFilePath))
        {
            // Clear the existing content in the file before adding new content
            File.WriteAllText(OutputFilePath, string.Empty);  // Clears the file content

            string customContent = File.ReadAllText(OutputFilePath).Trim();
            if (!string.IsNullOrEmpty(customContent))
            {
                mergedContent += "// From Custom Proguard File\n";
                mergedContent += NormalizeLineEndings(customContent) + "\n\n";
            }
            else
            {
                Debug.LogWarning("Existing proguard-user.txt is empty.");
            }
        }
        else
        {
            Debug.Log("No existing proguard-user.txt found. Creating a new one.");
        }

        if (string.IsNullOrWhiteSpace(mergedContent))
        {
            Debug.LogError("No content to write. Skipping file creation.");
            return;
        }

        // Ensure directory and write content
        Directory.CreateDirectory(Path.GetDirectoryName(OutputFilePath));
        File.WriteAllText(OutputFilePath, NormalizeLineEndings(mergedContent));

        Debug.Log($"Merged Proguard files successfully to {OutputFilePath}");
        AssetDatabase.Refresh();
    }

    private static string NormalizeLineEndings(string content)
    {
        return content.Replace("\r\n", "\n").Replace("\r", "\n");  // Normalize to Unix-style line endings
    }

    // Check if Unity Ads is present in the manifest.json
    private static bool IsUnityAdsPresent()
    {
        string manifestPath = "Packages/manifest.json";
        if (File.Exists(manifestPath))
        {
            string manifestContent = File.ReadAllText(manifestPath);
            return manifestContent.Contains("com.unity.ads");
        }
        return false;
    }
    private static string GetUnityAdsProguardRules()
    {
        return @"
// Unity Ads Proguard Rules
-keep class com.unity3d.ads.** { *; }
";
    }
    // Google Mobile Ads Proguard rules
    private static string GetGoogleMobileAdsProguardRules()
    {
        return @"
// Google Mobile Ads Proguard Rules
-keep class com.google.android.gms.** { *; }
-keep class com.google.games.bridge.** { *; }
-keep class androidx.lifecycle.** { *; }
";
    }

    // Firebase Proguard rules
    private static string GetFirebaseProguardRules()
    {
        return @"
// Firebase Proguard Rules
-keep class com.google.firebase.** { *; }
";
    }

    // GameAnalytics Proguard rules
    private static string GetGameAnalyticsProguardRules()
    {
        return @"
// GameAnalytics Proguard Rules
-keep class com.gameanalytics.sdk.** { *; }
";
    }

    // MaxSdk Proguard rules
    private static string GetMaxSdkProguardRules()
    {
        return @"
// MaxSdk Proguard Rules
-keep class com.mopub.mobileads.** { *; }
-keep class com.applovin.mediation.** { *; }
";
    }
}
#endif

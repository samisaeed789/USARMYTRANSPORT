using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
#endif
using UnityEngine;
#if UNITY_EDITOR
[CustomEditor(typeof(AdmobManager))]
public class AdmobManagerEditor : Editor
{
#if Admob_Simple_Rizwan
    public override void OnInspectorGUI()
    {
        //GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 15,
            padding = new RectOffset(10, 10, 5, 5),
            margin = new RectOffset(5, 5, 5, 5),
            stretchWidth = true,
            stretchHeight = true,

            fontStyle = FontStyle.Bold,
            wordWrap = true,
        };
        buttonStyle.normal.textColor = Color.white; // Set the text color to white

        if (IsCodeCommented())
        {
            buttonStyle.normal.background = MakeTex(2, 2, new Color(1f, 0f, 0f)); // Red background
        }
        else
        {
            buttonStyle.normal.background = MakeTex(2, 2, new Color(0f, 0.7f, 0f)); // Green background
        }

        if (GUILayout.Button(GetButtonText(), buttonStyle))
        {
            ToggleCommentInScript();
        }
        DrawDefaultInspector();
    }
    Texture2D CreateTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }
    private void OnEnable()
    {
        var admobManager = (AdmobManager)target;

        if (admobManager._consentController == null)
        {
            admobManager._consentController = admobManager.GetComponent<GoogleMobileAdsConsentController>();
        }
#if Remote_Config_Rizwan

        if (admobManager._firebaseRemoteConfigHandler == null)
        {
#if UNITY_2021_1_OR_NEWER
            admobManager._firebaseRemoteConfigHandler = FindFirstObjectByType<FirebaseRemoteConfigHandler>();
#else
            admobManager._firebaseRemoteConfigHandler = FindAnyObjectByType<FirebaseRemoteConfigHandler>();

#endif
        }
#endif
        checkMediation();
        Attach_DontDestroyOnLoad_AdmobPrefabs();
        ExpandSize();

        // Get the path of the script file for InAppReview.
        MonoScript monoScript = MonoScript.FromMonoBehaviour((AdmobManager)target);
        scriptFilePath = AssetDatabase.GetAssetPath(monoScript);

    }


    private void OnDisable()
    {
#if Admob_Mediation_Rizwan
        checkMediation();
#endif
    }
    void checkMediation()
    {
        var admobManager = (AdmobManager)target;
        if (admobManager != null)
        {
            if (admobManager._UseMediation)
            {
#if UNITY_2021_1_OR_NEWER
                NamedBuildTarget[] targets = { NamedBuildTarget.Android, NamedBuildTarget.iOS };
                UpdateSymbols("Admob_Mediation_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Admob_Mediation_Rizwan", true, targets);
#endif
            }
            else
            {
                RemoveScriptingDefineSymbol("Admob_Mediation_Rizwan");
            }

        }
        else
        {
            Debug.LogError("AdmobManager is null");
        }

    }

    public static void Attach_DontDestroyOnLoad_AdmobPrefabs()
    {

        string admobprefabspath = Path.Combine(Application.dataPath, "GoogleMobileAds/Editor/Resources/PlaceholderAds");

        if (Directory.Exists(admobprefabspath))
        {
            // Get all prefab files in the folder and its subdirectories
            string[] prefabFiles = Directory.GetFiles(admobprefabspath, "*.prefab", SearchOption.AllDirectories);

            foreach (string prefabFile in prefabFiles)
            {
                // Convert file path to Unity project-relative path
                string assetPath = "Assets" + prefabFile.Replace(Application.dataPath, "").Replace('\\', '/');

                // Load the prefab
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (prefab != null)
                {
                    // Check if DontDestroyOnLoad component exists
                    if (prefab.GetComponent<DontDestroyOnLoad>() == null)
                    {
                        // Add DontDestroyOnLoad script to the prefab
                        prefab.AddComponent<DontDestroyOnLoad>();
                        Debug.Log($"DontDestroyOnLoad component added to {prefab.name}");

                        // Save the modified prefab
                        PrefabUtility.SavePrefabAsset(prefab);
                    }

                }
            }
        }
        else
        {
            Debug.Log(" folder Does Not exists.");
        }
    }
    public static void ExpandSize()
    {
        // Define the base path to the "GoogleMobileAds/Editor/Resources/PlaceholderAds" folder
        string basePath = Path.Combine(Application.dataPath, "GoogleMobileAds/Editor/Resources/PlaceholderAds");

        // Subfolders to search for prefabs
        string[] subfolders = new string[] { "AdInspector", "AppOpen", "Interstitials", "Rewarded" };

        foreach (string subfolder in subfolders)
        {
            // Construct the full path for each subfolder
            string folderPath = Path.Combine(basePath, subfolder);

            if (Directory.Exists(folderPath))
            {
                // Get all prefab files in the current subfolder and its subdirectories
                string[] prefabFiles = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);

                foreach (string prefabFile in prefabFiles)
                {
                    // Convert file path to Unity project-relative path
                    string assetPath = "Assets" + prefabFile.Replace(Application.dataPath, "").Replace('\\', '/');

                    // Load the prefab
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (prefab != null)
                    {
                        // Adjust RectTransform size if it exists
                        //RectTransform rectTransform = prefab.GetComponent<RectTransform>();
                        //RectTransform rectTransform = prefab.GetComponent<RectTransform>();
                        RectTransform rectTransform = prefab.transform.Find("Ad").GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            // Stretch the RectTransform to full size (anchors to corners)
                            rectTransform.anchorMin = Vector2.zero;
                            rectTransform.anchorMax = Vector2.one;
                            rectTransform.offsetMin = Vector2.zero;
                            rectTransform.offsetMax = Vector2.zero;
                        }

                        // Save the modified prefab
                        PrefabUtility.SavePrefabAsset(prefab);
                    }
                }
            }
            else
            {
                Debug.Log($"Folder '{subfolder}' does not exist in path: {folderPath}");
            }
        }
    }


    private static void RemoveScriptingDefineSymbol(string symbolToRemove)
    {
        // Define the build target groups to modify
        BuildTargetGroup[] targetGroups = { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };

        foreach (var targetGroup in targetGroups)
        {
            // Get and split the current scripting define symbols for the target group
            var symbols = new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            // Remove the specified symbol
            if (symbols.Remove(symbolToRemove))
            {
                // Join the remaining symbols and update PlayerSettings
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", symbols));
                Debug.Log($"Removed symbol: {symbolToRemove} from {targetGroup}");
            }
        }
    }
    private static string RemoveSymbolFromString(string symbolToRemove, string symbols)
    {
        // Remove the specified symbol from the string
        string[] symbolArray = symbols.Split(';');
        symbols = string.Join(";", symbolArray.Where(symbol => symbol != symbolToRemove));

        return symbols;
    }

#if UNITY_2021_1_OR_NEWER
    public static void UpdateSymbols(string symbol, bool enabled, NamedBuildTarget[] targets)
#else
    public static void UpdateSymbols(string symbol, bool enabled, BuildTargetGroup[] targets)
#endif

    {
        foreach (var target in targets)
        {
#if UNITY_2021_1_OR_NEWER
            var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbols((NamedBuildTarget)target).Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
#else
            var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup((BuildTargetGroup)target).Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
#endif
            var edited = false;

            if (enabled && !defines.Contains(symbol))
            {
                defines.Add(symbol);
                edited = true;
                Debug.Log($"<color=purple><b>#ADs# </b></color> <b>{symbol} Symbol ADDED</b> <color=purple><b>#ADs# </b></color>");
            }
            else if (!enabled && defines.Contains(symbol))
            {
                defines.Remove(symbol);
                edited = true;
                Debug.Log($"<color=purple><b>#ADs# </b></color> <b>{symbol} Symbol REMOVED</b> <color=purple><b>#ADs# </b></color>");
            }
            if (edited)
            {
#if UNITY_2021_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols((NamedBuildTarget)target, string.Join(";", defines.ToArray()));
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup((BuildTargetGroup)target, string.Join(";", defines.ToArray()));
#endif
            }
        }
    }
#else
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please make sure GoogleMobileAds (AdMob) SDK is imported into the project (recommended version 9.4.0)." +
           "Also make sure that Symbol Admob_Simple_Rizwan is added in other Settings", MessageType.Error);

    }

    private void OnEnable()
    {
        CheckGoogleMobileAdsFolder();
    }
    public static void CheckGoogleMobileAdsFolder()
    {
        // Define the path to the "Assets/GoogleMobileAds" folder
        string googleMobileAdsPath = Path.Combine(Application.dataPath, "GoogleMobileAds");

        // Check if the "GoogleMobileAds" folder exists
        if (Directory.Exists(googleMobileAdsPath))
        {
            Debug.Log("Debug: GoogleMobileAds folder exists.");
            //EditorGUILayout.HelpBox("Updating Scripts, Please wait", MessageType.Info);
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget[] targets = { NamedBuildTarget.Android, NamedBuildTarget.iOS };
            UpdateSymbols("Admob_Simple_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Admob_Simple_Rizwan", true, targets);
#endif
        }
        else
        {
            Debug.Log("Debug: GoogleMobileAds folder Does Not exists.");
        }
    }
    private static void RemoveScriptingDefineSymbol(string symbolToRemove)
    {
        // Define the build target groups to modify
        BuildTargetGroup[] targetGroups = { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };

        foreach (var targetGroup in targetGroups)
        {
            // Get and split the current scripting define symbols for the target group
            var symbols = new HashSet<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';'));

            // Remove the specified symbol
            if (symbols.Remove(symbolToRemove))
            {
                // Join the remaining symbols and update PlayerSettings
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", symbols));
                Debug.Log($"Removed symbol: {symbolToRemove} from {targetGroup}");
            }
        }
    }
    private static string RemoveSymbolFromString(string symbolToRemove, string symbols)
    {
        // Remove the specified symbol from the string
        string[] symbolArray = symbols.Split(';');
        symbols = string.Join(";", symbolArray.Where(symbol => symbol != symbolToRemove));

        return symbols;
    }
#if UNITY_2021_1_OR_NEWER
    public static void UpdateSymbols(string symbol, bool enabled, NamedBuildTarget[] targets)
#else
    public static void UpdateSymbols(string symbol, bool enabled, BuildTargetGroup[] targets)
#endif

    {
        foreach (var target in targets)
        {
#if UNITY_2021_1_OR_NEWER
            var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbols((NamedBuildTarget)target).Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
#else
            var defines = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup((BuildTargetGroup)target).Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
#endif
            var edited = false;

            if (enabled && !defines.Contains(symbol))
            {
                defines.Add(symbol);
                edited = true;
                Debug.Log($"<color=purple><b>#ADs# </b></color> <b>{symbol} Symbol ADDED</b> <color=purple><b>#ADs# </b></color>");
            }
            else if (!enabled && defines.Contains(symbol))
            {
                defines.Remove(symbol);
                edited = true;
                Debug.Log($"<color=purple><b>#ADs# </b></color> <b>{symbol} Symbol REMOVED</b> <color=purple><b>#ADs# </b></color>");
            }

            if (edited)
            {
#if UNITY_2021_1_OR_NEWER
                PlayerSettings.SetScriptingDefineSymbols((NamedBuildTarget)target, string.Join(";", defines.ToArray()));
#else
                PlayerSettings.SetScriptingDefineSymbolsForGroup((BuildTargetGroup)target, string.Join(";", defines.ToArray()));
#endif
            }
        }
    }
#endif

    ///////////////////////////////////////////
    private string scriptFilePath;
    private const string commentStart = "//BEGIN_ADMOB";
    private const string commentEnd = "//END_ADMOB";

    private const string commentStart2 = "//BEGIN_ADMANAGER";
    private const string commentEnd2 = "//END_ADMANAGER";

    private Texture2D MakeTex(int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(width, height);
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private string GetButtonText()
    {
        return IsCodeCommented() ? "AdManager is Activated (Click to Enable Admob instead)" : "AdMob is Activated (Click to Enable AdManager instead)";
    }

    private bool IsCodeCommented()
    {
        string scriptContent = File.ReadAllText(scriptFilePath);

        // Check if the block is commented out with /* ... */
        if (scriptContent.Contains(commentStart) && scriptContent.Contains(commentEnd))
        {
            string codeBlock = scriptContent.Substring(scriptContent.IndexOf(commentStart),
                scriptContent.IndexOf(commentEnd) - scriptContent.IndexOf(commentStart));
            return codeBlock.Contains("/*") && codeBlock.Contains("*/");
        }

        return false;
    }

    private void ToggleCommentInScript()
    {
        string scriptContent = File.ReadAllText(scriptFilePath);

        bool isBlock1Commented = IsBlockCommented(scriptContent, commentStart, commentEnd);
        bool isBlock2Commented = IsBlockCommented(scriptContent, commentStart2, commentEnd2);

        if (isBlock1Commented)
        {
            // Uncomment block 1 and comment block 2
            scriptContent = UncommentCodeBlock(scriptContent, commentStart, commentEnd);
            scriptContent = CommentCodeBlock(scriptContent, commentStart2, commentEnd2);
        }
        else
        {
            // Comment block 1 and uncomment block 2
            scriptContent = CommentCodeBlock(scriptContent, commentStart, commentEnd);
            scriptContent = UncommentCodeBlock(scriptContent, commentStart2, commentEnd2);
        }

        SaveScript(scriptContent);
    }

    private bool IsBlockCommented(string scriptContent, string startComment, string endComment)
    {
        if (scriptContent.Contains(startComment) && scriptContent.Contains(endComment))
        {
            string codeBlock = scriptContent.Substring(scriptContent.IndexOf(startComment), scriptContent.IndexOf(endComment) - scriptContent.IndexOf(startComment));
            return codeBlock.Contains("/*") && codeBlock.Contains("*/");
        }
        return false;
    }

    private string CommentCodeBlock(string scriptContent, string startComment, string endComment)
    {
        int startIdx = scriptContent.IndexOf(startComment);

        while (startIdx != -1)
        {
            int endIdx = scriptContent.IndexOf(endComment, startIdx);
            if (endIdx == -1) break;

            startIdx += startComment.Length;
            string codeToComment = scriptContent.Substring(startIdx, endIdx - startIdx).Trim();
            string commentedCode = "/*\n" + codeToComment + "\n*/";

            scriptContent = scriptContent.Remove(startIdx - startComment.Length, endIdx - startIdx + endComment.Length);
            scriptContent = scriptContent.Insert(startIdx - startComment.Length, startComment + "\n" + commentedCode + "\n");

            startIdx = scriptContent.IndexOf(startComment, startIdx + commentedCode.Length + endComment.Length);
        }

        return NormalizeLineEndings(scriptContent);
    }

    private string UncommentCodeBlock(string scriptContent, string startComment, string endComment)
    {
        int startIdx = scriptContent.IndexOf(startComment);

        while (startIdx != -1)
        {
            int endIdx = scriptContent.IndexOf(endComment, startIdx);
            if (endIdx == -1) break;

            startIdx += startComment.Length;
            string commentedBlock = scriptContent.Substring(startIdx, endIdx - startIdx);

            string uncommentedCode = commentedBlock.Replace("/*", "").Replace("*/", "").Trim();

            scriptContent = scriptContent.Remove(startIdx - startComment.Length, endIdx - startIdx + endComment.Length);
            scriptContent = scriptContent.Insert(startIdx - startComment.Length, startComment + "\n" + uncommentedCode + "\n");

            startIdx = scriptContent.IndexOf(startComment, startIdx + uncommentedCode.Length + endComment.Length);
        }

        return NormalizeLineEndings(scriptContent);
    }

    private void SaveScript(string scriptContent)
    {
        // Write the modified content back to the script file
        File.WriteAllText(scriptFilePath, scriptContent);

        // Re-import the script to apply changes in the editor
        AssetDatabase.ImportAsset(scriptFilePath);
        AssetDatabase.SaveAssets(); // Save changes to the asset database
    }

    private string NormalizeLineEndings(string scriptContent)
    {
        // Normalize line endings to LF
        return scriptContent.Replace("\r\n", "\n").Replace("\r", "\n");
    }
    //////////////////////////////////////////

}
#endif


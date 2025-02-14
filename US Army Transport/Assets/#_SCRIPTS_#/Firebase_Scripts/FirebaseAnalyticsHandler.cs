using UnityEngine;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Build;
#endif
#if Firebase_Rizwan
using Firebase.Analytics;
#endif


public class FirebaseAnalyticsHandler : MonoBehaviour
{
    public static FirebaseAnalyticsHandler Instance;
    [HideInInspector]
    public bool isInitialized = false;
    private void Awake()
    {
        if (Instance == null)
        { Instance = this; }
        else
        { Destroy(this); }

    }

    public void InitializeFirebase_Analytics()
    {
#if Firebase_Rizwan
        PrintStatus("Enabling data collection.", false);
        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

        PrintStatus("Set user properties.", false);
        // Set the user's sign up method.
        FirebaseAnalytics.SetUserProperty(FirebaseAnalytics.UserPropertySignUpMethod, "Google");
        // Set the user ID.
        //FirebaseAnalytics.SetUserId(UserID);
        // Set default session duration values.
        FirebaseAnalytics.SetSessionTimeoutDuration(new System.TimeSpan(0, 30, 0));
        isInitialized = true;
#endif
    }
    #region NewTypes
    public void LogFirebaseEvent(string eventName)
    {
#if Firebase_Rizwan
        //Debug.Log(FireBaseInitializer.Instance?._firebaseInitialized + " " + eventName);
        if (isInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName);
        }
#endif
    }
    public void LogFirebaseEvent(string eventName, string parameterName, string parameterValue)
    {
#if Firebase_Rizwan
        if (isInitialized)
        {
            FirebaseAnalytics.LogEvent(eventName, parameterName, parameterValue);
        }
#endif
    }


    public void LogFirebaseEvent_Group(string eventName, int totalParameters, string[] parameterName, string[] parameterValue)
    {
#if Firebase_Rizwan
        if (isInitialized)
        {
            var impressionParameters = new Firebase.Analytics.Parameter[totalParameters];

            for (int i = 0; i < totalParameters; i++)
            {
                impressionParameters[i] = new Firebase.Analytics.Parameter(parameterName[i], parameterValue[i]);
            }

            Firebase.Analytics.FirebaseAnalytics.LogEvent(eventName, impressionParameters);
        }
#endif
    }
    //This is a sample code 
    //void SampleCode_LogFirebaseEvent_Group()
    //{

    //    string eventName = "sample_event";
    //    int totalParameters = 2; // Example: assuming you have 2 parameters
    //    string[] parameterName = new string[totalParameters];
    //    string[] parameterValue = new string[totalParameters];

    //    // Fill in parameter names and values
    //    parameterName[0] = "parameter1_name";
    //    parameterValue[0] = "parameter1_value";

    //    parameterName[1] = "parameter2_name";
    //    parameterValue[1] = "parameter2_value";

    //    // Call the method
    //    LogFirebaseEvent_Group(eventName, totalParameters, parameterName, parameterValue);
    //}

    void PrintStatus(string _string, bool errorLog)
    {
        if (AdsController.Instance && AdsController.Instance.DebugMode)
        {
#if UNITY_EDITOR
            if (errorLog)
            {
                Debug.Log("<color=red><b>#ADMOB# </b></color> <b>" + _string + "</b> <color=red><b>#ADMOB# </b></color>");
            }
            else
            {
                Debug.Log("<color=green><b>#ADMOB# </b></color> <b>" + _string + "</b> <color=green><b>#ADMOB# </b></color>");
            }
#elif UNITY_ANDROID || UNITY_IOS
            Debug.Log(_string);
#endif
        }
    }
    #endregion
}

#if UNITY_EDITOR
[CustomEditor(typeof(FirebaseAnalyticsHandler))]
class FirebaseAnalyticsHandlerEditor : Editor
{
#if Firebase_Rizwan

#else
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please make sure Firebase Analytics SDK is imported into the project (recommended version 12.4.1)." +
           "Also make sure that Symbol Firebase_Rizwan is added in other Settings", MessageType.Error);

    }

    private void OnEnable()
    {
        CheckFirebaseAnalyticsFolder();
    }
    public static void CheckFirebaseAnalyticsFolder()
    {
        // Define the path to the "Assets/GoogleMobileAds" folder
        string FirebasePath = Path.Combine(Application.dataPath, "Firebase/Plugins/Firebase.Analytics.dll");

        // Check if the "GoogleMobileAds" folder exists
        if (File.Exists(FirebasePath))
        {
            Debug.Log("Debug: Firebase folder exists.");
            //EditorGUILayout.HelpBox("Updating Scripts, Please wait", MessageType.Info);
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
            UpdateSymbols("Firebase_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Firebase_Rizwan", true, targets);
#endif
        }
        else
        {
            Debug.Log("Debug: FirebasePath folder Does Not exists.");
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

}

#endif
using UnityEngine;
#if Remote_Config_Rizwan
using Firebase.Extensions;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;

#endif

#if Remote_Config_Rizwan
using Newtonsoft.Json;
#endif

public class FirebaseRemoteConfigHandler : MonoBehaviour
{
    public static FirebaseRemoteConfigHandler Instance { get; private set; }

    [System.Serializable]
    [SerializeField]
    class remoteData
    {
        public string key = string.Empty;
        public string value = string.Empty;
        public string defaultvalue = string.Empty;
        public enum Type { String, Number, Bool }
        public Type valueType = Type.String;
    }

    [SerializeField] remoteData[] RemoteData;

    internal Dictionary<string, object> dict = new Dictionary<string, object>();
    internal static Action RemoteConfigUpdated, RemoteConfigureFailed;
    internal bool initialized = false;

#if Remote_Config_Rizwan
    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnEnable()
    {
        RemoteConfigUpdated += LoadManual_RemoteData;
    }
    private void OnDisable()
    {
        RemoteConfigUpdated -= LoadManual_RemoteData;
    }
    void LoadManual_RemoteData()
    {
        // Check the type of value and perform appropriate actions
        for (int i = 0; i < RemoteData.Length; i++)
        {
            switch (RemoteData[i].valueType)
            {
                case remoteData.Type.String:
                    Instance.AddOrUpdateKey(RemoteData[i].key, RemoteData[i].value, RemoteData[i].defaultvalue);
                    break;
                case remoteData.Type.Number:
                    // If the value type is number, try to convert it into an integer
                    int intValue, dafaultintvalue;
                    if (int.TryParse(RemoteData[i].value, out intValue) && (int.TryParse(RemoteData[i].defaultvalue, out dafaultintvalue)))
                    {
                        Instance.AddOrUpdateKey_Number(RemoteData[i].key, (double)intValue, (double)dafaultintvalue);
                    }
                    else
                    {
                        PrintStatus($"Value is not a valid number at {RemoteData[i]}", true);
                    }
                    break;
                case remoteData.Type.Bool:
                    // If the value type is boolean, convert it into bool
                    bool boolValue, defaultVale;
                    if (bool.TryParse(RemoteData[i].value, out boolValue) && (bool.TryParse(RemoteData[i].defaultvalue, out defaultVale)))
                    {
                        Instance.AddOrUpdateKey_Bool(RemoteData[i].key, boolValue, defaultVale);
                    }
                    else
                    {
                        PrintStatus($"Value is not a valid Boolean at {RemoteData[i]}", true);
                    }
                    break;
                default:
                    Debug.LogError("Unknown value type.");
                    break;
            }

            Debug.Log("Processed value: " + RemoteData[i].value);
        }
    }



    internal void CheckInitilization()
    {
#if Admob_Simple_Rizwan && Max_Mediation_Rizwan
        if (AdmobManager.DictUpdated && MaxMediation.DictUpdated)
        {
            InitializeFirebase_RemoteConfig(dict);
        }
#elif Admob_Simple_Rizwan
        if (AdmobManager.DictUpdated)
        {
            InitializeFirebase_RemoteConfig(dict);
        }
#elif Max_Mediation_Rizwan
        if (MaxMediation.DictUpdated)
        {
            InitializeFirebase_RemoteConfig(dict);
        }
#else
        InitializeFirebase_RemoteConfig(dict);
#endif

    }
    public async void InitializeFirebase_RemoteConfig(Dictionary<string, object> dictionary)
    {
        // Wait for dependencies to be checked and resolved
        var dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            if (!initialized)
            {
                await Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(dictionary);
                await FetchDataAsync(); // Assuming FetchDataAsync is an async method
                initialized = true;
            }
        }
        else
        {
            UnityEngine.Debug.Log($"Could not resolve all Firebase dependencies RemoteConfig: {dependencyStatus}");
        }
    }

    private static Task FetchDataAsync()
    {
        PrintStatus("Fetching data...", false);
        var fetchTask =
            Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    private static void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
        {
            PrintStatus("Fetch canceled.", true);
        }
        else if (fetchTask.IsFaulted)
        {
            PrintStatus("Fetch encountered an error.", true);
        }
        else if (fetchTask.IsCompleted)
        {
            PrintStatus("Fetch completed successfully!", false);
        }

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync()
                    .ContinueWithOnMainThread(task =>
                    {
                        PrintStatus($"Remote data loaded and ready (last fetch time {info.FetchTime}).", false);
                        RemoteConfigUpdated?.Invoke();
                    });
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                RemoteConfigureFailed?.Invoke();
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        PrintStatus("****************Fetch failed for unknown reason*********", false);

                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:

                        PrintStatus("*****Fetch throttled until ****" + info.ThrottledEndTime, false);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:

                PrintStatus("Latest Fetch call still pending", true);

                break;
        }
    }


    public string AddOrUpdateKey(string key, string value, object defaultValue)
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig remoteConfig = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
        if (!remoteConfig.Keys.Contains(key))
        {
            remoteConfig.SetDefaultsAsync(new Dictionary<string, object> { { key, defaultValue } });
            remoteConfig.FetchAsync(TimeSpan.Zero)
                .ContinueWithOnMainThread(fetchTask =>
                {
                    if (fetchTask.IsCompleted && !fetchTask.IsFaulted && !fetchTask.IsCanceled)
                    {
                        remoteConfig.ActivateAsync()
                            .ContinueWithOnMainThread(activateTask =>
                            {
                                PrintStatus($"Key added to Remote Config: {key}", false);
                            });
                    }
                });
        }
        else
        {
            value = remoteConfig.GetValue(key).StringValue;
            PrintStatus($"Already have this key: {key}", false);

        }
        return value; // Return the updated value
    }


    public double AddOrUpdateKey_Number(string key, double value, double defaultValue)
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig remoteConfig = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
        if (!remoteConfig.Keys.Contains(key))
        {
            remoteConfig.SetDefaultsAsync(new Dictionary<string, object> { { key, defaultValue } });
            remoteConfig.FetchAsync(TimeSpan.Zero)
                .ContinueWithOnMainThread(fetchTask =>
                {
                    if (fetchTask.IsCompleted && !fetchTask.IsFaulted && !fetchTask.IsCanceled)
                    {
                        remoteConfig.ActivateAsync()
                            .ContinueWithOnMainThread(activateTask =>
                            {
                                PrintStatus($"Key added to Remote Config: {key}", false);
                            });
                    }
                });
        }
        else
        {
            value = remoteConfig.GetValue(key).DoubleValue;

        }
        return value; // Return the updated value
    }
    public bool AddOrUpdateKey_Bool(string key, bool value, bool defaultValue)
    {
        Firebase.RemoteConfig.FirebaseRemoteConfig remoteConfig = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance;
        if (!remoteConfig.Keys.Contains(key))
        {
            remoteConfig.SetDefaultsAsync(new Dictionary<string, object> { { key, defaultValue } });
            remoteConfig.FetchAsync(TimeSpan.Zero)
                .ContinueWithOnMainThread(fetchTask =>
                {
                    if (fetchTask.IsCompleted && !fetchTask.IsFaulted && !fetchTask.IsCanceled)
                    {
                        remoteConfig.ActivateAsync()
                            .ContinueWithOnMainThread(activateTask =>
                            {
                                PrintStatus($"Key added to Remote Config: {key}", false);
                            });
                    }
                });
        }
        else
        {
            value = remoteConfig.GetValue(key).BooleanValue;

        }
        return value; // Return the updated value
    }

#endif

    private static void PrintStatus(string _string, bool errorLog)
    {
        if (AdsController.Instance && AdsController.Instance.DebugMode)
        {
#if UNITY_EDITOR
            if (errorLog)
            {
                Debug.Log("<color=red><b>#REMOTE_CONFIG# </b></color> <b>" + _string + "</b> <color=red><b>#REMOTE_CONFIG# </b></color>");
            }
            else
            {
                Debug.Log("<color=green><b>#REMOTE_CONFIG# </b></color> <b>" + _string + "</b> <color=green><b>#REMOTE_CONFIG# </b></color>");
            }
#elif UNITY_ANDROID || UNITY_IOS

            Debug.Log(_string);

#endif
        }
    }
}





#if UNITY_EDITOR
[CustomEditor(typeof(FirebaseRemoteConfigHandler))]
public class FirebaseRemoteConfigHandlerEditor : Editor
{
#if Remote_Config_Rizwan
    private GUIStyle buttonStyle;
    private GUIStyleState normalState;
    private GUIStyleState hoverState;
    public override void OnInspectorGUI()
    {

        var firebaseremoteconfighandler = (FirebaseRemoteConfigHandler)target;
        if (firebaseremoteconfighandler == null)
        {
            // Debug.LogError("AdsController is null");
            return;
        }
        GUILayout.BeginVertical();
        // Create a new GUIStyle
        buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 15;
        buttonStyle.fixedHeight = 30;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.fontStyle = FontStyle.Bold;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.hover.textColor = Color.black;
        if (GUILayout.Button("Generate JSON", buttonStyle))
        {
            GenerateJsonFile();
        }

        //buttonStyle.normal.background = MakeTex(new Color(0.8584906f, 0.7468122f, 0.06884123f));
        if (GUILayout.Button("Find Json Location", buttonStyle))
        {
            FindJsonfileLocation();
        }
        //buttonStyle.normal.background = MakeTex(new Color(0.8584906f, 0.7468122f, 0.06884123f));
        if (GUILayout.Button("Find Json Local Location", buttonStyle))
        {
            FindRemoteConfigLocation();
        }
        if (GUILayout.Button("Delete Json Local Data", buttonStyle))
        {
            DeleteRemoteConfigFile();
        }
        GUILayout.EndVertical();
        //base.OnInspectorGUI();

        DrawDefaultInspector();


    }
    private Texture2D MakeTex(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        return texture;
    }

    private static void GenerateJsonFile()
    {
#if Admob_Simple_Rizwan
        AdmobManager admobManager = GameObject.FindObjectOfType<AdmobManager>();
#endif

#if Max_Mediation_Rizwan
        MaxMediation maxMediation = GameObject.FindObjectOfType<MaxMediation>();
#endif
        bool admobGenerated = false;
        bool maxGenerated = false;
#if Admob_Simple_Rizwan
        if (admobManager != null)
        {
            admobManager.JsonDataUpdate();

            Dictionary<string, object> admobDataDictionary = admobManager.tempdict;

            Dictionary<string, object> jsonDict = new Dictionary<string, object>();
            Dictionary<string, object> parametersDict = new Dictionary<string, object>();

            // Adding Admob parameters
            foreach (var kvp in admobDataDictionary)
            {
                Dictionary<string, object> parameterDict = new Dictionary<string, object>();
                parameterDict["defaultValue"] = new Dictionary<string, object> { { "value", kvp.Value } };
                parameterDict["valueType"] = "STRING";

                parametersDict[kvp.Key] = parameterDict;
            }

            jsonDict["parameters"] = parametersDict;

            string json = JsonConvert.SerializeObject(jsonDict, Formatting.Indented);

            //string json = JsonUtility.ToJson(jsonDict, false);
            // Define the folder path
            string folderPath = Path.Combine(Application.dataPath, "..", "RemoteConfig");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path within the folder
            string filePath = Path.Combine(folderPath, "data_admob.json");

            // Save the JSON to the file path
            File.WriteAllText(filePath, json);

            Debug.Log("Admob JSON file generated and saved at: " + filePath);
            admobGenerated = true;
        }
#endif
#if Max_Mediation_Rizwan
        if (maxMediation != null)
        {
            maxMediation.JsonDataUpdate();

            Dictionary<string, object> maxDataDictionary = maxMediation.tempdict;

            Dictionary<string, object> jsonDict = new Dictionary<string, object>();
            Dictionary<string, object> parametersDict = new Dictionary<string, object>();

            // Adding Max parameters
            foreach (var kvp in maxDataDictionary)
            {
                Dictionary<string, object> parameterDict = new Dictionary<string, object>();
                parameterDict["defaultValue"] = new Dictionary<string, object> { { "value", kvp.Value } };
                parameterDict["valueType"] = "STRING";

                parametersDict[kvp.Key] = parameterDict;
            }

            jsonDict["parameters"] = parametersDict;
            string json = JsonConvert.SerializeObject(jsonDict, Formatting.Indented);
            // string json = JsonSerializer.Serialize(jsonDict, options);
            //string json = JsonUtility.ToJson(jsonDict, true);

            // Define the folder path
            string folderPath = Path.Combine(Application.dataPath, "..", "RemoteConfig");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path within the folder
            string filePath = Path.Combine(folderPath, "data_max.json");

            // Save the JSON to the file path
            File.WriteAllText(filePath, json);

            Debug.Log("Max JSON file generated and saved at: " + filePath);
            maxGenerated = true;
        }
#if Admob_Simple_Rizwan
        if (admobManager != null && maxMediation != null)
        {
            Dictionary<string, object> jsonDict = new Dictionary<string, object>();
            Dictionary<string, object> parametersDict = new Dictionary<string, object>();

            // Adding Admob parameters
            admobManager.JsonDataUpdate();
            Dictionary<string, object> admobDataDictionary = admobManager.tempdict;


            foreach (var kvp in admobDataDictionary)
            {
                Dictionary<string, object> parameterDict = new Dictionary<string, object>();
                parameterDict["defaultValue"] = new Dictionary<string, object> { { "value", kvp.Value } };
                parameterDict["valueType"] = "STRING";

                parametersDict[kvp.Key] = parameterDict;
            }

            // Adding Max parameters
            maxMediation.JsonDataUpdate();

            Dictionary<string, object> maxDataDictionary = maxMediation.tempdict;

            foreach (var kvp in maxDataDictionary)
            {
                Dictionary<string, object> parameterDict = new Dictionary<string, object>();
                parameterDict["defaultValue"] = new Dictionary<string, object> { { "value", kvp.Value } };
                parameterDict["valueType"] = "STRING";

                parametersDict[kvp.Key] = parameterDict;
            }

            jsonDict["parameters"] = parametersDict;

            string json = JsonConvert.SerializeObject(jsonDict, Formatting.Indented);

            // Define the folder path
            string folderPath = Path.Combine(Application.dataPath, "..", "RemoteConfig");

            // Create the directory if it doesn't exist
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Define the file path within the folder
            string filePath = Path.Combine(folderPath, "data_combined.json");

            // Save the JSON to the file path
            File.WriteAllText(filePath, json);

            Debug.Log("Max JSON file generated and saved at: " + filePath);
            maxGenerated = true;
        }
#endif
#endif

        if (admobGenerated && !maxGenerated)
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string folderPath = Path.Combine(projectPath, "RemoteConfig");
            string filePath = Path.Combine(folderPath, "data_admob.json");
            EditorUtility.RevealInFinder(filePath);

        }
        else if (maxGenerated && !admobGenerated)
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string folderPath = Path.Combine(projectPath, "RemoteConfig");
            string filePath = Path.Combine(folderPath, "data_max.json");
            EditorUtility.RevealInFinder(filePath);

        }
        else if (maxGenerated && admobGenerated)
        {
            string projectPath = Directory.GetParent(Application.dataPath).FullName;
            string folderPath = Path.Combine(projectPath, "RemoteConfig");
            string filePath = Path.Combine(folderPath, "data_admob.json");
            EditorUtility.RevealInFinder(filePath);

        }

        else if (!admobGenerated && !maxGenerated)
        {
            EditorUtility.DisplayDialog("Scripts Missing", "Neither AdmobManager nor MaxMediation script found in the scene. Make sure both are attached to GameObjects.", "Ok");
        }
    }

    private static void FindJsonfileLocation()
    {
        string projectPath = Directory.GetParent(Application.dataPath).FullName;
        string folderPath = Path.Combine(projectPath, "RemoteConfig");
        string filePath = Path.Combine(folderPath, "data_admob.json");
        EditorUtility.RevealInFinder(filePath);

        if (File.Exists(filePath))
        {
            EditorUtility.RevealInFinder(filePath);
        }
        else
        {
            EditorUtility.DisplayDialog("File Not Found", $"Remote Config Json file not found at: {filePath}", "OK");
        }
    }

    private static void DeleteRemoteConfigFile()
    {
        string projectPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
#if UNITY_ANDROID
        string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#elif UNITY_IOS
        string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
#else
        Debug.LogError("Please switch your project to Android or iOS");
#endif
        string combinedpath = Path.Combine(projectPath, bundleId, "__FIRAPP_DEFAULT", "remote_config_data");
        if (File.Exists(combinedpath))
        {
            File.Delete(combinedpath);
        }
        else
        {
            EditorUtility.DisplayDialog("File Not Found", $"Remote Config Json file not found at: {combinedpath}", "OK");
        }
    }

    private static void FindRemoteConfigLocation()
    {
        string projectPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
#if UNITY_ANDROID
        string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#elif UNITY_IOS
        string bundleId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
#else
    Debug.LogError("Please switch your project to Android or iOS");
#endif
        string combinedpath = Path.Combine(projectPath, bundleId, "__FIRAPP_DEFAULT", "remote_config_data");
        if (File.Exists(combinedpath))
        {
            EditorUtility.RevealInFinder(combinedpath);
            Debug.Log($"File remote_config_data deleted at location {combinedpath}");
        }
        else
        {
            EditorUtility.DisplayDialog("File Not Found", $"Remote Config Json file not found at: {combinedpath}", "OK");
        }
    }

#else
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please make sure Firebase Remote Config SDK is imported into the project (recommended version 12.4.1)." +
             "Also make sure that Symbol Remote_Config_Rizwan is added in other Settings", MessageType.Error);
    }

    private void OnEnable()
    {
        CheckRemoteConfigFolder();
    }
    public static void CheckRemoteConfigFolder()
    {
        // Define the path to the "Assets/Firebase/Firebase.RemoteConfig" folder
        string firebaseRemoteConfigPath = Path.Combine(Application.dataPath, "Firebase/Plugins/Firebase.RemoteConfig.dll");

        if (File.Exists(firebaseRemoteConfigPath))
        {
            Debug.Log("Debug: Firebase RemoteConfig folder exists.");
            //EditorGUILayout.HelpBox("Updating Scripts, Please wait", MessageType.Info);
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
            UpdateSymbols("Remote_Config_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Remote_Config_Rizwan", true, targets);
#endif
        }
        else
        {
            Debug.Log("Debug: FirebaseRemoteConfigPath folder Does Not exists.");
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




using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System;
using System.Globalization;




#if UNITY_EDITOR
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build;
#endif

public class PluginManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(PluginManager))]
public class PluginManagerEditor : Editor
{
    private static PluginManager pluginManager = null;
    private static FireBaseInitializer firebaseInitializerComponent;
    private static FirebaseAnalyticsHandler firebaseAnalyticsComponent;
    private static FirebaseRemoteConfigHandler firebaseRemoteConfigComponent;

    private static InAppReview InAppReviewComponent;
    private static Transform inappReviewInstanceChild;
    private const string scriptFilePath_Review = "Assets/#_SCRIPTS_#/InApp_Review/Scripts/InAppReview.cs";
    private const string commentStart_Review = "//BEGIN_IN_APP_REVIEW";
    private const string commentEnd_Review = "//END_IN_APP_REVIEW";


    private static IAP_Controller InAppPurchaseComponent;
    private const string scriptFilePath_Purchase = "Assets/#_SCRIPTS_#/InApp_Purchases/Scripts/IAP_Controller.cs";
    private const string commentStart_Purchase = "//BEGIN_IN_APP_PURCHASE";
    private const string commentEnd_Purchase = "//END_IN_APP_PURCHASE";

    private static InAppUpdate InAppUpdateComponent;
    private const string scriptFilePath_Update = "Assets/#_SCRIPTS_#/InApp_Update/Scripts/InAppUpdate.cs";
    private const string commentStart_Update = "//BEGIN_IN_APP_UPDATE";
    private const string commentEnd_Update = "//END_IN_APP_UPDATE";

    private static bool attPermission = false;


    public Texture2D NormalTexture;
    public Texture2D HoverTexture;
    public Texture2D ActiveTexture;
    public Texture2D ActiveTexture2;
    private GUIStyle buttonStyle;
    private Texture2D CreateTexture(Color color)
    {
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, color);
        texture.Apply();
        texture.hideFlags = HideFlags.HideAndDontSave;
        return texture;
    }

    private void OnEnable()
    {
        CheckFirebaseAnalyticsFolder();
        ATT_PERMISSION();

        InAppReviewComponent = pluginManager.GetComponentInChildren<InAppReview>(true);
        InAppPurchaseComponent = pluginManager.GetComponentInChildren<IAP_Controller>(true);
        InAppUpdateComponent = pluginManager.GetComponentInChildren<InAppUpdate>(true);


    }
    private void Awake()
    {
        // Pre-create textures
        NormalTexture = CreateTexture(Color.gray);
        HoverTexture = CreateTexture(Color.yellow);
        ActiveTexture = CreateTexture(new Color(0f, 0.5f, 0f)); // Dark green
        ActiveTexture2 = CreateTexture(Color.red);
    }
    private void OnDisable()
    {
        ATT_PERMISSION();
    }
    public static void CheckFirebaseAnalyticsFolder()
    {
        // Define the path to the "Assets/GoogleMobileAds" folder
        string FirebasePath = Path.Combine(Application.dataPath, "Firebase/Plugins/Firebase.Analytics.dll");

        // Check if the "Firebase Analytics" folder exists
        if (File.Exists(FirebasePath))
        {
            //EditorGUILayout.HelpBox("Updating Scripts, Please wait", MessageType.Info);
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
            UpdateSymbols("Firebase_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Firebase_Rizwan", true, targets);
#endif
        }

        string firebaseRemoteConfigPath = Path.Combine(Application.dataPath, "Firebase/Plugins/Firebase.RemoteConfig.dll");
        // Check if the "firebaseRemoteConfigPath" folder exists
        if (File.Exists(firebaseRemoteConfigPath))
        {
            //EditorGUILayout.HelpBox("Updating Scripts, Please wait", MessageType.Info);
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
            UpdateSymbols("Remote_Config_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Remote_Config_Rizwan", true, targets);
#endif
        }
    }

    #region Plugin
    [MenuItem("Custom_Tools/Generate Plugin Manager", priority = 0)]
    public static void GeneratePluginManager()
    {
        if (EditorSceneManager.GetActiveScene().buildIndex > 0 || !EditorBuildSettings.scenes.Any(s => s.path == EditorSceneManager.GetActiveScene().path))
        {
            Debug.LogWarning("Active scene is not the first scene. Opening the scene with index 0.");
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (EditorBuildSettings.scenes.Length == 0)
            {
                EditorUtility.DisplayDialog("Scene in Build Empty", $"Please Add any scene in Build Settings", "Ok");
                return;
            }
            var firstScenePath = EditorBuildSettings.scenes[0].path;
            EditorSceneManager.OpenScene(firstScenePath);
        }
        // Check if the GameObject "PLUGIN MANAGER" already exists
        GameObject existingPluginManager = GameObject.Find("PLUGIN MANAGER");
        if (existingPluginManager != null)
        {
            // Check if it has the PluginManager component
            if (existingPluginManager.GetComponent<PluginManager>() != null)
            {
                Debug.Log("PLUGIN MANAGER GameObject with PluginManager component already exists.");
                return;
            }
            else
            {
                // If it doesn't have the PluginManager component, delete it
                Debug.LogWarning("PLUGIN MANAGER GameObject found but without PluginManager component. Deleting it.");
                GameObject.DestroyImmediate(existingPluginManager);
            }
        }

        // Check if PluginManager script exists
        var pluginManagerType = typeof(PluginManager);
        if (pluginManagerType == null)
        {
            Debug.LogError("PluginManager script not found in the project");
            return;
        }

        // Create a new GameObject for PLUGIN MANAGER
        GameObject pluginManagerGameObject = new GameObject("PLUGIN MANAGER");
        var pluginManagerComponent = (PluginManager)pluginManagerGameObject.AddComponent(pluginManagerType);
        if (pluginManagerComponent == null)
        {
            Debug.LogError("Failed to add Plugin Manager component to the GameObject");
            return;
        }

        // Mark the new GameObject as dirty to ensure changes are saved
        EditorUtility.SetDirty(pluginManagerGameObject);
        Debug.Log("PLUGIN MANAGER GameObject with PluginManager component created successfully.");
    }
    #endregion

    public override void OnInspectorGUI()
    {
        GUIStyle centeredStyle = new GUIStyle(GUI.skin.label);
        centeredStyle.alignment = TextAnchor.UpperRight;
        GUILayout.Label("Version: 3.0.0", centeredStyle, GUILayout.ExpandWidth(true));

        pluginManager = (PluginManager)target;
        if (pluginManager == null)
        {
            Debug.LogError("Plugin Manager is null");
            return;
        }
        if (PrefabUtility.IsPartOfPrefabInstance(pluginManager.gameObject))
        {
            // Unpack the prefab instance completely
            PrefabUtility.UnpackPrefabInstance(pluginManager.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
            Debug.Log("Prefab unpacked completely.");
        }
        GUILayout.BeginVertical();

        buttonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 15,
            fixedHeight = 40,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            padding = new RectOffset(10, 10, 10, 10)
        };

        // Set style states
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.normal.background = NormalTexture;

        buttonStyle.hover.textColor = Color.black;
        buttonStyle.hover.background = HoverTexture;

        buttonStyle.active.textColor = Color.white;
        buttonStyle.active.background = ActiveTexture2;

        //////////////////////////////////////////////////
        /////ADS CONTROLLER/////
        ///////////////////////////////////////////////
        #region ADS CONTROLLER
        var AdsControllerComponent = pluginManager.GetComponentInChildren<AdsController>(true);
        if (AdsControllerComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("ADS CONTROLLER", buttonStyle))
            {
                // Load the AdsController prefab from the specified path
                string adsControllerPath = "Assets/#_SCRIPTS_#/_ADS/Prefabs/ADS CONTROLLER.prefab";
                GameObject adsControllerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(adsControllerPath);
                if (adsControllerPrefab == null)
                {
                    EditorUtility.DisplayDialog("Prefab Missing", $"AdsController prefab not found at path: {adsControllerPath}.", "Ok");
                    return;
                }
                // Instantiate the prefab
                GameObject adsControllerInstance = (GameObject)PrefabUtility.InstantiatePrefab(adsControllerPrefab, pluginManager.transform);
                PrefabUtility.UnpackPrefabInstance(adsControllerInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                Transform adsControllerInstanceChild = adsControllerInstance.transform.GetChild(0);
                if (adsControllerInstance == null)
                {
                    Debug.LogError("Failed to instantiate AdsController prefab");
                    return;
                }

                var Canvas_Plugin = FindWithName(pluginManager.transform, "Canvas_Plugin");

                if (Canvas_Plugin == null)
                {
                    Debug.Log("Nhi Mil rha ");
                    string canvasPath = "Assets/#_SCRIPTS_#/Prefabs/Canvas_Plugin.prefab";
                    GameObject canvas_Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(canvasPath);

                    if (canvas_Prefab == null)
                    {
                        EditorUtility.DisplayDialog("Prefab Missing", $"Canvas_Review prefab not found at path: {canvasPath}.", "Ok");
                        return;
                    }
                    // Instantiate the prefab
                    GameObject CanvasInstance = (GameObject)PrefabUtility.InstantiatePrefab(canvas_Prefab, pluginManager.transform);
                    PrefabUtility.UnpackPrefabInstance(CanvasInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                    Canvas_Plugin = CanvasInstance.transform;
                }
                adsControllerInstanceChild.SetParent(Canvas_Plugin.transform);
                adsControllerInstanceChild.transform.SetAsLastSibling();
                adsControllerInstanceChild.transform.name = "AdsController Canvas Stuff";
                // Set the parent and sibling index
                adsControllerInstance.transform.SetParent(pluginManager.transform);
                adsControllerInstance.transform.SetSiblingIndex(0);
                RectTransform rectTransform = adsControllerInstanceChild.GetComponent<RectTransform>();

                // Stretch the RectTransform to fill its parent (set anchors to the edges)
                rectTransform.anchorMin = new Vector2(0, 0);  // Bottom-left corner
                rectTransform.anchorMax = new Vector2(1, 1);  // Top-right corner

                // Reset the pivot (optional, default is (0.5, 0.5))
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // Set sizeDelta to (0, 0) because we are stretching it to fill the parent
                rectTransform.sizeDelta = Vector2.zero;

                // Reset scale to (1, 1, 1)
                rectTransform.localScale = Vector3.one;

                // Optional: Set the RectTransform's edges (left, right, top, bottom) to 0
                rectTransform.offsetMin = Vector2.zero;  // Set left and bottom to 0
                rectTransform.offsetMax = Vector2.zero;  // Set right and top to 0


                EditorGUIUtility.PingObject(adsControllerInstance.transform);

                // Mark the PluginManager as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);

                Debug.Log("AdsController component added and linked successfully");
            }
        }
        else
        {
            buttonStyle.normal.background = ActiveTexture;

            if (GUILayout.Button("ADS CONTROLLER", buttonStyle))
            {
                //deleting admob and max folders, files and symbols
                string[] pathsToDelete = {
                 "Assets/GoogleMobileAds",
                "Assets/Plugins/Android/googlemobileads-unity.aar",
                "Assets/Plugins/Android/GoogleMobileAdsPlugin.androidlib",
                "Assets/Plugins/iOS/GADUAdNetworkExtras.h",
                "Assets/Plugins/iOS/unity-plugin-library.a",
                 "Assets/MaxSdk"
                    };
                DeletePluginFilesFolders(pathsToDelete);



                RemoveScriptingDefineSymbol("Admob_Simple_Rizwan");
                RemoveScriptingDefineSymbol("Admob_Mediation_Rizwan");
                RemoveScriptingDefineSymbol("Max_Mediation_Rizwan");
                RemoveScriptingDefineSymbol("UnityAds_Rizwan");

                //RemoveScriptingDefineSymbol("Firebase_Rizwan");
                //RemoveScriptingDefineSymbol("Remote_Config_Rizwan");

                RemoveScriptingDefineSymbol("gameanalytics_admob_enabled");
                RemoveScriptingDefineSymbol("gameanalytics_max_enabled");

                var AdsControllerComponentChild = FindWithName(pluginManager.transform, "AdsController Canvas Stuff");
                DestroyImmediate(AdsControllerComponent.gameObject);
                DestroyImmediate(AdsControllerComponentChild.gameObject);
                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);

                Debug.Log("AdsController component removed successfully");
            }
        }
        GUILayout.Space(10);
        #endregion


        ///////////////////////////////////////////////
        /////FIREBASE ANALYTICS/////
        ///////////////////////////////////////////////
        ///
        #region FIREBASE ANALYTICS

        //  firebaseInitializerComponent = pluginManager.GetComponentInChildren<FireBaseInitializer>(true);
        firebaseAnalyticsComponent = pluginManager.GetComponentInChildren<FirebaseAnalyticsHandler>(true);

        if (firebaseAnalyticsComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("FIREBASE ANALYTICS", buttonStyle))
            {
                FirebaseAnalytics_Import();
            }
        }
        else
        {
            //buttonStyle.normal.background = MakeTex(600, 1, new Color(0.1f, 0.6f, 0.8f));
            buttonStyle.normal.background = ActiveTexture;
            if (GUILayout.Button("FIREBASE ANALTICS", buttonStyle))
            {
                // Initialize components and wait for Unity Editor to refresh
                FirebaseAnalytics_Remove();
            }
        }

        void FirebaseAnalytics_Import()
        {
            // Check if the FireBaseInitializer script exists in the project
            var FireBaseInitializerType = typeof(FireBaseInitializer);
            if (FireBaseInitializerType == null)
            {
                Debug.LogError("FireBaseInitializer script not found in the project");
                return;
            }
            var FireBaseAnalyticsType = typeof(FirebaseAnalyticsHandler);
            if (FireBaseAnalyticsType == null)
            {
                Debug.LogError("FireBaseInitializer script not found in the project");
                return;
            }
            // Create a new GameObject named "FIREBASE ANALYTICS"
            //$ nhi lagaya to life jheenga lala
            GameObject FireBaseGameObject = new GameObject($"FIREBASE ANALYTICS");
            // Add the FIREBASE ANALYTICS component to the new GameObject
            firebaseInitializerComponent = (FireBaseInitializer)FireBaseGameObject.AddComponent(FireBaseInitializerType);
            firebaseAnalyticsComponent = (FirebaseAnalyticsHandler)FireBaseGameObject.AddComponent(FireBaseAnalyticsType);
            if (firebaseInitializerComponent == null)
            {
                Debug.LogError("Failed to add FIREBASE Inilizer component to the GameObject");
                return;
            }
            if (firebaseAnalyticsComponent == null)
            {
                Debug.LogError("Failed to add firebaseAnalytics component to the GameObject");
                return;
            }

            // Insert the new GameObject at the second position
            FireBaseGameObject.transform.SetParent(pluginManager.transform);
            FireBaseGameObject.transform.SetSiblingIndex(1);
            EditorGUIUtility.PingObject(FireBaseGameObject.transform);

            //#if UNITY_2021_1_OR_NEWER
            //            NamedBuildTarget[] targets = { NamedBuildTarget.Android, NamedBuildTarget.iOS };
            //            UpdateSymbols("Firebase_Rizwan", true, targets);
            //#else
            //                BuildTargetGroup[] targets = {BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
            //                UpdateSymbols("Firebase_Rizwan", true, targets);
            //#endif
            //adding max.unitypackage 
            //string packagePath = "Assets/#_SCRIPTS_#/DATA/FIREBASE/FirebaseAnalytics 11.6.0.unitypackage";
            //AssetDatabase.ImportPackage(packagePath, false);

            // Mark the AdsController as dirty to ensure changes are saved
            EditorUtility.SetDirty(pluginManager);

            Debug.Log("AdmobManager component added and linked successfully");
        }

        void FirebaseAnalytics_Remove()
        {
            string[] pathsToDelete = {
                    "Assets/Firebase",
                    "Assets/Plugins/Android/FirebaseApp.androidlib",
                    "Assets/Plugins/iOS/Firebase",
                    "Assets/Plugins/tvOS/Firebase",
                    "Assets/Editor Default Resources"
                };
            DeletePluginFilesFolders(pathsToDelete);

            RemoveScriptingDefineSymbol("Firebase_Rizwan");

            DestroyImmediate(firebaseAnalyticsComponent.gameObject);

            if (firebaseRemoteConfigComponent)
            {
                RemoveScriptingDefineSymbol("Remote_Config_Rizwan");
                DestroyImmediate(firebaseRemoteConfigComponent.gameObject);
            }
            // Mark the AdsController as dirty to ensure changes are saved
            EditorUtility.SetDirty(pluginManager);

            //Debug.Log("AdmobManager component removed successfully");
        }

        #endregion


        ///////////////////////////////////////////////
        /////FIREBASE REMOTE_CONFIG/////
        //////////////////////////////////////////////
        ///
        #region FIREBASE REMOTE_CONFIG

        GUILayout.Space(10);
        firebaseRemoteConfigComponent = pluginManager.GetComponentInChildren<FirebaseRemoteConfigHandler>(true);
        if (firebaseRemoteConfigComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("FIREBASE REMOTE_CONFIG", buttonStyle))
            {

                string packagestring = "com.unity.nuget.newtonsoft-json";
                if (!PackageExists(packagestring))
                {
                    AddPackage(packagestring);
                }
                //AddRequest request = Client.Add("com.unity.nuget.newtonsoft-json");

                //while (!request.IsCompleted)
                //{
                //    //wait jab tak load na ho jaey
                //}

                //if (request.Status == StatusCode.Success)
                //{
                //    Debug.Log("Package " + "InApp purchases" + " added successfully.");
                //}
                //else
                //{
                //    Debug.LogError("Failed to add package " + "InApp purchases" + ": " + request.Error.message);
                //}
                if (firebaseAnalyticsComponent == null) FirebaseAnalytics_Import();
                var FireBaseRemoteConfigType = typeof(FirebaseRemoteConfigHandler);
                if (FireBaseRemoteConfigType == null)
                {
                    Debug.LogError("firebaseRemoteConfig script not found in the project");
                    return;
                }
                //$ nhi lagaya to life jheenga lala
                GameObject FireBaseGameObject = new GameObject($"FIREBASE REMOTE_CONFIG");
                // Add the FIREBASE ANALYTICS component to the new GameObject
                firebaseRemoteConfigComponent = (FirebaseRemoteConfigHandler)FireBaseGameObject.AddComponent(FireBaseRemoteConfigType);
                if (firebaseRemoteConfigComponent == null)
                {
                    Debug.LogError("Failed to add FIREBASE Inilizer component to the GameObject");
                    return;
                }
                // Insert the new GameObject at the second position
                FireBaseGameObject.transform.SetParent(pluginManager.transform);
                FireBaseGameObject.transform.SetSiblingIndex(1);
                EditorGUIUtility.PingObject(FireBaseGameObject.transform);
#if Admob_Simple_Rizwan
                var admobManager = pluginManager.GetComponentInChildren<AdmobManager>(true);

                if (admobManager)
                {
#if Remote_Config_Rizwan

                    admobManager._firebaseRemoteConfigHandler = firebaseRemoteConfigComponent;
#endif
                }
#endif
#if Max_Mediation_Rizwan
                var maxMediation = pluginManager.GetComponentInChildren<MaxMediation>(true);
                if (maxMediation)
                {
                    maxMediation._fireBaseRemoteConfigHandler = firebaseRemoteConfigComponent;
                }
#endif
                //#if UNITY_2021_1_OR_NEWER
                //                NamedBuildTarget[] targets = { NamedBuildTarget.Android, NamedBuildTarget.iOS };
                //                UpdateSymbols("Remote_Config_Rizwan", true, targets);
                //#else
                //                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
                //                UpdateSymbols("Remote_Config_Rizwan", true, targets);
                //#endif

                //automatically add kernay k liay hay ye
                //adding max.unitypackage 
                //string packagePath = "Assets/#_SCRIPTS_#/DATA/FIREBASE/FirebaseRemoteConfig 11.6.0.unitypackage";
                //AssetDatabase.ImportPackage(packagePath, false);

                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);

                Debug.Log("AdmobManager component added and linked successfully");
            }
        }

        else
        {
            //buttonStyle.normal.background = MakeTex(600, 1, new Color(0.1f, 0.6f, 0.8f));
            buttonStyle.normal.background = ActiveTexture;
            if (GUILayout.Button("FIREBASE REMOTE_CONFIG", buttonStyle))
            {
                string packagestring = "com.unity.nuget.newtonsoft-json";
                if (PackageExists(packagestring))
                {
                    RemovePackage(packagestring);
                }

                //RemoveRequest request = Client.Remove("com.unity.nuget.newtonsoft-json");

                //while (!request.IsCompleted)
                //{
                //    //wait jab tak load na ho jaey
                //}
                //if (request.Status == StatusCode.Success)
                //{
                //    Debug.Log("Package " + "Advertisement Legacy" + " Removed successfully.");
                //}
                //else
                //{
                //    Debug.LogError("Failed to add package " + "InApp purchases" + ": " + request.Error.message);
                //}

                // Path to search for the file starting with 'FirebaseRemoteConfig_version'
                string editorPath = "Assets/Firebase/Editor/";

                // Get the full path of any file that starts with 'FirebaseRemoteConfig_version'
                string[] versionFiles = Directory.GetFiles(editorPath, "FirebaseRemoteConfig_version*", SearchOption.TopDirectoryOnly);

                // Create an array of paths that need to be deleted
                List<string> pathsToDelete = new List<string> {
        "Assets/Firebase/Plugins/Firebase.RemoteConfig.dll",
        "Assets/Firebase/Plugins/Firebase.RemoteConfig.pdb",
        "Assets/Firebase/Plugins/x86_64/FirebaseCppRemoteConfig.bundle",
        "Assets/Firebase/Plugins/x86_64/FirebaseCppRemoteConfig.dll",
        "Assets/Firebase/Plugins/x86_64/FirebaseCppRemoteConfig.so",
        "Assets/Firebase/Plugins/iOS/Firebase.RemoteConfig.dll",
        "Assets/Firebase/Plugins/iOS/Firebase.RemoteConfig.pdb",
        "Assets/Plugins/iOS/Firebase/libFirebaseCppRemoteConfig.a",
        "Assets/Plugins/tvOS/Firebase/libFirebaseCppRemoteConfig.a",
        "Assets/Firebase/m2repository/com/google/firebase/firebase-config-unity",
        "Assets/Firebase/Editor/RemoteConfigDependencies.xml"
    };

                // Add the found 'FirebaseRemoteConfig_version' files to pathsToDelete
                pathsToDelete.AddRange(versionFiles);

                // Ensure there are no duplicate paths
                pathsToDelete = pathsToDelete.Distinct().ToList();

                // Delete the specified files and folders
                DeletePluginFilesFolders(pathsToDelete.ToArray());

                // Remove the scripting define symbol
                RemoveScriptingDefineSymbol("Remote_Config_Rizwan");

                // Destroy the Firebase Remote Config component
                DestroyImmediate(firebaseRemoteConfigComponent.gameObject);

                // Optional: Handling additional packages (commented out)
                // string packagePath = "Assets/#_SCRIPTS_#/DATA/FIREBASE/FirebaseAnalytics 11.6.0.unitypackage";
                // AssetDatabase.ImportPackage(packagePath, false);

#if Admob_Simple_Rizwan
                var admobManager = pluginManager.GetComponentInChildren<AdmobManager>(true);
                if (admobManager)
                {
#if Remote_Config_Rizwan

                    admobManager._firebaseRemoteConfigHandler = null;
#endif
                }
#endif

#if Max_Mediation_Rizwan
    var maxMediation = pluginManager.GetComponentInChildren<MaxMediation>(true);
    if (maxMediation)
    {
        maxMediation._fireBaseRemoteConfigHandler = null;
    }
#endif

                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);

                Debug.Log("AdmobManager component removed successfully");
            }

        }

        #endregion


        ///////////////////////////////////////////////
        /////GAME_ANALYTICS/////
        ///////////////////////////////////////////////
        ///
        #region GAME_ANALYTICS

        GUILayout.Space(10);
        var gameAnalyticsComponent = pluginManager.GetComponentInChildren<GameAnalyticsInitializer>(true);

        if (gameAnalyticsComponent == null)
        {


            buttonStyle.normal.background = NormalTexture;
            GameObject gameAnalytics = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/GameAnalytics");
            if (AssetDatabase.IsValidFolder("Assets/GameAnalytics"))
            {
                EditorGUILayout.HelpBox("Click the GameAnalytics Button if you want to add, otherwise remove GameAnaltytics from project", MessageType.Warning);
            }
            else
            {
                if (analyticscliked)
                {
                    EditorGUILayout.HelpBox("Import GameAnalytics SDK before Activating it", MessageType.Info);
                }

            }

            if (GUILayout.Button("GAME_ANALYTICS", buttonStyle))
            {
                var gameAnalyticsType = typeof(GameAnalyticsInitializer);
                if (gameAnalyticsType == null)
                {

                    Debug.LogError("GameAnlytics script not found in the project");
                    return;
                }
                analyticscliked = true;
                //adding max.unitypackage      
                //string packagePath = "Assets/#_SCRIPTS_#/DATA/GAME_ANALYTICS/GA_SDK_UNITY 7.9.1.unitypackage";
                //AssetDatabase.ImportPackage(packagePath, false);

                string gameAnalyticspath = "Assets/GameAnalytics/Plugins/Prefabs/GameAnalytics.prefab";
                GameObject gameAnalyticsPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(gameAnalyticspath);
                if (gameAnalyticsPrefab == null)
                {
                    // EditorUtility.DisplayDialog("Prefab Missing", $"gameAnalytics prefab not found at path: {gameAnalyticspath}.", "Ok");
                    return;
                }
                // Instantiate the prefab
                GameObject gameAnalyticsInstance = (GameObject)PrefabUtility.InstantiatePrefab(gameAnalyticsPrefab, pluginManager.transform);
                PrefabUtility.UnpackPrefabInstance(gameAnalyticsInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                // Insert the new GameObject at the second position
                gameAnalyticsInstance.transform.SetParent(pluginManager.transform);
                gameAnalyticsInstance.transform.SetAsLastSibling();

                gameAnalyticsComponent = (GameAnalyticsInitializer)gameAnalyticsInstance.AddComponent(gameAnalyticsType);
                if (gameAnalyticsComponent == null)
                {
                    Debug.LogError("Failed to add GameAnalytics Inilizer component to the GameObject");
                    return;
                }

                EditorUtility.SetDirty(pluginManager);

                Debug.Log("GameAnalytics component added and linked successfully");
            }
        }
        else
        {
            buttonStyle.normal.background = ActiveTexture;
            if (GUILayout.Button("GAME_ANALYTICS", buttonStyle))
            {
                string[] pathsToDelete = {
                    "Assets/GameAnalytics",
                    "Assets/Resources/GameAnalytics"
                };
                DeletePluginFilesFolders(pathsToDelete);

                RemoveScriptingDefineSymbol("gameanalytics_admob_enabled");
                RemoveScriptingDefineSymbol("gameanalytics_max_enabled");
                analyticscliked = false;
                DestroyImmediate(gameAnalyticsComponent.gameObject);

                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);
                AssetDatabase.Refresh();

                Debug.Log("AdmobManager component removed successfully");
            }
        }
        #endregion


        //////////////////////////////////////////////////
        /////INAPP REVIEW/////
        ///////////////////////////////////////////////
        #region INAPP_REVIEW

        GUILayout.Space(10);

        if (InAppReviewComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("INAPP REVIEW", buttonStyle))
            {
                // Load the InAppReview prefab from the specified path             
                string InappReviewPath = "Assets/#_SCRIPTS_#/InApp_Review/Prefabs/InAPP_REVIEW.prefab";
                GameObject inappReviewPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(InappReviewPath);
                if (inappReviewPrefab == null)
                {
                    EditorUtility.DisplayDialog("Prefab Missing", $"InAppReview prefab not found at path: {InappReviewPath}.", "Ok");
                    return;
                }
                // Instantiate the prefab
                GameObject inappReviewInstance = (GameObject)PrefabUtility.InstantiatePrefab(inappReviewPrefab, pluginManager.transform);
                PrefabUtility.UnpackPrefabInstance(inappReviewInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                Transform inappReviewInstanceChild = inappReviewInstance.transform.GetChild(0);

                if (AdsControllerComponent != null)
                {
                    var Canvas_Plugin = FindWithName(pluginManager.transform, "Canvas_Plugin");

                    if (Canvas_Plugin == null)
                    {

                        Debug.Log("Nhi Mil gaya ");
                        string canvasPath = "Assets/#_SCRIPTS_#/Prefabs/Canvas_Plugin.prefab";
                        GameObject canvas_Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(canvasPath);

                        if (canvas_Prefab == null)
                        {
                            EditorUtility.DisplayDialog("Prefab Missing", $"Canvas_Review prefab not found at path: {canvasPath}.", "Ok");
                            return;
                        }
                        // Instantiate the prefab
                        GameObject CanvasInstance = (GameObject)PrefabUtility.InstantiatePrefab(canvas_Prefab, pluginManager.transform);
                        PrefabUtility.UnpackPrefabInstance(CanvasInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                        Canvas_Plugin = CanvasInstance.transform;

                    }
                    inappReviewInstanceChild.SetParent(Canvas_Plugin.transform);
                    inappReviewInstanceChild.transform.SetAsLastSibling();
                    inappReviewInstanceChild.transform.name = "InAppReview Dialogue Box";
                    //RectTransform rectTransform = inappReviewInstanceChild.GetComponent<RectTransform>();

                }
                else
                {
                    var Canvas_Plugin = FindWithName(pluginManager.transform, "Canvas_Plugin");

                    if (Canvas_Plugin == null)
                    {

                        Debug.Log("Nhi Mil gaya ");
                        string canvasPath = "Assets/#_SCRIPTS_#/Prefabs/Canvas_Plugin.prefab";
                        GameObject canvas_Prefab = AssetDatabase.LoadAssetAtPath<GameObject>(canvasPath);

                        if (canvas_Prefab == null)
                        {
                            EditorUtility.DisplayDialog("Prefab Missing", $"Canvas_Review prefab not found at path: {canvasPath}.", "Ok");
                            return;
                        }
                        // Instantiate the prefab
                        GameObject CanvasInstance = (GameObject)PrefabUtility.InstantiatePrefab(canvas_Prefab, pluginManager.transform);
                        PrefabUtility.UnpackPrefabInstance(CanvasInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                        Canvas_Plugin = CanvasInstance.transform;

                    }
                    inappReviewInstanceChild.SetParent(Canvas_Plugin.transform);
                    inappReviewInstanceChild.transform.SetAsLastSibling();
                    inappReviewInstanceChild.transform.name = "InAppReview Dialogue Box";
                    inappReviewInstance.GetComponent<RectTransform>();
                }
                RectTransform rectTransform = inappReviewInstanceChild.GetComponent<RectTransform>();
                // Set the anchor to the center of the screen
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);

                // Set the pivot to the center
                rectTransform.pivot = new Vector2(0.5f, 0.5f);

                // Reset the position and size
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;

                //PrefabUtility.UnpackPrefabInstance(inappReviewInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                InAppReviewComponent = inappReviewInstance.GetComponent<InAppReview>();

                UnCommentInScript(scriptFilePath_Review, commentStart_Review, commentEnd_Review);
                // Set the parent and sibling index
                inappReviewInstance.transform.SetParent(pluginManager.transform);
                inappReviewInstance.transform.SetAsLastSibling();


                EditorGUIUtility.PingObject(inappReviewInstance.transform);


                // Mark the PluginManager as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);

                Debug.Log("InAppReview component added and linked successfully");
            }
        }
        else
        {
            buttonStyle.normal.background = ActiveTexture;

            if (GUILayout.Button("INAPP REVIEW", buttonStyle))
            {

                //deleting review folders, files and symbols
                string[] pathsToDelete = {
                 "Assets/GooglePlayPlugins/com.google.play.review"
                // "Assets/MaxSdk"
                    };
                DeletePluginFilesFolders(pathsToDelete);

                var inappReviewComponentChild = FindWithName(pluginManager.transform, "InAppReview Dialogue Box");

                DestroyImmediate(InAppReviewComponent.gameObject);
                DestroyImmediate(inappReviewComponentChild.gameObject);


                CommentInScript(scriptFilePath_Review, commentStart_Review, commentEnd_Review);
                // Mark the InAppReview as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);
                Debug.Log("InAppReview component removed successfully");
            }
        }

        #endregion


        //////////////////////////////////////////////////
        /////INAPP PURCHASE/////
        ///////////////////////////////////////////////
        ///
        #region INAPP_PURCHASE

        GUILayout.Space(10);
        if (InAppPurchaseComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("INAPP PURCHASE", buttonStyle))
            {

                // Load the InAppPurchase prefab from the specified path             
                string InAppPurchasePath = "Assets/#_SCRIPTS_#/InApp_Purchases/Prefabs/IAP_Controller.prefab";
                GameObject InAppPurchasePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(InAppPurchasePath);
                if (InAppPurchasePrefab == null)
                {
                    EditorUtility.DisplayDialog("Prefab Missing", $"InAppPurchase prefab not found at path: {InAppPurchasePath}.", "Ok");
                    return;
                }
                // Instantiate the prefab
                GameObject InAppPurchaseInstance = (GameObject)PrefabUtility.InstantiatePrefab(InAppPurchasePrefab, pluginManager.transform);

                PrefabUtility.UnpackPrefabInstance(InAppPurchaseInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                InAppPurchaseComponent = InAppPurchaseInstance.GetComponent<IAP_Controller>();

                UnCommentInScript(scriptFilePath_Purchase, commentStart_Purchase, commentEnd_Purchase);
                // Set the parent and sibling index
                InAppPurchaseInstance.transform.SetParent(pluginManager.transform);
                InAppPurchaseInstance.transform.SetAsLastSibling();
                EditorGUIUtility.PingObject(InAppPurchaseInstance.transform);



                string packagestring = "com.unity.purchasing";
                if (!PackageExists(packagestring))
                {
                    AddPackage(packagestring);
                }

                //AddRequest request = Client.Add("com.unity.purchasing");
                //while (!request.IsCompleted)
                //{
                //    //wait jab tak load na ho jaey
                //}

                //if (request.Status == StatusCode.Success)
                //{
                //    Debug.Log("Package " + "InApp purchases" + " added successfully.");
                //}
                //else
                //{
                //    Debug.LogError("Failed to add package " + "InApp purchases" + ": " + request.Error.message);
                //}



                // Mark the PluginManager as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);

                Debug.Log("InAppPurchase component added and linked successfully");
            }
        }
        else
        {
            buttonStyle.normal.background = ActiveTexture;

            if (GUILayout.Button("INAPP PURCHASE", buttonStyle))
            {

                DestroyImmediate(InAppPurchaseComponent.gameObject);
                string packagestring = "com.unity.purchasing";
                if (PackageExists(packagestring))
                {
                    RemovePackage(packagestring);
                }

                //RemoveRequest request = Client.Remove("com.unity.purchasing");
                //while (!request.IsCompleted)
                //{
                //    //wait krn jab tak request pori na ho
                //}
                //if (request.Status == StatusCode.Success)
                //{
                //    Debug.Log("Package " + "InApp Purchasing" + " Removed successfully.");
                //}
                //else
                //{
                //    Debug.LogError("Failed to add package " + "InApp Purchasing" + ": " + request.Error.message);
                //}

                CommentInScript(scriptFilePath_Purchase, commentStart_Purchase, commentEnd_Purchase);
                // Mark the InAppPurchase as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);
                Debug.Log("InAppPurchasing component removed successfully");
            }
        }

        #endregion

        //////////////////////////////////////////////////
        /////INAPP UPDATE/////
        ///////////////////////////////////////////////
        ///
        #region INAPP_UPDATE

        GUILayout.Space(10);
        if (InAppUpdateComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("INAPP UPDATE", buttonStyle))
            {
                // Load the InAppUpdate prefab from the specified path             
                string InAppUpdatePath = "Assets/#_SCRIPTS_#/InApp_Update/Prefabs/InAPP_UPDATE.prefab";
                GameObject InAppUpdatePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(InAppUpdatePath);
                if (InAppUpdatePrefab == null)
                {
                    EditorUtility.DisplayDialog("Prefab Missing", $"InAppUpdate prefab not found at path: {InAppUpdatePath}.", "Ok");
                    return;
                }
                // Instantiate the prefab
                GameObject InAppUpdateInstance = (GameObject)PrefabUtility.InstantiatePrefab(InAppUpdatePrefab, pluginManager.transform);

                PrefabUtility.UnpackPrefabInstance(InAppUpdateInstance.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
                InAppUpdateComponent = InAppUpdateInstance.GetComponent<InAppUpdate>();

                UnCommentInScript(scriptFilePath_Update, commentStart_Update, commentEnd_Update);
                // Set the parent and sibling index
                InAppUpdateInstance.transform.SetParent(pluginManager.transform);
                InAppUpdateInstance.transform.SetAsLastSibling();
                EditorGUIUtility.PingObject(InAppUpdateInstance.transform);
                // Mark the PluginManager as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);
                Debug.Log("InAppUpdate component Added successfully");
            }
        }
        else
        {
            buttonStyle.normal.background = ActiveTexture;

            if (GUILayout.Button("INAPP UPDATE", buttonStyle))
            {
                //deleting review folders, files and symbols
                string[] pathsToDelete = {
                 "Assets/GooglePlayPlugins/com.google.play.appupdate"
                // "Assets/MaxSdk"
                    };
                DeletePluginFilesFolders(pathsToDelete);

                DestroyImmediate(InAppUpdateComponent.gameObject);

                CommentInScript(scriptFilePath_Update, commentStart_Update, commentEnd_Update);
                // Mark the InAppUpdate as dirty to ensure changes are saved
                EditorUtility.SetDirty(pluginManager);
                Debug.Log("InAppUpdate component removed successfully");
            }
        }
        GUILayout.Space(10);
        #endregion
        Repaint();
        GUILayout.EndVertical();
    }
    bool analyticscliked = false;
    private void ATT_PERMISSION()
    {
        ///////////////////////////////////////////////
        /////AttPermissionRequest/////
        ///////////////////////////////////////////////
        #region ATT_PermissionRequest
        pluginManager = (PluginManager)target;
        if (pluginManager == null)
        {
            //Debug.LogError("pluginManager is null. Make sure it is assigned in the inspector or initialized properly.");
            return;
        }

        AttPermissionRequest attPermissionRequestComponent = pluginManager.GetComponentInChildren<AttPermissionRequest>(true);
#if UNITY_IOS || UNITY_IPHONE
        if (attPermissionRequestComponent == null)
        {
         string packagestring = "com.unity.ads.ios-support";
   if (!PackageExists(packagestring))
 {
     AddPackage(packagestring);
 }
            var attPermissionType = typeof(AttPermissionRequest);
            if (attPermissionType == null)
            {
                Debug.LogError("attPermission script not found in the project");
                return;
            }
            // Create a new GameObject named "FIREBASE ANALYTICS"
            //$ nhi lagaya to life jheenga lala
            GameObject AttPermissionGameObject = new GameObject($"ATTPermissionRequest");
            // Add the FIREBASE ANALYTICS component to the new GameObject
            attPermissionRequestComponent = (AttPermissionRequest)AttPermissionGameObject.AddComponent(attPermissionType);
            if (attPermissionRequestComponent == null)
            {
                Debug.LogError("Failed to add FIREBASE Inilizer component to the GameObject");
                return;
            }
            // Insert the new GameObject at the second position
            AttPermissionGameObject.transform.SetParent(pluginManager.transform);
            AttPermissionGameObject.transform.SetSiblingIndex(1);
            EditorGUIUtility.PingObject(AttPermissionGameObject.transform);

            EditorUtility.SetDirty(pluginManager);

            Debug.Log("Att Permison component added and linked successfully");
        }
#elif UNITY_ANDROID
        if (attPermissionRequestComponent != null)
        {
            DestroyImmediate(attPermissionRequestComponent.gameObject);
            // Mark the AdsController as dirty to ensure changes are saved
            EditorUtility.SetDirty(pluginManager);
            Debug.Log("AdmobManager component removed successfully");

            string packagestring = "com.unity.purchasing";
            if (PackageExists(packagestring))
            {
                RemovePackage(packagestring);
            }

        }

#endif
        #endregion
        ///////////////////////////////////////////////////////
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
    private void DeletePluginFilesFolders(string[] pathsToDelete)
    {
        foreach (string path in pathsToDelete)
        {
            if (File.Exists(path))
            {
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
                //File.Delete(path);
                Debug.Log($"{path} file deleted successfully.");
            }
            else if (Directory.Exists(path))
            {
                // Directory.Delete(path, true);
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
                Debug.Log($"{path} folder deleted successfully.");
            }
            else
            {
                Debug.LogWarning($"{path} not found.");
            }
        }

        AssetDatabase.Refresh();
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

    private Texture2D MakeTex(int width, int height, Color color)
    {
        // Create a new Texture2D with the given dimensions
        Texture2D texture = new Texture2D(width, height);

        // Fill the texture with the specified color
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        // Apply the pixel data to the texture
        texture.SetPixels(pixels);
        texture.Apply();

        // Make the texture non-readable to optimize memory usage (optional)
        texture.hideFlags = HideFlags.HideAndDontSave;
        texture.wrapMode = TextureWrapMode.Clamp; // Avoid texture tiling

        return texture;
    }


    private void CommentInScript(string scriptfilepath, string commentstart, string commentend)
    {
        string script = File.ReadAllText(scriptfilepath);
        // Comment the block of code
        script = CommentCodeBlock(scriptfilepath, commentstart, commentend);

        // Write the changes back to the file
        SaveScript(scriptfilepath, script);
    }
    private void UnCommentInScript(string scriptfilepath, string commentstart, string commentend)
    {
        string script = File.ReadAllText(scriptfilepath);
        // Uncomment the code
        script = UncommentCodeBlock(scriptfilepath, commentstart, commentend);

        // Write the changes back to the file
        SaveScript(scriptfilepath, script);
    }

    private string CommentCodeBlock(string scriptfilepath, string commentstart, string commentend)
    {
        string scriptcontent = File.ReadAllText(scriptfilepath);

        int startIdx = scriptcontent.IndexOf(commentstart);

        while (startIdx != -1)
        {
            int endIdx = scriptcontent.IndexOf(commentend, startIdx);
            if (endIdx == -1) break; // If no matching end comment is found, exit loop

            // Extract the code to comment out
            startIdx += commentstart.Length; // Move to the content after commentStart
            string codeToComment = scriptcontent.Substring(startIdx, endIdx - startIdx).Trim();

            // Commenting each line with /* ... */
            string commentedCode = "/*\n" + codeToComment + "\n*/";

            // Replace the section with commented code
            scriptcontent = scriptcontent.Remove(startIdx - commentstart.Length, endIdx - startIdx + commentend.Length);
            scriptcontent = scriptcontent.Insert(startIdx - commentstart.Length, commentstart + "\n" + commentedCode + "\n");

            // Search for the next occurrence of the start marker
            startIdx = scriptcontent.IndexOf(commentstart, startIdx + commentedCode.Length + commentend.Length);
        }

        return NormalizeLineEndings(scriptcontent); // Normalize line endings
    }

    private string UncommentCodeBlock(string scriptfilepath, string commentstart, string commentend)
    {
        string scriptcontent = File.ReadAllText(scriptfilepath);
        int startIdx = scriptcontent.IndexOf(commentstart);

        while (startIdx != -1)
        {
            int endIdx = scriptcontent.IndexOf(commentend, startIdx);
            if (endIdx == -1) break; // If no matching end comment is found, exit loop

            // Extract the commented code
            startIdx += commentstart.Length; // Move to the content after commentStart
            string commentedBlock = scriptcontent.Substring(startIdx, endIdx - startIdx);

            // Remove comment markers
            string uncommentedCode = commentedBlock.Replace("/*", "").Replace("*/", "").Trim();

            // Reassemble the script with uncommented lines
            scriptcontent = scriptcontent.Remove(startIdx - commentstart.Length, endIdx - startIdx + commentend.Length);
            scriptcontent = scriptcontent.Insert(startIdx - commentstart.Length, commentstart + "\n" + uncommentedCode + "\n");

            // Search for the next occurrence of the start marker
            startIdx = scriptcontent.IndexOf(commentstart, startIdx + uncommentedCode.Length + commentend.Length);
        }

        return NormalizeLineEndings(scriptcontent); // Normalize line endings
    }

    private void SaveScript(string scriptfilepath, string scriptcontent)
    {
        //Write the modified content back to the script file
        File.WriteAllText(scriptfilepath, scriptcontent);

        // Re-import the script to apply changes in the editor
        AssetDatabase.ImportAsset(scriptfilepath);
        AssetDatabase.SaveAssets(); // Save changes to the asset database
        AssetDatabase.Refresh();
    }

    private string NormalizeLineEndings(string scriptcontent)
    {
        // Normalize line endings to LF
        return scriptcontent.Replace("\r\n", "\n").Replace("\r", "\n");
    }

    Transform FindWithName(Transform parent, string name)
    {
        if (parent.name == name) return parent;

        foreach (Transform child in parent)
        {
            Transform result = FindWithName(child, name);
            if (result != null) return result;
        }
        return null;
    }
    //Transform FindWithName(Transform parent, string name) =>parent.GetComponentsInChildren<Transform>(true).FirstOrDefault(child => child.name == name);


    private static bool PackageExists(string packageName)
    {
        var request = Client.List(true);
        while (!request.IsCompleted) System.Threading.Thread.Sleep(100);

        if (request.Status == StatusCode.Success)
        {
            return request.Result.Any(p => p.name == packageName);
        }
        else
        {
            Debug.LogError($"Failed to list packages: {request.Error.message}");
            return false;
        }
    }

    private static void AddPackage(string packageName)
    {
        // Add the package
        AddRequest request = Client.Add(packageName);

        // Wait for the request to complete
        while (!request.IsCompleted)
        {
            //System.Threading.Thread.Sleep(100);
        }

        // Check if the request was successful
        if (request.Status == StatusCode.Success)
        {
            Debug.Log($"Package '{packageName}' added successfully.");
        }
        else
        {
            Debug.LogError($"Failed to add package '{packageName}': {request.Error.message}");
        }
    }

    private static void RemovePackage(string packageName)
    {
        // Remove the package
        RemoveRequest request = Client.Remove(packageName);

        // Wait for the request to complete
        while (!request.IsCompleted)
        {
            //System.Threading.Thread.Sleep(100);
        }

        // Check if the request was successful
        if (request.Status == StatusCode.Success)
        {
            Debug.Log($"Package '{packageName}' removed successfully.");
        }
        else
        {
            Debug.LogError($"Failed to remove package '{packageName}': {request.Error.message}");
        }
    }
}
#endif





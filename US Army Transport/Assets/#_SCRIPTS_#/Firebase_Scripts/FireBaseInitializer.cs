using UnityEngine;
#if Firebase_Rizwan
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
#endif
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FireBaseInitializer : MonoBehaviour
{
    public static FireBaseInitializer Instance;
#if Firebase_Rizwan
    private DependencyStatus _dependencyStatus = DependencyStatus.UnavailableOther;
#endif
    internal bool _firebaseInitialized = false;

    //public FireBaseConfig fireBaseConfig;
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
        OnFireBase();
    }



    private void OnFireBase()
    {
#if Firebase_Rizwan
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            _dependencyStatus = task.Result;
            if (_dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                    "Could not resolve all Firebase dependencies: " + _dependencyStatus);
            }
        });

#endif
    }
    private void InitializeFirebase()
    {
        _firebaseInitialized = true;
#if Firebase_Rizwan
        FirebaseAnalyticsHandler.Instance.InitializeFirebase_Analytics();
#endif

    }
}

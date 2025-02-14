using UnityEngine;

#if gameanalytics_admob_enabled
using GameAnalyticsSDK;
#endif

#if gameanalytics_admob_enabled && UNITY_ANDROID
public class GameAnalyticsInitializer : MonoBehaviour
#elif gameanalytics_max_enabled && UNITY_IOS
public class GameAnalyticsInitializer : MonoBehaviour, IGameAnalyticsATTListener
#else
public class GameAnalyticsInitializer : MonoBehaviour
#endif
{
    void Start()
    {
#if gameanalytics_admob_enabled
#if UNITY_IOS
        GameAnalytics.RequestTrackingAuthorization(this);
#elif UNITY_ANDROID
        GameAnalytics.Initialize();
#endif
#endif
    }

#if gameanalytics_max_enabled && UNITY_IOS
    // Implementing IGameAnalyticsATTListener methods
    public void GameAnalyticsATTListenerNotDetermined()
    {
        GameAnalytics.Initialize();
    }

    public void GameAnalyticsATTListenerRestricted()
    {
        GameAnalytics.Initialize();
    }

    public void GameAnalyticsATTListenerDenied()
    {
        GameAnalytics.Initialize();
    }

    public void GameAnalyticsATTListenerAuthorized()
    {
        GameAnalytics.Initialize();
    }
#endif
}

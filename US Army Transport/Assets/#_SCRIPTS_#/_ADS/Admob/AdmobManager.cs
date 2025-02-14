#if Admob_Simple_Rizwan
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using GoogleMobileAds.Api.AdManager;
#endif
#if gameanalytics_admob_enabled
using GameAnalyticsSDK;
#endif
using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Build;
#endif
public enum BannerAdSize
{
    Banner = 0,
    MediumRectangle = 1,
    IABBanner = 2,
    Leaderboard = 3,
    adaptiveSize = 4,
    adaptiveCustomSize_500 = 5,
    collapsible = 6
}
public class AdmobManager : MonoBehaviour
{
#if Admob_Simple_Rizwan
#if Remote_Config_Rizwan
    public FirebaseRemoteConfigHandler _firebaseRemoteConfigHandler;
#endif

    public static bool DictUpdated = false;
    [Tooltip("Controller for the Google User Messaging Platform (UMP) Unity plugin.")]
    public GoogleMobileAdsConsentController _consentController;

    #region Other Stuff
    internal Action FullScreenAd_Shown;
    internal Action FullScreenAd_Closed;

    public bool _UseMediation = false;

    public bool _UseRemoteConfig = false;

    // Google User Messaging Platform (UMP) Unity plugin.
    [Space(20)]
    internal bool _TestMode = true;

    [Space(20)]
    [Header("LOW DEVICE EXEMPT")]
    [SerializeField, Tooltip("if value = false,minimum ram limited check will be neglected")]
    private bool StopShowingAdsOnLowEndDevices = false;
    [SerializeField, Tooltip("1=1GB, 1.5=1.5GB")]
    private int minRamSize_GB = 1;

    private float deviceRam = 0;
    private bool lowRam = false;
    public Dictionary<string, object> tempdict;

    //[Space(20)]
    //[Range(0, 1)]
    //public float _VideoAdsSoundVolume = 0.5f;

    #endregion

    #region UserConsent
    // The Google Mobile Ads Unity plugin needs to be run only once.
    private static bool? _isInitialized;
    [Space(20)]
    #endregion

    int bannerInc = 0;
    #region  Banner
    [Tooltip("Warning:Keeping it true,may cause Match Rate Fall")]
    [SerializeField] bool HideBannerWhenShowingFullScreenAd = false;



    [Space(20)]
    //BEGIN_ADMOB
    public BannerView[] _bannerViews;
    //END_ADMOB

    //BEGIN_ADMANAGER
    /*
    public AdManagerBannerView[] _bannerViews;
    */
    //END_ADMANAGER 




    internal bool[] _bannerLoaded;
    internal bool[] _bannerShowing;
    internal bool[] _bannerShowingForAllADs;

    private string[] _BannerAdId;


    [Space(10)]
    public _bannerAd[] _BannerAdUnit;
    [System.Serializable]
    public class _bannerAd
    {
        public string ANDROID;
        public string IOS;
        public AdPosition _AdPosition = AdPosition.Top;

        public enum BannerAdSize
        {
            Banner = 0,
            MediumRectangle = 1,
            IABBanner = 2,
            Leaderboard = 3,
            adaptiveSize = 4,
            adaptiveCustomSize_500 = 5,
            collapsible = 6
        }
        public BannerAdSize adSize = BannerAdSize.Banner;
        private AdSize _adsize;
        public AdSize _AdSize
        {
            get
            {
                // Convert the selected adSizeEnum to AdSize
                switch (adSize)
                {
                    case BannerAdSize.Banner:
                        _adsize = AdSize.Banner;
                        break;
                    case BannerAdSize.MediumRectangle:
                        _adsize = AdSize.MediumRectangle;
                        break;
                    case BannerAdSize.IABBanner:
                        _adsize = AdSize.IABBanner;
                        break;
                    case BannerAdSize.Leaderboard:
                        _adsize = AdSize.Leaderboard;
                        break;
                    case BannerAdSize.adaptiveSize:
                        _adsize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                        break;
                    case BannerAdSize.adaptiveCustomSize_500:
                        _adsize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(500);
                        break;
                    case BannerAdSize.collapsible:
                        _adsize = AdSize.Banner;
                        break;

                }
                return _adsize;
            }
        }

    }

    #endregion

    #region Interstitial
    //BEGIN_ADMOB
    internal InterstitialAd[] _interstitialAd;
    //END_ADMOB

    //BEGIN_ADMANAGER
    /*
    internal AdManagerInterstitialAd[] _interstitialAd;
    */
    //END_ADMANAGER
    private string[] InterstitialAdId;

    private bool isShowingInterstitial = false;
    [Space(20)]
    [Header("INTERSTITIAL ADs")]
    public _InterstitialAd[] _InterstitialAdUnit;

    [System.Serializable]
    public class _InterstitialAd
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;
    }

    #endregion

    #region Rewarded

    internal RewardedAd[] _rewardedAd;
    internal string[] RewardedAdId;
    private bool isShowingRewarded = false;
    [Space(10)]
    [Header("REWARDED ADs")]
    public _RewardedAd[] _RewardedAdUnit;
    [System.Serializable]
    public class _RewardedAd
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;
    }

    #endregion

    #region RewardedInterstital
    internal RewardedInterstitialAd[] _rewardedInterstitialAd;
    internal string[] RewardedInterstitialAdId;
    private bool isShowingRewardedInterstitial = false;

    [Space(10)]
    [Header("REWARDED INTERSTITIAL ADs")]
    public _RewardedInterstitalAd[] _RewardedInterstitalAdUnit;
    [System.Serializable]
    public class _RewardedInterstitalAd
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;

    }

    #endregion

    #region AppOpen

    private readonly TimeSpan TIMEOUT = TimeSpan.FromHours(4);
    private DateTime _expireTime;

    internal AppOpenAd _appOpenAd;

    internal string _AppOpenAdId;

    private bool isShowingAppopen = false;
    [Space(20)]
    [Header("APPOPEN ADs")]
    [SerializeField] private bool CanShowAppOpen = true;
    [SerializeField] private bool CanShowAppOpenAtStartup = true;
    [SerializeField] bool HideTopBannersWhenShowingAppOpen = false;

    //  [SerializeField, Range(0, 20)] private int SecondsLimitToShowAppOpen = 5;
    public _appOpen _AppOpenAdUnit;
    [System.Serializable]
    public class _appOpen
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;
        //public ScreenOrientation _ScreenOrientation = ScreenOrientation.LandscapeLeft;
    }
    #endregion

    //    // Always use test ads.
    //    // https://developers.google.com/admob/unity/test-ads
    //internal static List<string> TestDeviceIds = new List<string>()
    //    {
    //            AdRequest.TestDeviceSimulator,
    //#if UNITY_IPHONE
    //            "96e23e80653bb28980d3f40beb58915c",
    //#elif UNITY_ANDROID
    //            "702815ACFC14FF222DA1DC767672A573"
    //#endif
    //};

    private void OnEnable()
    {
#if Remote_Config_Rizwan
        FirebaseRemoteConfigHandler.RemoteConfigUpdated += RemoteConfigUpdate;
#endif
    }
    private void OnDisable()
    {
#if Remote_Config_Rizwan
        FirebaseRemoteConfigHandler.RemoteConfigUpdated -= RemoteConfigUpdate;
#endif
    }
    //private void Awake()
    //{
    //    AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    //}

    private void OnDestroy()
    {
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }

    internal void StartFunctions()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        if (_UseRemoteConfig && !_TestMode)
        {
#if Remote_Config_Rizwan
            StartStuff();
            StartRemoteConfigAdUnits();
            DictUpdated = true;
            _firebaseRemoteConfigHandler.CheckInitilization();
#else
            PrintStatus("Import Firebase RemoteConfig", true);
#endif
        }
        else
        {
            StartStuff();
            InitiliazeStuff();
        }


    }
    private void StartRemoteConfigAdUnits()
    {

#if Remote_Config_Rizwan

        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            _firebaseRemoteConfigHandler.dict.Add("Admob_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
            _firebaseRemoteConfigHandler.dict.Add("Admob_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());
        }
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            _firebaseRemoteConfigHandler.dict.Add("Admob_Interstitial_ANDROID_" + i, _InterstitialAdUnit[i].ANDROID.Trim());
            _firebaseRemoteConfigHandler.dict.Add("Admob_Interstitial_IOS_" + i, _InterstitialAdUnit[i].IOS.Trim());
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            _firebaseRemoteConfigHandler.dict.Add("Admob_Rewarded_ANDROID_" + i, _RewardedAdUnit[i].ANDROID.Trim());
            _firebaseRemoteConfigHandler.dict.Add("Admob_Rewarded_IOS_" + i, _RewardedAdUnit[i].IOS.Trim());
        }
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
            _firebaseRemoteConfigHandler.dict.Add("Admob_RewardedInterstital_ANDROID_" + i, _RewardedInterstitalAdUnit[i].ANDROID.Trim());
            _firebaseRemoteConfigHandler.dict.Add("Admob_RewardedInterstital_IOS_" + i, _RewardedInterstitalAdUnit[i].IOS.Trim());
        }
        _firebaseRemoteConfigHandler.dict.Add("Admob_AppOpen_ANDROID", _AppOpenAdUnit.ANDROID.Trim());
        _firebaseRemoteConfigHandler.dict.Add("Admob_AppOpen_IOS", _AppOpenAdUnit.IOS.Trim());
#endif
    }
    private void StartStuff()
    {
        Start_Banner();
        Start_Interstitial();
        Start_Rewarded();
        Start_RewardedInterstitial();
        Start_AppOpen();

    }
    private void InitiliazeStuff()
    {
        deviceRam = SystemInfo.systemMemorySize;
        if (StopShowingAdsOnLowEndDevices)
        {
            if (deviceRam >= minRamSize_GB * 1024)
            {
                Start_UserConsent();
            }
            else
            {
                lowRam = true;
            }
        }
        else
        {
            Start_UserConsent();

        }
    }
    public void RemoteConfigUpdate()
    {
        if (_UseRemoteConfig && !_TestMode)
        {

#if Remote_Config_Rizwan
            AssignRemoteConfigValues();
            InitiliazeStuff();

#else
            PrintStatus("Import Firebase RemoteConfig Or Check if TextMode is Active", true);
#endif
        }
    }

    private void AssignRemoteConfigValues()
    {
#if Remote_Config_Rizwan

        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
#if UNITY_ANDROID
            _BannerAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Banner_ANDROID_" + i, _BannerAdId[i], _BannerAdUnit[i].ANDROID.Trim());
            _BannerAdUnit[i].ANDROID = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID, _BannerAdUnit[i].ANDROID.Trim());
#elif UNITY_IOS
                _BannerAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Banner_IOS_" + i,_BannerAdId[i],  _BannerAdUnit[i].IOS.Trim());
                _BannerAdUnit[i].IOS = _firebaseRemoteConfigHandler.AddOrUpdateKey( "Admob_Banner_IOS_" + i,_BannerAdUnit[i].IOS, _BannerAdUnit[i].IOS.Trim());
#endif
        }

        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
#if UNITY_ANDROID
            InterstitialAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Interstitial_ANDROID_" + i, InterstitialAdId[i], _InterstitialAdUnit[i].ANDROID.Trim());
            _InterstitialAdUnit[i].ANDROID = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Interstitial_ANDROID_" + i, _InterstitialAdUnit[i].ANDROID, _InterstitialAdUnit[i].ANDROID.Trim());
#elif UNITY_IOS

                InterstitialAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey( "Admob_Interstitial_IOS_" + i, InterstitialAdId[i],_InterstitialAdUnit[i].IOS.Trim());
                _InterstitialAdUnit[i].IOS = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Interstitial_IOS_" + i,_InterstitialAdUnit[i].IOS,  _InterstitialAdUnit[i].IOS.Trim());
#endif
        }


        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
#if UNITY_ANDROID
            RewardedAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Rewarded_ANDROID_" + i, RewardedAdId[i], _RewardedAdUnit[i].ANDROID.Trim());
            _RewardedAdUnit[i].ANDROID = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Rewarded_ANDROID_" + i, _RewardedAdUnit[i].ANDROID, _RewardedAdUnit[i].ANDROID.Trim());
#elif UNITY_IOS
                RewardedAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_Rewarded_IOS_" + i,RewardedAdId[i],  _RewardedAdUnit[i].IOS.Trim());
                _RewardedAdUnit[i].IOS = _firebaseRemoteConfigHandler.AddOrUpdateKey( "Admob_Rewarded_IOS_" + i,_RewardedAdUnit[i].IOS, _RewardedAdUnit[i].IOS.Trim());
#endif
        }

        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
#if UNITY_ANDROID
            RewardedInterstitialAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_RewardedInterstital_ANDROID_" + i, RewardedInterstitialAdId[i], _RewardedInterstitalAdUnit[i].ANDROID.Trim());
            _RewardedInterstitalAdUnit[i].ANDROID = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_RewardedInterstital_ANDROID_" + i, _RewardedInterstitalAdUnit[i].ANDROID, _RewardedInterstitalAdUnit[i].ANDROID.Trim());

#elif UNITY_IOS
                RewardedInterstitialAdId[i] = _firebaseRemoteConfigHandler.AddOrUpdateKey( "Admob_RewardedInterstital_IOS_" + i, RewardedInterstitialAdId[i],_RewardedInterstitalAdUnit[i].IOS.Trim());
                _RewardedInterstitalAdUnit[i].IOS = _firebaseRemoteConfigHandler.AddOrUpdateKey( "Admob_RewardedInterstital_IOS_" + i,_RewardedInterstitalAdUnit[i].IOS, _RewardedInterstitalAdUnit[i].IOS.Trim());

#endif
        }
#if UNITY_ANDROID
        _AppOpenAdId = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_AppOpen_ANDROID", _AppOpenAdId, _AppOpenAdUnit.ANDROID.Trim());
        _AppOpenAdUnit.ANDROID = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_AppOpen_ANDROID", _AppOpenAdUnit.ANDROID, _AppOpenAdUnit.ANDROID.Trim());
#elif UNITY_IOS

            _AppOpenAdId = _firebaseRemoteConfigHandler.AddOrUpdateKey( "Admob_AppOpen_IOS",_AppOpenAdId, _AppOpenAdUnit.IOS.Trim());
            _AppOpenAdUnit.IOS = _firebaseRemoteConfigHandler.AddOrUpdateKey("Admob_AppOpen_IOS",_AppOpenAdUnit.IOS,  _AppOpenAdUnit.IOS.Trim());
#endif
#endif
        PrintStatus("Remote Config AssignRemoteConfigValues assigned", true);
    }
    public void JsonDataUpdate()
    {
#if Remote_Config_Rizwan
        tempdict = new Dictionary<string, object>();
        UpdateJsonData();
#else
        Debug.LogError("Import Firebase RemoteConfig");
#endif

    }
    private void UpdateJsonData()
    {
#if Remote_Config_Rizwan

        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            tempdict.Add("Admob_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
            tempdict.Add("Admob_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());
        }
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            tempdict.Add("Admob_Interstitial_ANDROID_" + i, _InterstitialAdUnit[i].ANDROID.Trim());
            tempdict.Add("Admob_Interstitial_IOS_" + i, _InterstitialAdUnit[i].IOS.Trim());
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            tempdict.Add("Admob_Rewarded_ANDROID_" + i, _RewardedAdUnit[i].ANDROID.Trim());
            tempdict.Add("Admob_Rewarded_IOS_" + i, _RewardedAdUnit[i].IOS.Trim());
        }
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
            tempdict.Add("Admob_RewardedInterstital_ANDROID_" + i, _RewardedInterstitalAdUnit[i].ANDROID.Trim());
            tempdict.Add("Admob_RewardedInterstital_IOS_" + i, _RewardedInterstitalAdUnit[i].IOS.Trim());
        }
        tempdict.Add("Admob_AppOpen_ANDROID", _AppOpenAdUnit.ANDROID.Trim());
        tempdict.Add("Admob_AppOpen_IOS", _AppOpenAdUnit.IOS.Trim());
#endif
    }
    #region USER CONSENT
    void Start_UserConsent()
    {
        // If we can request ads, we should initialize the Google Mobile Ads Unity plugin.
        if (_consentController.CanRequestAds)
        {
            InitializeGoogleMobileAds();
            //return;
        }
        InitializeGoogleMobileAdsConsent();
    }

    private void InitializeGoogleMobileAdsConsent()
    {
        PrintStatus("Google Mobile Ads gathering consent.", false);

        _consentController.GatherConsent((string error) =>
        {
            if (error != null)
            {
                PrintStatus("Failed to gather consent with error: " + error, true);
            }
            else
            {
                PrintStatus("Google Mobile Ads consent updated.", false);
            }

            if (_consentController.CanRequestAds)
            {
                InitializeGoogleMobileAds();
            }

        });
    }


    private void InitializeGoogleMobileAds()
    {
        // The Google Mobile Ads Unity plugin needs to be run only once and before loading any ads.
        if (_isInitialized.HasValue)
        {
            return;
        }
        _isInitialized = false;

        PrintStatus("Google Mobile Ads Initializing.", false);
        MobileAds.Initialize((InitializationStatus initstatus) =>
        {
#if Admob_Mediation_Rizwan
            if (_UseMediation)
            {
                //Admob Mediation Portion
                Dictionary<string, AdapterStatus> map = initstatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState)
                    {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            PrintStatus("Adapter: " + className + " not ready.", true);
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            PrintStatus("Adapter: " + className + " is initialized.", false);
                            break;
                    }
                }
            }
#endif

            if (initstatus == null)
            {
                _isInitialized = null;
                PrintStatus("Google Mobile Ads initialization failed.", true);

            }
            else
            {

                _isInitialized = true;
                PrintStatus("Google Mobile Ads initialization complete.", false);
            }

            StartCoroutine(nameof(InitiliazeAdUnits));
            #region AppOpen        
            AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
            LoadAppOpenAd();
            #endregion
        });

        if (_TestMode)
        {
            //MobileAds.SetRequestConfiguration(new RequestConfiguration
            //{
            //    TestDeviceIds = TestDeviceIds
            //});
            StartCoroutine(nameof(InitiliazeAdUnits));
        }
        else
        {
            if (_consentController.CanRequestAds)
            {
                //Admob
                RequestConfiguration requestConfiguration = new RequestConfiguration
                {
                    TagForChildDirectedTreatment = TagForChildDirectedTreatment.False,
                    TagForUnderAgeOfConsent = TagForUnderAgeOfConsent.False,
                    PublisherFirstPartyIdEnabled = true
                };

                MobileAds.SetRequestConfiguration(requestConfiguration);

#if Admob_Mediation_Rizwan
                if (_UseMediation)
                {
                    ////Applovin
                    //GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetHasUserConsent(true);
                    //GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetIsAgeRestrictedUser(false);
                    //GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetDoNotSell(true);
                    //GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.Initialize();

                    ////InMobi
                    //Dictionary<string, string> consentObject = new Dictionary<string, string>();
                    //consentObject.Add("gdpr_consent_available", "true");
                    //consentObject.Add("gdpr", "1");

                    //GoogleMobileAds.Mediation.InMobi.Api.InMobi.UpdateGDPRConsent(consentObject);

                    ////UnityAds
                    //GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData("gdpr.consent", true);
                }
#endif

            }

        }

    }
    //    private void InitializeGoogleMobileAds()
    //    {
    //        // The Google Mobile Ads Unity plugin needs to be run only once and before loading any ads.
    //        if (_isInitialized.HasValue)
    //        {
    //            return;
    //        }
    //        _isInitialized = false;
    //        // Initialize the Google Mobile Ads Unity plugin.
    //        // Debug.Log("Google Mobile Ads Initializing.");

    //        MobileAds.Initialize((InitializationStatus initstatus) =>
    //        {
    //            if (initstatus == null)
    //            {
    //                Debug.LogError("Google Mobile Ads initialization failed.");
    //                _isInitialized = null;
    //                return;
    //            }
    //            // If you use mediation, you can check the status of each adapter.
    //            var adapterStatusMap = initstatus.getAdapterStatusMap();
    //            if (adapterStatusMap != null)
    //            {
    //                foreach (var item in adapterStatusMap)
    //                {
    //                    Debug.Log(string.Format("Adapter {0} is {1}",
    //                        item.Key,
    //                        item.Value.InitializationState));
    //                }
    //            }

    //            Debug.Log("Google Mobile Ads initialization complete.");
    //            _isInitialized = true;
    //#if Admob_Mediation_Rizwan
    //                        if (_UseMediation)
    //                        {
    //                            //Admob Mediation Portion
    //                            Dictionary<string, AdapterStatus> map = initstatus.getAdapterStatusMap();
    //                            foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
    //                            {
    //                                string className = keyValuePair.Key;
    //                                AdapterStatus status = keyValuePair.Value;
    //                                switch (status.InitializationState)
    //                                {
    //                                    case AdapterState.NotReady:
    //                                        // The adapter initialization did not complete.
    //                                        PrintStatus("Adapter: " + className + " not ready.", true);
    //                                        break;
    //                                    case AdapterState.Ready:
    //                                        // The adapter was successfully initialized.
    //                                        PrintStatus("Adapter: " + className + " is initialized.", false);
    //                                        break;
    //                                }
    //                            }
    //                        }
    //#endif
    //        });

    //        if (_TestMode)
    //        {
    //            //MobileAds.SetRequestConfiguration(new RequestConfiguration
    //            //{
    //            //    TestDeviceIds = TestDeviceIds
    //            //});
    //            StartCoroutine(nameof(InitiliazeAdUnits));

    //        }
    //        else
    //        {
    //            StartCoroutine(nameof(InitiliazeAdUnits));
    //            if (_consentController.CanRequestAds)
    //            {
    //                //Admob
    //                RequestConfiguration requestConfiguration = new RequestConfiguration
    //                {
    //                    TagForChildDirectedTreatment = TagForChildDirectedTreatment.False,
    //                    TagForUnderAgeOfConsent = TagForUnderAgeOfConsent.False,
    //                    PublisherFirstPartyIdEnabled = true // comment it if using version 8.*.* and uncomment SameAppKeyEnabled=true 
    //                    //SameAppKeyEnabled = true   // comment it if using version 9.*.* and uncomment PublisherFirstPartyIdEnabled=true 
    //                };
    //                MobileAds.SetRequestConfiguration(requestConfiguration);
    //#if Admob_Mediation_Rizwan
    //                if (_UseMediation)
    //                {
    //                    //Applovin //Comment this Applovin portion if you are not using InMobie
    //                    GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetHasUserConsent(true);
    //                    GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetIsAgeRestrictedUser(false);
    //                    GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.SetDoNotSell(true);
    //                    GoogleMobileAds.Mediation.AppLovin.Api.AppLovin.Initialize();

    //                    //InMobi //Comment this inMobi portion if you are not using InMobie
    //                    Dictionary<string, string> consentObject = new Dictionary<string, string>
    //                    {
    //                        { "gdpr_consent_available", "true" },
    //                        { "gdpr", "1" }
    //                    };
    //                    GoogleMobileAds.Mediation.InMobi.Api.InMobi.UpdateGDPRConsent(consentObject);

    //                    //UnityAds //Comment this UnityAds portion if you are not using InMobie
    //                    GoogleMobileAds.Mediation.UnityAds.Api.UnityAds.SetConsentMetaData("gdpr.consent", true);
    //                }
    //#endif
    //                //  adjustAdid = Adjust.getAdid();
    //            }

    //        }

    //    }

    #endregion
    IEnumerator InitiliazeAdUnits()
    {
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            RequestAndLoadInterstitialAd(i);
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            RequestAndLoadRewardedAd(i);
        }
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            RequestAndLoadRewarded_InterstitalAd(i);
        }
        yield return null;
    }
    #region  BANNER
    private void Start_Banner()
    {
        _BannerAdId = new string[_BannerAdUnit.Length];

        if (_TestMode)
        {
            InitializeTestBannerAdIds();
        }
        else
        {
            InitializeRealBannerAdIds();
        }

        InitializeBannerViews();
    }

    private void InitializeTestBannerAdIds()
    {
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {

            //BEGIN_ADMOB
#if UNITY_ANDROID
            _BannerAdId[i] = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IOS
                    _BannerAdId[i] = "ca-app-pub-3940256099942544/2934735716";
#endif
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            #if UNITY_ANDROID
                                    _BannerAdId[i] = "/21775744923/example/adaptive-banner";
            #elif UNITY_IOS
                                                                    _BannerAdId[i] = "/21775744923/example/adaptive-banner";
            #endif
            */
            //END_ADMANAGER

        }
    }

    private void InitializeRealBannerAdIds()
    {
#if UNITY_ANDROID
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            _BannerAdId[i] = _BannerAdUnit[i].ANDROID.Trim();
        }
#elif UNITY_IOS
    for (int i = 0; i < _BannerAdUnit.Length; i++)
    {
        _BannerAdId[i] = _BannerAdUnit[i].IOS.Trim();
    }
#else
    for (int i = 0; i < _BannerAdUnit.Length; i++)
    {
        _BannerAdId[i] = "unexpected_platform";
    }
#endif
    }

    private void InitializeBannerViews()
    {
        //BEGIN_ADMOB
        _bannerViews = new BannerView[_BannerAdUnit.Length];
        //END_ADMOB

        //BEGIN_ADMANAGER
        /*
        _bannerViews = new AdManagerBannerView[_BannerAdUnit.Length];
        */
        //END_ADMANAGER 

        _bannerLoaded = new bool[_BannerAdUnit.Length];
        _bannerShowing = new bool[_BannerAdUnit.Length];
        _bannerShowingForAllADs = new bool[_BannerAdUnit.Length];

        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            _bannerViews[i] = null;
            _bannerLoaded[i] = false;
            _bannerShowing[i] = false;
            _bannerShowingForAllADs[i] = false;
        }
    }
    //BEGIN_ADMOB
    BannerView RequestBanner(int index)
    //END_ADMOB

    //BEGIN_ADMANAGER
    /*
    AdManagerBannerView RequestBanner(int index)
    */
    //END_ADMANAGER
    {
        // Clean up banner ad before creating a new one.
        if (_bannerViews[index] != null)
        {
            _bannerViews[index].Destroy();
            _bannerViews[index] = null;
        }

        //BEGIN_ADMOB
        _bannerViews[index] = new BannerView(_BannerAdId[index], _BannerAdUnit[index]._AdSize, _BannerAdUnit[index]._AdPosition);
        //END_ADMOB

        //BEGIN_ADMANAGER
        /*
        _bannerViews[index] = new AdManagerBannerView(_BannerAdId[index], _BannerAdUnit[index]._AdSize, _BannerAdUnit[index]._AdPosition);
        */
        //END_ADMANAGER 

        //BEGIN_ADMOB
        AdRequest adRequest = CreateAdRequest();
        //END_ADMOB

        //BEGIN_ADMANAGER
        /*
        AdManagerAdRequest adRequest = CreateAdRequest();
        */
        //END_ADMANAGER 

        //Load the banner ad with the request
        if (_BannerAdUnit[index].adSize == _bannerAd.BannerAdSize.collapsible)
        {
            if (_BannerAdUnit[index]._AdPosition == AdPosition.Top)
            {
                adRequest.Extras.Add("collapsible", "top");
                //adRequest.Extras.Add("collapsible_request_id", _BannerAdId[index]);
            }
            else if (_BannerAdUnit[index]._AdPosition == AdPosition.Bottom)
            {
                adRequest.Extras.Add("collapsible", "bottom");
                //adRequest.Extras.Add("collapsible_request_id", _BannerAdId[index]);
            }
        }

        // Load a banner ad.
        _bannerViews[index].LoadAd(adRequest);

        // Register for ad events.
        _bannerViews[index].OnBannerAdLoaded += () =>
        {
            OnBannerAdLoaded(index);
        };
        _bannerViews[index].OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            OnBannerAdLoadFailed(error, index);
        };
        return _bannerViews[index];
    }
    public void Show_BannerAd(int index)
    {
        if (lowRam)
        {
            return;
        }
        if (_bannerShowing[index] == false)
        {
            _bannerShowing[index] = true;
            if (_bannerLoaded[index] == false)
            {
                _bannerViews[index] = RequestBanner(index);
            }
            else
            {
                if (_bannerViews[index] != null)
                {
                    _bannerViews[index].Show();
                }
            }
        }
    }
    //BEGIN_ADMOB
    AdRequest CreateAdRequest()
    {
        var adRequest = new AdRequest();
        return adRequest;
    }
    //END_ADMOB

    //BEGIN_ADMANAGER
    /*
    AdManagerAdRequest CreateAdRequest()
            {
                var adRequest = new AdManagerAdRequest();
                return adRequest;
            }
    */
    //END_ADMANAGER 


    private void OnBannerAdLoaded(int index)
    {
        _bannerLoaded[index] = true;
        if (_bannerShowing[index] == false)
        {
            Hide_BannerAd(index);
        }

        PrintStatus("Banner view loaded an ad with response : "
                 + _bannerViews[index].GetResponseInfo(), false);
        PrintStatus($"Ad Height: {0}, width: {1}" +
            $"{_bannerViews[index].GetHeightInPixels()} " +
            $"{_bannerViews[index].GetWidthInPixels()}", true);
    }
    private void OnBannerAdLoadFailed(LoadAdError error, int index)
    {
        _bannerLoaded[index] = false;
        PrintStatus("_bannerViews " + index + " Banner ad failed to load with error: " + error, true);
    }
    public void Hide_BannerAd(int index)
    {
        _bannerShowing[index] = false;
        if (_bannerViews[index] != null)
        {
            //#if UNITY_EDITOR
            //  _bannerViews[index].Destroy();
            //#else
            _bannerViews[index].Hide();
            //#endif
        }
    }
    public void HideActiveBanners()
    {
        if (!HideBannerWhenShowingFullScreenAd) return;
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_bannerViews[i] != null && _bannerShowing[i])
            {
                //#if UNITY_EDITOR
                //                _bannerViews[i].Destroy();
                //#else
                _bannerViews[i].Hide();
                //#endif
                _bannerShowing[i] = false;
                _bannerShowingForAllADs[i] = true;
            }
        }
    }
    public void ShowActiveBanners()
    {
        if (!HideBannerWhenShowingFullScreenAd) return;
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
#if UNITY_EDITOR
            if (_bannerShowingForAllADs[i])
            {
                Show_BannerAd(i);
                _bannerShowingForAllADs[i] = false;
            }
#else
            if (_bannerViews[i] != null && _bannerShowingForAllADs[i])
            {
                _bannerViews[i].Show();
                _bannerShowing[i] = true;
                _bannerShowingForAllADs[i] = false;
            }
#endif
        }
    }
    public void HideActiveBannersForAppOpen()
    {
        if (HideBannerWhenShowingFullScreenAd) return;
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_bannerViews[i] != null && _bannerShowing[i])
            {
                if (HideTopBannersWhenShowingAppOpen
                    &&
                    (_BannerAdUnit[i]._AdPosition == AdPosition.Top
                    ||
                    _BannerAdUnit[i]._AdPosition == AdPosition.TopRight
                    ||
                    _BannerAdUnit[i]._AdPosition == AdPosition.TopLeft))
                {
                    _bannerViews[i].Hide();
                    _bannerShowing[i] = false;
                    _bannerShowingForAllADs[i] = true;
                }
            }
        }
    }
    public void ShowActiveBannersForAppOpen()
    {
        if (HideBannerWhenShowingFullScreenAd) return;
        if (HideTopBannersWhenShowingAppOpen)
        {
            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {
#if UNITY_EDITOR
                if (_bannerShowingForAllADs[i])
                {
                    Show_BannerAd(i);
                    _bannerShowingForAllADs[i] = false;
                }
#else
                if (_bannerViews[i] != null && _bannerShowingForAllADs[i])
                {
                    _bannerViews[i].Show();
                    _bannerShowing[i] = true;
                    _bannerShowingForAllADs[i] = false;
                }
#endif
            }
        }
    }
    // Destroy a specific banner ad by index
    public void Destroy_BannerAd(int index)
    {
        _bannerViews[index].Destroy();
        _bannerViews[index] = null;
        _bannerLoaded[index] = false;
        _bannerShowing[index] = false;
        _bannerShowingForAllADs[index] = false;

        //// Register for ad events.
        //_bannerViews[index].OnBannerAdLoaded -= () =>
        //{
        //    OnBannerAdLoaded(index);
        //};
        //_bannerViews[index].OnBannerAdLoadFailed -= (LoadAdError error) =>
        //{
        //    OnBannerAdLoadFailed(error, index);
        //};

        PrintStatus("_Banner Destroy Called", true);
    }
    #endregion

    #region INTERSTITIAL
    void Start_Interstitial()
    {
        if (_TestMode)
        {
            InterstitialAdId = new string[_InterstitialAdUnit.Length];
            InitializeTestInterstitialAdIds();
        }
        else
        {
            InterstitialAdId = new string[_InterstitialAdUnit.Length];
            InitializeRealInterstitialAdIds();
        }
        //BEGIN_ADMOB
        _interstitialAd = new InterstitialAd[InterstitialAdId.Length];
        //END_ADMOB

        //BEGIN_ADMANAGER
        /*
        _interstitialAd = new AdManagerInterstitialAd[InterstitialAdId.Length];
        */
        //END_ADMANAGER
        for (int i = 0; i < InterstitialAdId.Length; i++)
        {
            _interstitialAd[i] = null;
        }
    }

    private void InitializeTestInterstitialAdIds()
    {
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {

            //BEGIN_ADMOB
#if UNITY_ANDROID
            InterstitialAdId[i] = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IOS
                        InterstitialAdId[i] = "ca-app-pub-3940256099942544/4411468910";
#endif
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            #if UNITY_ANDROID
                                    InterstitialAdId[i] = "/21775744923/example/interstitial";
            #elif UNITY_IOS
                                                                        InterstitialAdId[i] = "/21775744923/example/interstitial";
            #endif
            */
            //END_ADMANAGER
        }
    }

    private void InitializeRealInterstitialAdIds()
    {
#if UNITY_ANDROID
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            InterstitialAdId[i] = _InterstitialAdUnit[i].ANDROID.Trim();
        }
#elif UNITY_IOS
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            InterstitialAdId[i] = _InterstitialAdUnit[i].IOS.Trim();
        }
#else
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            InterstitialAdId[i] = "unexpected_platform";
        }
#endif
    }
    //private InterstitialAd interstitial;
    public void RequestAndLoadInterstitialAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_InterstitialAdUnit.Length == 0)
        {

            return;
        }
        if (index >= 0 && index < InterstitialAdId.Length)
        {
            // Check if the Interstitial ad is already loaded
            if (_interstitialAd[index] != null)
            {

                _interstitialAd[index].Destroy();
                _interstitialAd[index] = null;
                //return;
            }
            //FireBaseListener.Instance?.//PrintStatus("Loadeding Interstitial");

            // Create our request used to load the ad.
            //BEGIN_ADMOB
            var adRequest = new AdRequest();
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            var adRequest = new AdManagerAdRequest();
            */
            //END_ADMANAGER 


            //BEGIN_ADMOB
            InterstitialAd.Load(InterstitialAdId[index], adRequest, (InterstitialAd ad, LoadAdError loadError) =>
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            AdManagerInterstitialAd.Load(InterstitialAdId[index], adRequest, (AdManagerInterstitialAd ad, LoadAdError loadError) =>
            */
            //END_ADMANAGER 
                  {
                      if (loadError != null)
                      {
                          PrintStatus("Interstitial ad " + index + " failed to load an ad with error : " + loadError, true);
                          return;
                      }
                      else if (ad == null)
                      {
                          PrintStatus("Unexpected error: Interstitial ad  " + index + " load event fired with null ad and null error.", true);
                          return;
                      }
                      _interstitialAd[index] = ad;
                      // Raised when the ad is estimated to have earned money.
                      ad.OnAdPaid += (AdValue adValue) =>
                      {
                          if (adValue == null)
                          {
                              PrintStatus("AdValue is null in OnAdPaid callback.", true);
                              return;
                          }
#if gameanalytics_admob_enabled
                                                                                      GameAnalytics.NewBusinessEvent(adValue.CurrencyCode, (int)adValue.Value, "InterStitial ad id", InterstitialAdId[index], "");
#endif
#if Firebase_Rizwan
                          FirebaseAnalyticsHandler.Instance.LogFirebaseEvent("adValue.CurrencyCode = " + adValue.CurrencyCode,
                                                                             ", adValue.Value = " + (int)adValue.Value,
                                                                             ", Formate = " + "AppOpen" + ", _AppOpenAdUnit Id = " + InterstitialAdId[index]);
#endif
                      };
                      // Raised when an impression is recorded for an ad.
                      //ad.OnAdImpressionRecorded += () =>
                      //{
                      //   PrintStatus("Interstitial ad " + index + " recorded an impression.");
                      //};
                      // Raised when a click is recorded for an ad.
                      //ad.OnAdClicked += () =>
                      //{
                      //    PrintStatus("Interstitial ad " + index + "was clicked.", false);
                      //};
                      // Raised when an ad opened full screen content.
                      ad.OnAdFullScreenContentOpened += () =>
                      {
                          // PrintStatus("Interstitial ad " + index + " full screen content opened.");
                          isShowingInterstitial = true;
#if gameanalytics_admob_enabled
                                                                                      GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, "Admob", "<InterStitial>");
#endif
                      };
                      // Raised when the ad closed full screen content.
                      ad.OnAdFullScreenContentClosed += () =>
                      {
                          //_interstitialAd[index] = null;
                          RequestAndLoadInterstitialAd(index);
                          isShowingInterstitial = false;
                          FullScreenAd_Closed?.Invoke();
                          PrintStatus("Interstitial ad " + index + " full screen content closed. ", false);

                      };
                      // Raised when the ad failed to open full screen content.
                      ad.OnAdFullScreenContentFailed += (AdError error) =>
                      {
                          //_interstitialAd[index] = null;
                          RequestAndLoadInterstitialAd(index);
                          isShowingInterstitial = false;
                          FullScreenAd_Closed?.Invoke();
                          PrintStatus("Interstitial ad " + index + " failed to open full screen content with error : " + error, true);
                      };

                  });
        }
        else
        {
            PrintStatus("_interstitialAd " + index + " is Out of bound", true);
        }


    }

    public void ShowInterstitialAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        if (AnyFullScreenAdShowing() == true)
        {
            PrintStatus("AnyFullScreenAdShowing == true", true);
            return;
        }

        if (index >= 0 && index < InterstitialAdId.Length)
        {
            if (_interstitialAd[index] != null && _interstitialAd[index].CanShowAd())
            {
                _interstitialAd[index].Show();
                isShowingInterstitial = true;
                FullScreenAd_Shown?.Invoke();

            }
            else
            {
                RequestAndLoadInterstitialAd(index);
                PrintStatus("Interstitial ad " + index + " is not ready yet.", true);
            }
        }
        else
        {
            PrintStatus("Interstitial Ad " + index + " is Out of bound", true);
        }
    }
    public void DestroyInterstitialAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        if (index >= 0 && index < _interstitialAd.Length)
        {
            if (_interstitialAd[index] != null)
            {
                _interstitialAd[index].Destroy();
                _interstitialAd[index] = null;
            }
        }
        else
        {
            //FireBaseListener.Instance?.//PrintStatus("Index is 0,Add placements for Interstitial");
        }
    }
    #endregion

    #region REWARDED
    void Start_Rewarded()
    {
        RewardedAdId = new string[_RewardedAdUnit.Length];
        if (_TestMode)
        {
            InitializeTestRewardedAdIds();
        }
        else
        {
            InitializeRealRewardedAdIds();
        }
        _rewardedAd = new RewardedAd[RewardedAdId.Length];
        for (int i = 0; i < RewardedAdId.Length; i++)
        {
            _rewardedAd[i] = null;
        }
    }

    private void InitializeTestRewardedAdIds()
    {
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {

            //BEGIN_ADMOB
#if UNITY_ANDROID
            RewardedAdId[i] = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IOS
                        RewardedAdId[i] = "ca-app-pub-3940256099942544/1712485313";
#endif
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            #if UNITY_ANDROID
                                    RewardedAdId[i] = "/21775744923/example/rewarded";
            #elif UNITY_IOS
                                                                        RewardedAdId[i] = "/21775744923/example/rewarded";
            #endif
            */
            //END_ADMANAGER
        }
    }

    private void InitializeRealRewardedAdIds()
    {
#if UNITY_ANDROID
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            RewardedAdId[i] = _RewardedAdUnit[i].ANDROID.Trim();
        }
#elif UNITY_IOS
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            RewardedAdId[i] = _RewardedAdUnit[i].IOS.Trim();
        }
#else
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            RewardedAdId[i] = "unexpected_platform";
        }
#endif
    }
    //private RewardedAd interstitial;
    public void RequestAndLoadRewardedAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        if (index >= 0 && index < RewardedAdId.Length)
        {
            // Check if the Rewarded ad is already loaded

            if (_rewardedAd[index] != null)
            {
                //FireBaseListener.Instance?.//PrintStatus("Already has Loaded RewardedAd");
                _rewardedAd[index].Destroy();
                _rewardedAd[index] = null;

                //return;
            }

            //FireBaseListener.Instance?.//PrintStatus("Laoding RewardedAd");

            //BEGIN_ADMOB
            var adRequest = new AdRequest();
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            var adRequest = new AdManagerAdRequest();
            */
            //END_ADMANAGER 

            RewardedAd.Load(RewardedAdId[index], adRequest, (RewardedAd ad, LoadAdError loadError) =>
            {
                if (loadError != null)
                {

                    PrintStatus("Rewarded ad failed to load an ad with error : " + loadError, true);
                    return;
                }
                else if (ad == null)
                {

                    PrintStatus("Unexpected error: Rewarded load event fired with null ad and null error.", true);
                    return;
                }

                //FireBaseListener.Instance?.//PrintStatus("Rewarded ad loaded.");
                _rewardedAd[index] = ad;
                // Raised when the ad is estimated to have earned money.
                ad.OnAdPaid += (AdValue adValue) =>
                {
                    if (adValue == null)
                    {
                        PrintStatus("AdValue is null in OnAdPaid callback.", true);
                        return;
                    }
#if gameanalytics_admob_enabled
                    GameAnalytics.NewBusinessEvent(adValue.CurrencyCode, (int)adValue.Value, "Rewarded", RewardedAdId.ToString(), "");
#endif
#if Firebase_Rizwan
                    FirebaseAnalyticsHandler.Instance.LogFirebaseEvent("adValue.CurrencyCode = " + adValue.CurrencyCode,
                                                                       ", adValue.Value = " + (int)adValue.Value,
                                                                       ", Formate = " + "Rewarded video" + ", Rewarded Ad Id = " + RewardedAdId[index]);
#endif

                    //// Replace with your attribution platform name, for example, "Adjust"
                    //string attributionPlatformName = MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER;

                    //// Replace "userid" with your attribution platform UID
                    //MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(attributionPlatformName, adjustAdid);

                    //mBridgeRevenueParamsEntity.SetAdmobAdUnitid(RewardedAdId[index]);
                    //// Replace with your current ad type, options include: Rewarded, Banner, Interstitial, Native, AppOpen.
                    //mBridgeRevenueParamsEntity.SetAdmobAdType(admobAdType.Rewarded);
                    //// adValue: a instance of AdValue
                    //mBridgeRevenueParamsEntity.SetAdmobAdValue(adValue);
                    //mBridgeRevenueParamsEntity.SetAdmobResponseInfo(ad.GetResponseInfo());
                    //MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);

                };
                // Raised when an impression is recorded for an ad.
                //ad.OnAdImpressionRecorded += () =>
                //{
                //   PrintStatus("Rewarded ad recorded an impression.");
                //};
                // Raised when a click is recorded for an ad.
                //ad.OnAdClicked += () =>
                //{                  
                //    PrintStatus("Rewarded ad was clicked.", false);
                //};
                // Raised when the ad opened full screen content.
                ad.OnAdFullScreenContentOpened += () =>
                {
                    isShowingRewarded = true;

                    PrintStatus("Rewarded ad full screen content opened.", false);
#if gameanalytics_admob_enabled
                    GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "Admob", "<RewardedAd>");
#endif
                };
                // Raised when the ad closed full screen content.
                ad.OnAdFullScreenContentClosed += () =>
                {
                    isShowingRewarded = false;
                    FullScreenAd_Closed?.Invoke();
                    RequestAndLoadRewardedAd(index);
                    PrintStatus("Rewarded ad full screen content closed.", false);
#if gameanalytics_admob_enabled
                    GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "Admob", "<RewardedAd>");
#endif

                };
                // Raised when the ad failed to open full screen content.
                ad.OnAdFullScreenContentFailed += (AdError error) =>
                {
                    isShowingRewarded = false;
                    FullScreenAd_Closed?.Invoke();
                    RequestAndLoadRewardedAd(index);
                    PrintStatus("Rewarded ad failed to open full screen content with error : " + error, true);
                };
            });
        }
        else
        {
            PrintStatus("Rewarded Ad " + index + " is Out of bound", true);
        }
    }

    public void ShowRewardedAd(int index, Action _reward, Action _failed)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        if (AnyFullScreenAdShowing() == true)
        {
            PrintStatus("AnyFullScreenAdShowing == true", true);
            return;
        }
        if (index >= 0 && index < _rewardedAd.Length)
        {
            if (_rewardedAd[index] != null && _rewardedAd[index].CanShowAd())
            {
                _rewardedAd[index].Show((Reward reward) =>
                {
                    //reward here
                    _reward?.Invoke();
                });
                isShowingRewarded = true;
                FullScreenAd_Shown?.Invoke();
            }
            else
            {
                _failed?.Invoke();
                RequestAndLoadRewardedAd(index);
            }
        }
        else
        {
            _failed?.Invoke();
        }
    }
    public void DestroyRewardedlAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        if (_rewardedAd[index] != null)
        {
            _rewardedAd[index].Destroy();
            _rewardedAd[index] = null;
        }
    }
    #endregion

    #region REWARDED_INTERSTITIAL
    void Start_RewardedInterstitial()
    {
        RewardedInterstitialAdId = new string[_RewardedInterstitalAdUnit.Length];
        if (_TestMode)
        {
            InitializeTestRewardedInterstitialAdIds();
        }
        else
        {
            InitializeRealRewardedInterstitialAdIds();
        }

        _rewardedInterstitialAd = new RewardedInterstitialAd[RewardedInterstitialAdId.Length];
        for (int i = 0; i < RewardedInterstitialAdId.Length; i++)
        {
            _rewardedInterstitialAd[i] = null;
        }
    }

    private void InitializeTestRewardedInterstitialAdIds()
    {
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {

            //BEGIN_ADMOB
#if UNITY_ANDROID
            RewardedInterstitialAdId[i] = "ca-app-pub-3940256099942544/5354046379";
#elif UNITY_IOS
                        RewardedInterstitialAdId[i] = "ca-app-pub-3940256099942544/6978759866";
#endif
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            #if UNITY_ANDROID
                                    RewardedInterstitialAdId[i] = "/21775744923/example/rewarded-interstitial";
            #elif UNITY_IOS
                                                                        RewardedInterstitialAdId[i] = "/21775744923/example/rewarded-interstitial";
            #endif
            */
            //END_ADMANAGER
        }
    }

    private void InitializeRealRewardedInterstitialAdIds()
    {
#if UNITY_ANDROID
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
            RewardedInterstitialAdId[i] = _RewardedInterstitalAdUnit[i].ANDROID.Trim();
        }
#elif UNITY_IOS
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
            RewardedInterstitialAdId[i] = _RewardedInterstitalAdUnit[i].IOS.Trim();
        }
#else
        for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        {
            RewardedInterstitialAdId[i] = "unexpected_platform";
        }
#endif
    }

    public void RequestAndLoadRewarded_InterstitalAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_RewardedInterstitalAdUnit.Length == 0)
        {
            PrintStatus("_RewardedInterstitalAdUnit is Null", true);
            return;
        }
        if (index >= 0 && index < RewardedInterstitialAdId.Length)
        {
            // Check if the RewardedInterstital ad is already loaded
            if (_rewardedInterstitialAd[index] != null)
            {
                _rewardedInterstitialAd[index].Destroy();
                _rewardedInterstitialAd[index] = null;
                //FireBaseListener.Instance?.//PrintStatus("Already has Loaded RewardedIntertitialAd");
                //return;
            }
            //FireBaseListener.Instance?.//PrintStatus("Laoding RewardedIntertitialAd");

            //BEGIN_ADMOB
            var adRequest = new AdRequest();
            //END_ADMOB

            //BEGIN_ADMANAGER
            /*
            var adRequest = new AdManagerAdRequest();
            */
            //END_ADMANAGER 

            RewardedInterstitialAd.Load(RewardedInterstitialAdId[index], adRequest,
                (RewardedInterstitialAd ad, LoadAdError loadError) =>
                {
                    if (loadError != null)
                    {

                        PrintStatus("RewardedInterstital ad failed to load with error: " + loadError.GetMessage(), true);
                        return;
                    }
                    else if (ad == null)
                    {

                        PrintStatus("RewardedInterstital ad failed to load.", true);
                        return;
                    }

                    //FireBaseListener.Instance?.//PrintStatus("RewardedInterstital ad loaded.");
                    _rewardedInterstitialAd[index] = ad;

                    // Raised when the ad is estimated to have earned money.
                    ad.OnAdPaid += (AdValue adValue) =>
                    {
                        if (adValue == null)
                        {
                            PrintStatus("AdValue is null in OnAdPaid callback.", true);
                            return;
                        }
#if gameanalytics_admob_enabled
                        GameAnalytics.NewBusinessEvent(adValue.CurrencyCode, (int)adValue.Value, "RewardedInterstital ", RewardedInterstitialAdId[index], "");
#endif
#if Firebase_Rizwan
                        FirebaseAnalyticsHandler.Instance.LogFirebaseEvent("adValue.CurrencyCode = " + adValue.CurrencyCode,
                                                                           ", adValue.Value = " + (int)adValue.Value,
                                                                           ", Formate = " + "RewardedInterstitial" + ", RewardedInterstitial Ad Id = " + RewardedInterstitialAdId[index]);

#endif
                    };
                    // Raised when an impression is recorded for an ad.
                    //ad.OnAdImpressionRecorded += () =>
                    //{
                    //   PrintStatus("Rewarded interstitial ad recorded an impression.");
                    //};
                    // Raised when a click is recorded for an ad.
                    //ad.OnAdClicked += () =>
                    //{                    
                    //    PrintStatus("Rewarded interstitial ad was clicked.", false);
                    //};
                    // Raised when an ad opened full screen content.
                    ad.OnAdFullScreenContentOpened += () =>
                    {
                        isShowingRewardedInterstitial = true;
                        PrintStatus("Rewarded interstitial ad full screen content opened.", false);
#if gameanalytics_admob_enabled
                        GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "Admob", "<Rewarded_InterstitialAd>");
#endif
                    };
                    // Raised when the ad closed full screen content.
                    ad.OnAdFullScreenContentClosed += () =>
                    {
                        isShowingRewardedInterstitial = false;
                        FullScreenAd_Closed?.Invoke();
                        RequestAndLoadRewarded_InterstitalAd(index);
                        PrintStatus("Rewarded interstitial ad full screen content closed.", false);
#if gameanalytics_admob_enabled
                        GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, "Admob", "<Rewarded_InterstitialAd>");
#endif
                    };
                    // Raised when the ad failed to open full screen content.
                    ad.OnAdFullScreenContentFailed += (AdError error) =>
                    {
                        isShowingRewardedInterstitial = false;
                        FullScreenAd_Closed?.Invoke();
                        RequestAndLoadRewarded_InterstitalAd(index);
                        PrintStatus("Rewarded interstitial ad failed to open full screen content" + " with error : " + error, true);
                    };

                });
        }
        else
        {
            //FireBaseListener.Instance?.//PrintStatus("Index is 0,Add placements for RewardedInterstital");
        }
    }



    public void ShowRewardedInterstitalAd(int index, Action _reward, Action _failed)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_RewardedInterstitalAdUnit.Length == 0)
        {
            PrintStatus("_RewardedInterstitalAdUnit is Null", true);
            return;
        }
        if (AnyFullScreenAdShowing() == true)
        {
            PrintStatus("AnyFullScreenAdShowing == true", true);
            return;
        }
        if (index >= 0 && index < _rewardedInterstitialAd.Length)
        {
            if (_rewardedInterstitialAd[index] != null && _rewardedInterstitialAd[index].CanShowAd())
            {
                _rewardedInterstitialAd[index].Show((Reward reward) =>
                {
                    //FireBaseListener.Instance?.//PrintStatus("RewardedInterstital ad granted a reward: " + reward.Amount);
                    //reward here
                    _reward?.Invoke();

                });

                isShowingRewardedInterstitial = true;
                FullScreenAd_Shown?.Invoke();

#if gameanalytics_admob_enabled
                GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, "ADMOB", " RewardedInterstitialAdId " + RewardedInterstitialAdId[index]);
#endif
#if Firebase_Rizwan
                FirebaseAnalyticsHandler.Instance.LogFirebaseEvent("Ad Action = Show",
                                                                   ", Ad Type = Banner ",
                                                                   ", RewardedInterstitial Ad Id = " + RewardedInterstitialAdId[index]);
#endif
            }
            else
            {
                _failed.Invoke();
                RequestAndLoadRewarded_InterstitalAd(index);
            }
        }
        else
        {
            _failed.Invoke();
            RequestAndLoadRewarded_InterstitalAd(index);
            PrintStatus("Rewarded_Interstitalad is not ready yet, failed.", true);
        }
    }
    public void DestroyRewarded_InterstitalAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_RewardedInterstitalAdUnit.Length == 0)
        {
            PrintStatus("_RewardedInterstitalAdUnit is Null", true);
            return;
        }
        if (_rewardedInterstitialAd[index] != null)
        {
            _rewardedInterstitialAd[index].Destroy();
            _rewardedInterstitialAd[index] = null;
        }
    }
    #endregion

    #region APPOPEN ADS

    void Start_AppOpen()
    {
        if (_TestMode)
        {
            InitializeTestAppOpenAdId();
        }
        else
        {
            InitializeRealAppOpenAdId();
        }

        LoadAppOpenAd();
    }

    private void InitializeTestAppOpenAdId()
    {

        //BEGIN_ADMOB
#if UNITY_ANDROID
        _AppOpenAdId = "ca-app-pub-3940256099942544/9257395921";
#elif UNITY_IOS
                _AppOpenAdId = "ca-app-pub-3940256099942544/5575463023";
#endif
        //END_ADMOB

        //BEGIN_ADMANAGER
        /*
        #if UNITY_ANDROID
                        _AppOpenAdId = "/21775744923/example/app-open";
        #elif UNITY_IOS
                                                _AppOpenAdId = "/21775744923/example/app-open";
        #endif
        */
        //END_ADMANAGER
    }

    private void InitializeRealAppOpenAdId()
    {
#if UNITY_ANDROID
        _AppOpenAdId = _AppOpenAdUnit.ANDROID.Trim();
#elif UNITY_IOS
        _AppOpenAdId = _AppOpenAdUnit.IOS.Trim();
#else
        _AppOpenAdId = "unexpected_platform";
#endif
    }


    public void LoadAppOpenAd()
    {
        PrintStatus("Loading app open ad.", false);
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (!CanShowAppOpen)
        {
            PrintStatus("CanShowAppOpen =false", true);
            return;
        }
        //BEGIN_ADMOB
        var adRequest = new AdRequest();
        //END_ADMOB

        //BEGIN_ADMANAGER
        /*
        var adRequest = new AdManagerAdRequest();
        */
        //END_ADMANAGER 

        AppOpenAd.Load(_AppOpenAdId, adRequest, (AppOpenAd openAppad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                PrintStatus("App open ad failed to load an ad with error : " + error, true);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (openAppad == null)
            {
                PrintStatus("Unexpected error: App open ad load event fired with " + " null ad and null error.", true);
                return;
            }
            // The operation completed successfully.
            PrintStatus("App open ad loaded ", false);

            _appOpenAd = openAppad;

            // App open ads can be preloaded for up to 4 hours.
            _expireTime = DateTime.Now + TIMEOUT;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(openAppad);

            if (CanShowAppOpenAtStartup && SceneManager.GetActiveScene().buildIndex == 0)
            {
                ShowAppOpenAdIfAvailable();
                CanShowAppOpenAtStartup = false;

            }
            else
            {
                CanShowAppOpenAtStartup = false;

            }

        });
    }

    bool AnyFullScreenAdShowing()
    {
#if Admob_Simple_Rizwan
        if (AnyFullScreenAdShowing_Admob()) { return true; }
#endif
#if Max_Mediation_Rizwan
        if (AdsController.Instance._maxMediation.AnyFullScreenAdShowing_Max()) { return true; }
#endif
#if UnityAds_Rizwan
        if (AdsController.Instance._unityAdsManager.AnyFullScreenAdShowing_UnityAds()) { return true; }
#endif
        return false;
    }

    bool AnyFullScreenAdShowing_Admob()
    {
        if (isShowingAppopen || isShowingInterstitial || isShowingRewarded || isShowingRewardedInterstitial)
        {
            return true;
        }
        return false;
    }


    public void ShowAppOpenAdIfAvailable()
    {
        if (AdsController.Instance != null && AdsController.Instance._adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }

        if (lowRam)
        {

            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }

        if (!CanShowAppOpen)
        {

            PrintStatus("CanShowAppOpen =false", true);
            return;
        }

        if (AnyFullScreenAdShowing() == true)
        {

            PrintStatus("AnyFullScreenAdShowing == true", true);
            return;
        }

        if (DateTime.Now > _expireTime)
        {

            DestroyAppOpenAd();
            PrintStatus("DateTime.Now > _expireTime ", true);
        }
        else if (_appOpenAd != null && _appOpenAd.CanShowAd() && DateTime.Now < _expireTime)
        {
            if (AdsController.Instance) AdsController.Instance.BlackBackgroundImage.SetActive(true);
            isShowingAppopen = true;
            FullScreenAd_Shown?.Invoke();
            _appOpenAd.Show();
            HideActiveBannersForAppOpen();
            PrintStatus("Showing app open ad.", true);
        }
        else
        {

            isShowingAppopen = false;
            LoadAppOpenAd();
            PrintStatus("App open ad is not ready yet.", true);
        }

    }
    //IEnumerator AppOpenAdShow_Co()
    //{
    //    yield return new WaitForSecondsRealtime(0.3f);
    //    _appOpenAd.Show();
    //}
    public void DestroyAppOpenAd()
    {
        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        if (_appOpenAd != null)
        {
            PrintStatus("Destroying app open ad.", true);
            _appOpenAd.Destroy();
            _appOpenAd = null;
        }
    }
    bool firstrun = false;
    private void OnAppStateChanged(AppState state)
    {

        if (lowRam)
        {
            PrintStatus("LowRAM so returning without destroy banner", true);
            return;
        }
        PrintStatus("App State changed to : " + state, true);
        // If the app is Foregrounded and the ad is available, show it.

        if (state == AppState.Foreground && firstrun)
        {
            ShowAppOpenAdIfAvailable();
        }
        else
        {
            firstrun = true;
        }

    }
    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            if (adValue == null)
            {
                PrintStatus("AdValue is null in OnAdPaid callback.", true);
                return;
            }
            //  PrintStatus(String.Format("App open ad paid {0} {1}.",   adValue.Value, adValue.CurrencyCode));
#if gameanalytics_admob_enabled
            GameAnalytics.NewBusinessEvent(adValue.CurrencyCode, (int)adValue.Value, "AppOpen ", _AppOpenAdId, "");
#endif
#if Firebase_Rizwan
            FirebaseAnalyticsHandler.Instance.LogFirebaseEvent("adValue.CurrencyCode = " + adValue.CurrencyCode,
                                                               ", adValue.Value = " + (int)adValue.Value,
                                                               ", Formate = " + "AppOpen" + ", _AppOpen AdId = " + _AppOpenAdId);
#endif
        };
        // Raised when an impression is recorded for an ad.
        //ad.OnAdImpressionRecorded += () =>
        //{
        //    PrintStatus("App open ad recorded an impression.", true);
        //};
        // Raised when a click is recorded for an ad.
        //ad.OnAdClicked += () =>
        //{
        //    PrintStatus("App open ad was clicked.", true);
        //};
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            isShowingAppopen = true;
            if (AdsController.Instance) AdsController.Instance.BlackBackgroundImage.SetActive(true);
            PrintStatus("App open ad full screen content opened.", true);
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            isShowingAppopen = false;
            if (AdsController.Instance) AdsController.Instance.BlackBackgroundImage.SetActive(false);
            FullScreenAd_Closed?.Invoke();
            ShowActiveBannersForAppOpen();
            LoadAppOpenAd();
            PrintStatus("App open ad full screen content closed.", true);
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            isShowingAppopen = false;
            FullScreenAd_Closed?.Invoke();
            ShowActiveBannersForAppOpen();
            LoadAppOpenAd();
            if (AdsController.Instance) AdsController.Instance.BlackBackgroundImage.SetActive(false);
            PrintStatus("App open ad failed to open full screen content with error : " + error, true);
        };
    }

    #endregion

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
#endif
}


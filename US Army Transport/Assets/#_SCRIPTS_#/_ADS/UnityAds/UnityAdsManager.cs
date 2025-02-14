
using UnityEngine.Advertisements;
#if gameanalytics_max_enabled || gameanalytics_admob_enabled
using GameAnalyticsSDK;
#endif
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;



#if UnityAds_Rizwan
//if you are having this error import advertisment/advertisment Legacy from Packages Manager
public class UnityAdsManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener, IUnityAdsInitializationListener

{
    // public FirebaseRemoteConfigHandler _fireBaseRemoteConfigHandler;
    public static bool DictUpdated = false;

    // public RectTransform canvasSize;
    #region Other Stuff
    internal Action FullScreenAd_Shown;
    internal Action FullScreenAd_Closed;

    // public bool _UseRemoteConfig = false;

    // Helper class that implements consent using the
    // Google User Messaging Platform (UMP) Unity plugin.
    [Space(20)]
    internal bool _TestMode = true;
    internal bool _Initialized = false;

    [Space(20)]
    [Header("LOW DEVICE EXEMPT")]
    [SerializeField, Tooltip("if value = false,minimum ram limited check will be neglected")]
    private bool StopShowingAdsOnLowEndDevices = false;
    [SerializeField, Tooltip("1=1GB, 1.5=1.5GB")]
    private int minRamSize_GB = 1;

    private float deviceRam = 0;
    private bool lowRam = false;
    public Dictionary<string, object> tempdict;

    [Space(20)]
    [Range(0, 1)]
    public float _VideoAdsSoundVolume = 0.5f;

    #endregion


    [Space(20)]

    public string _gameId_Android;
    public string _gameId_IOS;
    private string gameId;
    //private string _gameId;


    #region  Banner




    [Space(20)]
    public BannerLoadOptions[] optionsload = new BannerLoadOptions[1];
    public BannerOptions[] optionsshow = new BannerOptions[1];

    internal bool[] _bannerLoaded;
    internal bool[] _bannerShowing;
    internal bool[] _bannerShowingForAllADs;
    private string[] _BannerAdId;

    [Space(10)]
    public _bannerAd[] _BannerAdUnit = new _bannerAd[1]
{
     new _bannerAd
     {
         ANDROID = "Banner_Android",
         IOS = "Banner_iOS"
     }
};
    [System.Serializable]
    public class _bannerAd
    {
        public string ANDROID;
        public string IOS;
        public BannerPosition _AdPosition = BannerPosition.TOP_CENTER;
    }

    #endregion

    #region Interstitial
    internal string[] InterstitialAdId;
    internal bool[] InterstitialLoaded;
    private bool isShowingInterstitial = false;
    // [Space(20)]
    // [Header("INTERSTITIAL ADs")]
    public _InterstitialAd[] _InterstitialAdUnit = new _InterstitialAd[]
    {
        new _InterstitialAd
        {
            ANDROID = "Interstitial_Android",
            IOS = "Interstitial_iOS"
        }
    };


    [System.Serializable]
    public class _InterstitialAd
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;
    }

    #endregion

    #region Rewarded


    private string[] RewardedAdId;
    internal bool[] RewardedLoaded;
    private bool isShowingRewarded = false;
    public Action Reward, Failed;
    [Space(10)]
    [Header("REWARDED ADs")]

    public _RewardedAd[] _RewardedAdUnit = new _RewardedAd[1]
 {
     new _RewardedAd
     {
         ANDROID = "Rewarded_Android",
         IOS = "Rewarded_iOS"
     }
 };
    [System.Serializable]
    public class _RewardedAd
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;
    }

    #endregion



    private void OnEnable()
    {

#if Remote_Config_Rizwan
        //FirebaseRemoteConfigHandler.RemoteConfigUpdated += RemoteConfigUpdate;
#endif
    }
    private void OnDisable()
    {
#if Remote_Config_Rizwan
        #region RemoteConfig
        //FirebaseRemoteConfigHandler.RemoteConfigUpdated -= RemoteConfigUpdate;
        #endregion
#endif


    }


    internal void Start()
    {

        //        if (_UseRemoteConfig && !_TestMode)
        //        {
        //#if Remote_Config_Rizwan
        //            StartStuff();
        //            for (int i = 0; i < _BannerAdUnit.Length; i++)
        //            {
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());
        //            }
        //            for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        //            {
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_Interstitial_ANDROID_" + i, _InterstitialAdUnit[i].ANDROID.Trim());
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_Interstitial_IOS_" + i, _InterstitialAdUnit[i].IOS.Trim());
        //            }
        //            for (int i = 0; i < _RewardedAdUnit.Length; i++)
        //            {
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_Rewarded_ANDROID_" + i, _RewardedAdUnit[i].ANDROID.Trim());
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_Rewarded_IOS_" + i, _RewardedAdUnit[i].IOS.Trim());
        //            }
        //            for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
        //            {
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_RewardedInterstital_ANDROID_" + i, _RewardedInterstitalAdUnit[i].ANDROID.Trim());
        //                _fireBaseRemoteConfigHandler.dict.Add("UnityAds_RewardedInterstital_IOS_" + i, _RewardedInterstitalAdUnit[i].IOS.Trim());
        //            }
        //            _fireBaseRemoteConfigHandler.dict.Add("UnityAds_AppOpen_ANDROID", _AppOpenAdUnit.ANDROID.Trim());
        //            _fireBaseRemoteConfigHandler.dict.Add("UnityAds_AppOpen_IOS", _AppOpenAdUnit.IOS.Trim());

        //            DictUpdated = true;
        //            _fireBaseRemoteConfigHandler.CheckInitilization();


        //#else
        //            PrintStatus("Import Firebase RemoteConfig", true);
        //#endif
        //        }
        //        else
        //        {
        StartStuff();
        InitiliazeStuff();
        //}
    }
    private void StartStuff()
    {
#if UNITY_ANDROID
        gameId = _gameId_Android;

#elif UNITY_IOS
            gameId = _gameId_IOS;
#endif
        Start_Banner();
        Start_Interstitial();
        Start_Rewarded();



    }
    private void InitiliazeStuff()
    {
        deviceRam = SystemInfo.systemMemorySize;
        if (StopShowingAdsOnLowEndDevices)
        {
            if (deviceRam >= minRamSize_GB * 1024)
            {
                InitializeUnityAds();
            }
            else
            {
                lowRam = true;
            }
        }
        else
        {
            InitializeUnityAds();

        }
    }
    //    public void RemoteConfigUpdate()
    //    {
    //        if (_UseRemoteConfig && !_TestMode)
    //        {

    //#if Remote_Config_Rizwan
    //            for (int i = 0; i < _BannerAdUnit.Length; i++)
    //            {
    //#if UNITY_ANDROID
    //                _BannerAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Banner_ANDROID_" + i,  _BannerAdId[i],  _BannerAdUnit[i].ANDROID.Trim());
    //                _BannerAdUnit[i].ANDROID = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID, _BannerAdUnit[i].ANDROID.Trim());
    //#elif UNITY_IOS
    //                _BannerAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Banner_IOS_" + i,_BannerAdId[i],  _BannerAdUnit[i].IOS.Trim());
    //                _BannerAdUnit[i].IOS = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "UnityAds_Banner_IOS_" + i,_BannerAdUnit[i].IOS, _BannerAdUnit[i].IOS.Trim());
    //#endif
    //            }

    //            for (int i = 0; i < _InterstitialAdUnit.Length; i++)
    //            {
    //#if UNITY_ANDROID
    //                InterstitialAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Interstitial_ANDROID_" + i, InterstitialAdId[i],  _InterstitialAdUnit[i].ANDROID.Trim());
    //                _InterstitialAdUnit[i].ANDROID = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Interstitial_ANDROID_" + i, _InterstitialAdUnit[i].ANDROID, _InterstitialAdUnit[i].ANDROID.Trim());
    //#elif UNITY_IOS

    //                InterstitialAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "UnityAds_Interstitial_IOS_" + i, InterstitialAdId[i],_InterstitialAdUnit[i].IOS.Trim());
    //                _InterstitialAdUnit[i].IOS = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Interstitial_IOS_" + i,_InterstitialAdUnit[i].IOS,  _InterstitialAdUnit[i].IOS.Trim());
    //#endif
    //            }


    //            for (int i = 0; i < _RewardedAdUnit.Length; i++)
    //            {
    //#if UNITY_ANDROID
    //                RewardedAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Rewarded_ANDROID_" + i, RewardedAdId[i],  _RewardedAdUnit[i].ANDROID.Trim());
    //                _RewardedAdUnit[i].ANDROID = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Rewarded_ANDROID_" + i, _RewardedAdUnit[i].ANDROID, _RewardedAdUnit[i].ANDROID.Trim());
    //#elif UNITY_IOS
    //                RewardedAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_Rewarded_IOS_" + i,RewardedAdId[i],  _RewardedAdUnit[i].IOS.Trim());
    //                _RewardedAdUnit[i].IOS = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "UnityAds_Rewarded_IOS_" + i,_RewardedAdUnit[i].IOS, _RewardedAdUnit[i].IOS.Trim());
    //#endif
    //            }

    //            for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
    //            {
    //#if UNITY_ANDROID
    //                RewardedInterstitialAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_RewardedInterstital_ANDROID_" + i, RewardedInterstitialAdId[i],  _RewardedInterstitalAdUnit[i].ANDROID.Trim());
    //                _RewardedInterstitalAdUnit[i].ANDROID = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_RewardedInterstital_ANDROID_" + i, _RewardedInterstitalAdUnit[i].ANDROID,  _RewardedInterstitalAdUnit[i].ANDROID.Trim());

    //#elif UNITY_IOS
    //                RewardedInterstitialAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "UnityAds_RewardedInterstital_IOS_" + i, RewardedInterstitialAdId[i],_RewardedInterstitalAdUnit[i].IOS.Trim());
    //                _RewardedInterstitalAdUnit[i].IOS = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "UnityAds_RewardedInterstital_IOS_" + i,_RewardedInterstitalAdUnit[i].IOS, _RewardedInterstitalAdUnit[i].IOS.Trim());

    //#endif
    //            }
    //#if UNITY_ANDROID
    //            _AppOpenAdId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_AppOpen_ANDROID", _AppOpenAdId,  _AppOpenAdUnit.ANDROID.Trim());
    //            _AppOpenAdUnit.ANDROID = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_AppOpen_ANDROID", _AppOpenAdUnit.ANDROID,_AppOpenAdUnit.ANDROID.Trim());
    //#elif UNITY_IOS

    //            _AppOpenAdId = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "UnityAds_AppOpen_IOS",_AppOpenAdId, _AppOpenAdUnit.IOS.Trim());
    //            _AppOpenAdUnit.IOS = _fireBaseRemoteConfigHandler.AddOrUpdateKey("UnityAds_AppOpen_IOS",_AppOpenAdUnit.IOS,  _AppOpenAdUnit.IOS.Trim());
    //#endif
    //            InitiliazeStuff();
    //#else
    //            PrintStatus("Import Firebase RemoteConfig", true);
    //#endif
    //        }
    //    }
    //    public void JsonDataUpdate()
    //    {
    //#if Remote_Config_Rizwan
    //            tempdict = new Dictionary<string, object>();
    //            for (int i = 0; i < _BannerAdUnit.Length; i++)
    //            {
    //                tempdict.Add("UnityAds_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
    //                tempdict.Add("UnityAds_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());
    //            }
    //            for (int i = 0; i < _InterstitialAdUnit.Length; i++)
    //            {
    //                tempdict.Add("UnityAds_Interstitial_ANDROID_" + i, _InterstitialAdUnit[i].ANDROID.Trim());
    //                tempdict.Add("UnityAds_Interstitial_IOS_" + i, _InterstitialAdUnit[i].IOS.Trim());
    //            }
    //            for (int i = 0; i < _RewardedAdUnit.Length; i++)
    //            {
    //                tempdict.Add("UnityAds_Rewarded_ANDROID_" + i, _RewardedAdUnit[i].ANDROID.Trim());
    //                tempdict.Add("UnityAds_Rewarded_IOS_" + i, _RewardedAdUnit[i].IOS.Trim());
    //            }
    //            for (int i = 0; i < _RewardedInterstitalAdUnit.Length; i++)
    //            {
    //                tempdict.Add("UnityAds_RewardedInterstital_ANDROID_" + i, _RewardedInterstitalAdUnit[i].ANDROID.Trim());
    //                tempdict.Add("UnityAds_RewardedInterstital_IOS_" + i, _RewardedInterstitalAdUnit[i].IOS.Trim());
    //            }
    //            tempdict.Add("UnityAds_AppOpen_ANDROID", _AppOpenAdUnit.ANDROID.Trim());
    //            tempdict.Add("UnityAds_AppOpen_IOS", _AppOpenAdUnit.IOS.Trim());
    //#else
    //        Debug.LogError("Import Firebase RemoteConfig");
    //#endif

    //    }

    #region USER CONSENT

    private void InitializeUnityAds()
    {

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, _TestMode, this);
        }
    }


    public void OnInitializationComplete()
    {
        StartCoroutine(InitiliazeAdUnits());
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        _Initialized = false;
        Debug.LogError($"Initialize failed with error {error} and message is {message}");
    }

    #endregion
    IEnumerator InitiliazeAdUnits()
    {
        _Initialized = true;
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            RequestAndLoadInterstitialAd(i);
            yield return new WaitForSeconds(2f);
        }
        // yield return new WaitForSeconds(2f);
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            RequestAndLoadRewardedAd(i);
            yield return new WaitForSeconds(2f);
        }
        //  yield return new WaitForSeconds(2f);

        yield return null;
    }
    #region  BANNER
    void Start_Banner()
    {
        _BannerAdId = new string[_BannerAdUnit.Length];
        if (_TestMode)
        {
            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {
#if UNITY_ANDROID
                _BannerAdId[i] = "Banner_Android";

#elif UNITY_IOS
                _BannerAdId[i] = "Banner_iOS";
#endif
            }
        }
        else
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
                _BannerAdId[i]= "unexpected_platform" ;
            }  
       
#endif
        }

        //_resizeImage = new Image[_BannerAdId.Length];    
        _bannerLoaded = new bool[_BannerAdUnit.Length];
        _bannerShowing = new bool[_BannerAdUnit.Length];
        _bannerShowingForAllADs = new bool[_BannerAdUnit.Length];

        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            //_resizeImage[i] = Instantiate(_BackgroundImagePrefab, _Canvas.transform);
            //Color currentColor = _resizeImage[i].color;
            //// Set the alpha channel based on the opacity value
            //currentColor.a = BorderOpacity / 255f;
            //// Apply the new color to the image
            //_resizeImage[i].color = currentColor;
            //_resizeImage[i].gameObject.SetActive(false);
            _bannerLoaded[i] = false;
            _bannerShowing[i] = false;
            _bannerShowingForAllADs[i] = false;

        }

    }
    public void LoadAndShow_BannerAd(int index)
    {
        PrintStatus("_Banner LoadAndShow called ", true);
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return;
        }
        if (_BannerAdUnit.Length == 0)
        {
            PrintStatus("_BannerAdUnit " + index + " is Null", true);
            return;
        }

        // Set up options to notify the SDK of load events:
        optionsload[index] = new BannerLoadOptions
        {
            loadCallback = () => OnBannerLoaded(index),
            errorCallback = (message) => OnBannerError(message, index)
        };

        if (index >= 0 && index < _BannerAdUnit.Length)
        {
            // Load the Ad Unit with banner content:
            Advertisement.Banner.Load(_BannerAdId[index], optionsload[index]);
            _bannerShowing[index] = true;
        }
    }

    void OnBannerLoaded(int index)
    {
        Debug.Log("Banner loaded");
        _bannerLoaded[index] = true;
        Advertisement.Banner.Show(_BannerAdId[index], optionsshow[index]);
        if (_bannerShowing[index] == false)
        {
            Hide_BannerAd(index);
        }
    }

    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message, int index)
    {
        Debug.Log($"Banner Error: {message}");
        _bannerLoaded[index] = false;
        PrintStatus("unityAds banner " + index + " Banner ad failed to load with error: " + message.ToString(), true);

    }
    void OnBannerClicked(int index)
    {
    }
    void OnBannerShown(int index)
    {
    }
    void OnBannerHidden(int index)
    {
    }

    public void Show_BannerAd(int index)
    {

        if (CanShowBanner(index) == false)
        {
            PrintStatus("CanShowBanner", true);
            return;
        }
        if (_bannerLoaded[index])
        {
            Advertisement.Banner.Show(_BannerAdId[index], optionsshow[index]);
            _bannerShowing[index] = true;

            optionsshow[index] = new BannerOptions
            {
                clickCallback = () => OnBannerClicked(index),
                hideCallback = () => OnBannerHidden(index),
                showCallback = () => OnBannerShown(index)
            };

        }
        else
        {
            _bannerShowing[index] = true;
            LoadAndShow_BannerAd(index);
        }
    }

    // Hide a specific banner ad by index
    public void Hide_BannerAd(int index)
    {
        PrintStatus("_Banner Hide called", false);
        if (CanHideBanner(index) == false)
        {
            PrintStatus($"CanHideBanner is {CanHideBanner(index)}", true);
            return;
        }
        _bannerShowing[index] = false;
        _bannerShowingForAllADs[index] = false;
        PrintStatus($"_bannerShowing[{index}] is {_bannerShowing[index]}", true);
        Advertisement.Banner.Hide(false);
    }
    public void Hide_BannerAdForADs(int index)
    {
        PrintStatus("_Banner Hide called", false);
        if (_BannerAdId == null)
        {
            PrintStatus("_BannerAdId is null", true);
            return;
        }
        if (CanHideBanner(index) == false)
        {
            PrintStatus($"CanHideBanner  is {CanHideBanner(index)}", true);
            return;
        }
        _bannerShowing[index] = false;
        Advertisement.Banner.Hide(false);
    }
    public void HideActiveBanners()
    {
        PrintStatus("HideActiveBanners", false);
        StopCoroutine(nameof(ShowActiveBanners_Co));
        if (_bannerShowing != null && _bannerShowing.Length > 0)
        {

            for (int i = 0; i < _bannerShowing.Length; i++)
            {
                if (_bannerShowing[i])
                {
                    _bannerShowingForAllADs[i] = true;
                    Hide_BannerAdForADs(i);
                }
            }
        }
    }

    bool CanShowBanner(int index)
    {
        PrintStatus("_Banner Show called", false);
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return false;
        }
        if (_BannerAdUnit.Length == 0)
        {
            PrintStatus("_BannerAdUnit is Null", true);
            return false;
        }
        if (string.IsNullOrWhiteSpace(_BannerAdId[index]))
        {
            PrintStatus("_BannerAdId is IsNullOrWhiteSpace", true);
            return false;
        }
        if (index < 0 || index >= _BannerAdUnit.Length)
        {
            PrintStatus("_bannerViews " + index + " is Out of bound", true);
            return false;
        }
        if (_bannerShowing[index] == true)
        {
            PrintStatus("_bannerShowing true " + "_bannerShowing is " + _bannerShowing, true);
            return false;
        }
        if (_bannerShowingForAllADs[index] == true)
        {
            PrintStatus("_bannerShowingForAllADs true" + "_bannerShowingForAllADs is " + _bannerShowingForAllADs, true);
            return false;
        }
        PrintStatus("_bannerViews CanShowBanner is returned true", false);

        return true;

    }
    bool CanHideBanner(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return false;
        }
        if (_BannerAdUnit.Length == 0)
        {
            PrintStatus("_BannerAdUnit is Null", true);
            return false;
        }
        if (string.IsNullOrWhiteSpace(_BannerAdId[index]))
        {
            PrintStatus("_BannerAdId is IsNullOrWhiteSpace", true);
            return false;
        }
        if (index < 0 || index >= _BannerAdUnit.Length)
        {
            PrintStatus("_BannerAdUnit " + index + " is Out of bound", true);
            return false;
        }
        if (_bannerShowing[index] == false)
        {
            PrintStatus("_bannerShowing false " + "_bannerShowing[index] is " + _bannerShowing, true);
            return false;
        }

        PrintStatus("_bannerViews CanHideBanner is returned true", false);
        return true;
    }

    public void ShowActiveBanners()
    {
        StartCoroutine(nameof(ShowActiveBanners_Co));
    }
    IEnumerator ShowActiveBanners_Co()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (AnyFullScreenAdShowing_UnityAds() == false)
        {
            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {
                if (_bannerShowingForAllADs[i] && _bannerShowing[i])
                {
                    _bannerShowingForAllADs[i] = false;
                    Show_BannerAd(i);
                }
            }
        }
        else
        {
            PrintStatus("AnyFullScreenAdShowing returned False, means Full Screen Already Active", true);
        }

    }
    // Destroy a specific banner ad by index
    public void Destroy_BannerAd(int index)
    {
        PrintStatus("_Banner Destroy Called", true);
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return;
        }

        if (index >= 0 && index < _BannerAdUnit.Length)
        {
            _bannerLoaded[index] = false;
            _bannerShowing[index] = false;
            _bannerShowingForAllADs[index] = false;
            Advertisement.Banner.Hide(true);
        }
        else
        {
            PrintStatus("_BannerAdUnit " + index + " is Out of bound", true);
        }
    }

    //without of index  ads,REMEMBER that without of index will call the index 0 



    #endregion

    #region INTERSTITIAL

    void Start_Interstitial()
    {
        if (_TestMode)
        {
            InterstitialAdId = new string[_InterstitialAdUnit.Length];
            InterstitialLoaded = new bool[_InterstitialAdUnit.Length];
            for (int i = 0; i < _InterstitialAdUnit.Length; i++)
            {
#if UNITY_ANDROID
                InterstitialAdId[i] = "Interstitial_Android";
#elif UNITY_IOS
                InterstitialAdId[i] = "Interstitial_iOS";
#endif
            }
        }
        else
        {

            InterstitialAdId = new string[_InterstitialAdUnit.Length];
            InterstitialLoaded = new bool[_InterstitialAdUnit.Length];
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
                InterstitialAdId[i]= "unexpected_platform" ;                
            }      
#endif
        }
    }

    public void RequestAndLoadInterstitialAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return;
        }
        if (_InterstitialAdUnit.Length == 0)
        {

            return;
        }

        if (index >= 0 && index < InterstitialAdId.Length)
        {
            Advertisement.Load(InterstitialAdId[index], this);

        }
    }

    public void ShowInterstitialAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return;
        }
        if (_InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        if (AnyFullScreenAdShowing_UnityAds() == true)
        {
            PrintStatus("AnyFullScreenAdShowing == true", true);
            return;
        }
        if (index >= 0 && index < InterstitialAdId.Length)
        {
            if (_InterstitialAdUnit[index] != null)
            {
                Advertisement.Show(InterstitialAdId[index], this);
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

    #endregion

    #region REWARDED
    void Start_Rewarded()
    {
        if (_TestMode)
        {
            RewardedAdId = new string[_RewardedAdUnit.Length];
            RewardedLoaded = new bool[_InterstitialAdUnit.Length];
            for (int i = 0; i < _RewardedAdUnit.Length; i++)
            {
#if UNITY_ANDROID
                RewardedAdId[i] = "Rewarded_Android";
#elif UNITY_IOS
                RewardedAdId[i] = "Rewarded_iOS";
#endif
            }
        }
        else
        {
            RewardedAdId = new string[_RewardedAdUnit.Length];
            RewardedLoaded = new bool[_InterstitialAdUnit.Length];

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
                RewardedAdId[i]= "unexpected_platform" ;               
            }        
#endif
        }
    }
    //private RewardedAd interstitial;
    public void RequestAndLoadRewardedAd(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return;
        }
        if (_RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        if (index >= 0 && index < RewardedAdId.Length)
        {
            Advertisement.Load(RewardedAdId[index], this);
        }
        else
        {
            PrintStatus("Rewarded Ad " + index + " is Out of bound", true);
        }
    }
    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:

    public void ShowRewardedAd(int index, Action _reward, Action _failed)
    {
        Reward = _reward;
        Failed = _failed;

        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return;
        }
        if (_RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        if (AnyFullScreenAdShowing_UnityAds() == true)
        {
            PrintStatus("AnyFullScreenAdShowing == true", true);
            return;
        }
        if (index >= 0 && index < _RewardedAdUnit.Length)
        {
            Advertisement.Show(RewardedAdId[index], this);
            isShowingRewarded = true;
            FullScreenAd_Shown?.Invoke();
        }
        else
        {
            _failed?.Invoke();
        }
    }

    #endregion



    public bool AnyFullScreenAdShowing_UnityAds()
    {
        if (isShowingInterstitial || isShowingRewarded)
        {
            return true;
        }
        return false;
    }

    void PrintStatus(string _string, bool errorLog)
    {
        if (AdsController.Instance && AdsController.Instance.DebugMode)
        {
#if UNITY_EDITOR
            if (errorLog)
            {
                Debug.Log("<color=red><b>#UnityAds# </b></color> <b>" + _string + "</b> <color=red><b>#UnityAds# </b></color>");
            }
            else
            {
                Debug.Log("<color=green><b>#UnityAds# </b></color> <b>" + _string + "</b> <color=green><b>#UnityAds# </b></color>");
            }
#elif UNITY_ANDROID || UNITY_IOS

            Debug.Log(_string);

#endif
        }
    }

    //EVENTS

    // Implement Load Listener and Show Listener interface methods: 
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        // Optionally execute code if the Ad Unit successfully loads content.
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            if (adUnitId.Equals(InterstitialAdId[i]))
            {
                InterstitialLoaded[i] = true;
            }
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            if (adUnitId.Equals(RewardedAdId[i]))
            {
                RewardedLoaded[i] = true;

            }
        }
    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
        // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(InterstitialAdId[i]))
            {
                InterstitialLoaded[i] = false;
            }
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(RewardedAdId[i]))
            {
                Failed?.Invoke();
                RewardedLoaded[i] = false;

            }
        }

    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");

        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(InterstitialAdId[i]))
            {
                Advertisement.Load(_adUnitId, this);
                isShowingInterstitial = false;
                FullScreenAd_Closed?.Invoke();
                InterstitialLoaded[i] = false;
            }
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(RewardedAdId[i]))
            {
                Failed?.Invoke();
                Advertisement.Load(_adUnitId, this);
                isShowingRewarded = false;
                FullScreenAd_Closed?.Invoke();
                RewardedLoaded[i] = false;
            }
        }
    }

    public void OnUnityAdsShowStart(string _adUnitId)
    {
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(InterstitialAdId[i]))
            {
                isShowingInterstitial = true;

            }
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(RewardedAdId[i]))
            {
                isShowingRewarded = true;
            }
        }
    }

    public void OnUnityAdsShowComplete(string _adUnitId, string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(InterstitialAdId[i]))
            {

                Advertisement.Load(_adUnitId, this);
                PrintStatus($"Interstitial ad {_adUnitId} full screen content closed. ", false);
                isShowingInterstitial = false;
                FullScreenAd_Closed?.Invoke();
            }
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            if (_adUnitId.Equals(RewardedAdId[i]) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                Reward?.Invoke();
                Advertisement.Load(_adUnitId, this);
                PrintStatus($"Interstitial ad {_adUnitId} full screen content closed. ", false);
                isShowingRewarded = false;
                FullScreenAd_Closed?.Invoke();
            }
        }

    }

    public void OnUnityAdsShowClick(string placementId)
    {

    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        for (int i = 0; i < _InterstitialAdUnit.Length; i++)
        {
            if (placementId.Equals(InterstitialAdId[i]))
            {
                Advertisement.Load(placementId, this);
                PrintStatus($"Interstitial ad {placementId} full screen content closed. ", false);
                isShowingInterstitial = false;
                FullScreenAd_Closed?.Invoke();
            }
        }
        for (int i = 0; i < _RewardedAdUnit.Length; i++)
        {
            if (placementId.Equals(RewardedAdId[i]) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                Reward?.Invoke();
                Advertisement.Load(placementId, this);
                PrintStatus($"Interstitial ad {placementId} full screen content closed. ", false);
                isShowingRewarded = false;
                FullScreenAd_Closed?.Invoke();
            }
        }
    }



}
#else
public class UnityAdsManager : MonoBehaviour
{ }
#endif
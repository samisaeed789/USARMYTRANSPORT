
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEditor.Build;
#endif
public class MaxMediation : MonoBehaviour
{
#if Max_Mediation_Rizwan
    public FirebaseRemoteConfigHandler _fireBaseRemoteConfigHandler;
    public static bool DictUpdated = false;
    public static Task _task;
    internal bool _TestMode = false;
    public bool _UseRemoteConfig = false;
    internal Action FullScreenAd_Shown;
    internal Action FullScreenAd_Closed;

    [SerializeField]
    private string MaxSdkKey = string.Empty;
    [SerializeField]
    private bool StopShowingAdsOnLowEndDevices = false;
    [SerializeField]
    private int minRamSize_GB = 1;

    private float deviceRam = 0;
    private bool lowRam = false;
    public Dictionary<string, object> tempdict;

    #region Banner

    internal bool[] _bannerLoaded;
    internal bool[] _bannerShowing;
    internal bool[] _bannerShowingForAllADs;

    private string[] _BannerAdId;
    private int[] bannerAdAttempt;

    [Space(10)]
    public _bannerAd[] _BannerAdUnit;
    [System.Serializable]
    public class _bannerAd
    {
        public string ANDROID;
        [Space(10)]
        public string IOS;

        public _bannerPosition AdPosition = _bannerPosition.TopCenter;

        public enum _bannerPosition
        {
            TopLeft,
            TopCenter,
            TopRight,
            Centered,
            CenterLeft,
            CenterRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
        public enum BannerSize
        {
            Banner,
            Adaptive,
            MediumRectangle
        }
        public BannerSize bannerSize = BannerSize.Banner;
        private MaxSdkBase.BannerPosition _banner_position;
        private MaxSdkBase.AdViewPosition _adviewposition;

        public MaxSdkBase.BannerPosition bannerPosition
        {
            get
            {  // Convert the selected adSizeEnum to AdSize
                switch (bannerSize)
                {
                    case BannerSize.Banner:
                        switch (AdPosition)
                        {
                            case _bannerPosition.TopLeft:
                                _banner_position = MaxSdkBase.BannerPosition.TopLeft;
                                break;
                            case _bannerPosition.TopCenter:
                                _banner_position = MaxSdkBase.BannerPosition.TopCenter;
                                break;
                            case _bannerPosition.TopRight:
                                _banner_position = MaxSdkBase.BannerPosition.TopRight;
                                break;
                            case _bannerPosition.Centered:
                                _banner_position = MaxSdkBase.BannerPosition.Centered;
                                break;
                            case _bannerPosition.CenterLeft:
                                _banner_position = MaxSdkBase.BannerPosition.CenterLeft;
                                break;
                            case _bannerPosition.CenterRight:
                                _banner_position = MaxSdkBase.BannerPosition.CenterRight;
                                break;
                            case _bannerPosition.BottomLeft:
                                _banner_position = MaxSdkBase.BannerPosition.BottomLeft;
                                break;
                            case _bannerPosition.BottomCenter:
                                _banner_position = MaxSdkBase.BannerPosition.BottomCenter;
                                break;
                            case _bannerPosition.BottomRight:
                                _banner_position = MaxSdkBase.BannerPosition.BottomRight;
                                break;
                            default:
                                // Handle the case where AdPosition doesn't match any of the specified values
                                break;
                        }
                        break;

                    case BannerSize.Adaptive:
                        switch (AdPosition)
                        {
                            case _bannerPosition.TopLeft:
                                _banner_position = MaxSdkBase.BannerPosition.TopCenter;
                                break;
                            case _bannerPosition.TopCenter:
                                _banner_position = MaxSdkBase.BannerPosition.TopCenter;
                                break;
                            case _bannerPosition.TopRight:
                                _banner_position = MaxSdkBase.BannerPosition.TopCenter;
                                break;
                            case _bannerPosition.Centered:
                                _banner_position = MaxSdkBase.BannerPosition.Centered;
                                break;
                            case _bannerPosition.CenterLeft:
                                _banner_position = MaxSdkBase.BannerPosition.Centered;
                                break;
                            case _bannerPosition.CenterRight:
                                _banner_position = MaxSdkBase.BannerPosition.Centered;
                                break;
                            case _bannerPosition.BottomLeft:
                                _banner_position = MaxSdkBase.BannerPosition.BottomCenter;
                                break;
                            case _bannerPosition.BottomCenter:
                                _banner_position = MaxSdkBase.BannerPosition.BottomCenter;
                                break;
                            case _bannerPosition.BottomRight:
                                _banner_position = MaxSdkBase.BannerPosition.BottomCenter;
                                break;
                            default:
                                // Handle the case where AdPosition doesn't match any of the specified values
                                break;
                        }
                        break;

                }
                return _banner_position;
            }
        }
        public MaxSdkBase.AdViewPosition adviewPosition
        {
            get
            {  // Convert the selected adSizeEnum to AdSize
                switch (bannerSize)
                {
                    case BannerSize.MediumRectangle:

                        switch (AdPosition)
                        {
                            case _bannerPosition.TopLeft:
                                _adviewposition = MaxSdkBase.AdViewPosition.TopLeft;
                                break;
                            case _bannerPosition.TopCenter:
                                _adviewposition = MaxSdkBase.AdViewPosition.TopCenter;
                                break;
                            case _bannerPosition.TopRight:
                                _adviewposition = MaxSdkBase.AdViewPosition.TopRight;
                                break;
                            case _bannerPosition.Centered:
                                _adviewposition = MaxSdkBase.AdViewPosition.Centered;
                                break;
                            case _bannerPosition.CenterLeft:
                                _adviewposition = MaxSdkBase.AdViewPosition.CenterLeft;
                                break;
                            case _bannerPosition.CenterRight:
                                _adviewposition = MaxSdkBase.AdViewPosition.CenterRight;
                                break;
                            case _bannerPosition.BottomLeft:
                                _adviewposition = MaxSdkBase.AdViewPosition.BottomLeft;
                                break;
                            case _bannerPosition.BottomCenter:
                                _adviewposition = MaxSdkBase.AdViewPosition.BottomCenter;
                                break;
                            case _bannerPosition.BottomRight:
                                _adviewposition = MaxSdkBase.AdViewPosition.BottomRight;
                                break;
                            default:
                                // Handle the case where AdPosition doesn't match any of the specified values
                                break;
                        }

                        break;
                }
                return _adviewposition;
            }
        }

    }
    #endregion

    #region Interstitial

    internal string InterstitialAdUnitId;
    [Space(20)]
    public _InterstitialAd _InterstitialAdUnit;

    [System.Serializable]
    public class _InterstitialAd
    {
        public string ANDROID_1;
        public string ANDROID_2;
        public string ANDROID_3;
        [Space(10)]
        public string IOS_1;
        public string IOS_2;
        public string IOS_3;

    }
    private int interstitialAdAttempt = 0;

    private bool isShowingInterstitial = false;
    internal bool isInterstitialAdReady = false;
    #endregion

    #region Rewarded
    internal string RewardedAdUnitId;
    [Space(20)]
    public _RewardedAd _RewardedAdUnit;
    [System.Serializable]
    public class _RewardedAd
    {
        public string ANDROID_1;
        public string ANDROID_2;
        public string ANDROID_3;
        [Space(10)]
        public string IOS_1;
        public string IOS_2;
        public string IOS_3;
    }
    private int rewardedAdAttempt = 0;
    private bool isShowingRewarded = false;
    [HideInInspector] public bool isRewardedAdReady = false;


    #endregion

    #region RewardedInterstitial

    private string RewardedInterstitialAdUnitId;
    [Space(20)]
    public _RewardedInterstitialAd _RewardedInterstitialAdUnit;
    [System.Serializable]
    public class _RewardedInterstitialAd
    {
        public string ANDROID_1;
        public string ANDROID_2;
        public string ANDROID_3;
        [Space(10)]
        public string IOS_1;
        public string IOS_2;
        public string IOS_3;
    }
    private int rewardedInterstitialAdAttempt = 0;
    private bool isShowingRewardedInterstitial = false;
    [HideInInspector] public bool isRewardedInterstitialAdReady = false;

    #endregion

    #region AppOpen


    private bool isShowingAppOpen = false;
    [Space(20)]
    [Header("APPOPEN ADs")]
    [SerializeField] private bool CanShowAppOpen = true;
    [SerializeField] private bool CanShowAppOpenAtStartup = false;
    [SerializeField, Range(0, 20)] private int SecondsLimitToShowAppOpen = 3;
    [HideInInspector] public bool isAppOpenAdReady = false;

    private string AppOpenAdUnitId;
    [Space(20)]
    public _AppOpenAd _AppOpenAdUnit;
    [System.Serializable]
    public class _AppOpenAd
    {
        public string ANDROID_1;
        public string ANDROID_2;
        public string ANDROID_3;
        [Space(10)]
        public string IOS_1;
        public string IOS_2;
        public string IOS_3;

    }
    private int AppOpenAdAttempt = 0;

    #endregion

    private Action _reward, _failed;
    private bool rewardGiven = false;
    private string appsFlyerId = string.Empty;

    private void OnEnable()
    {
#if Remote_Config_Rizwan
        FirebaseRemoteConfigHandler.RemoteConfigUpdated += RemoteConfigUpdate;
        FirebaseRemoteConfigHandler.RemoteConfigureFailed += InitiliazeStuff;
#endif
    }
    private void OnDestroy()
    {
#if Remote_Config_Rizwan
        FirebaseRemoteConfigHandler.RemoteConfigUpdated -= RemoteConfigUpdate;
        FirebaseRemoteConfigHandler.RemoteConfigureFailed -= InitiliazeStuff;
#endif
    }

    IEnumerator StartSecond()
    {
        yield return new WaitForSeconds(SecondsLimitToShowAppOpen);
        CanShowAppOpenAtStartup = false;
    }
    public void ShowDebugMediation()
    {
        MaxSdk.ShowMediationDebugger();
    }


    internal void Start()
    {
        if (_UseRemoteConfig && !_TestMode)
        {
#if Remote_Config_Rizwan
            StartStuff();

            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {
                _fireBaseRemoteConfigHandler.dict.Add("Max_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
                _fireBaseRemoteConfigHandler.dict.Add("Max_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());
            }
            _fireBaseRemoteConfigHandler.dict.Add("Max_Interstitial_ANDROID_1", _InterstitialAdUnit.ANDROID_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Interstitial_ANDROID_2", _InterstitialAdUnit.ANDROID_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Interstitial_ANDROID_3", _InterstitialAdUnit.ANDROID_3.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Interstitial_IOS_1", _InterstitialAdUnit.IOS_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Interstitial_IOS_2", _InterstitialAdUnit.IOS_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Interstitial_IOS_3", _InterstitialAdUnit.IOS_3.Trim());

            _fireBaseRemoteConfigHandler.dict.Add("Max_Rewarded_ANDROID_1", _RewardedAdUnit.ANDROID_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Rewarded_ANDROID_2", _RewardedAdUnit.ANDROID_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Rewarded_ANDROID_3", _RewardedAdUnit.ANDROID_3.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Rewarded_IOS_1", _RewardedAdUnit.IOS_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Rewarded_IOS_2", _RewardedAdUnit.IOS_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_Rewarded_IOS_3", _RewardedAdUnit.IOS_3.Trim());

            _fireBaseRemoteConfigHandler.dict.Add("Max_RewardedInterstital_ANDROID_1", _RewardedInterstitialAdUnit.ANDROID_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_RewardedInterstital_ANDROID_2", _RewardedInterstitialAdUnit.ANDROID_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_RewardedInterstital_ANDROID_3", _RewardedInterstitialAdUnit.ANDROID_3.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_RewardedInterstital_IOS_1", _RewardedInterstitialAdUnit.IOS_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_RewardedInterstital_IOS_2", _RewardedInterstitialAdUnit.IOS_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_RewardedInterstital_IOS_3", _RewardedInterstitialAdUnit.IOS_3.Trim());

            _fireBaseRemoteConfigHandler.dict.Add("Max_AppOpen_ANDROID_1", _AppOpenAdUnit.ANDROID_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_AppOpen_ANDROID_2", _AppOpenAdUnit.ANDROID_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_AppOpen_ANDROID_3", _AppOpenAdUnit.ANDROID_3.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_AppOpen_IOS_1", _AppOpenAdUnit.IOS_1.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_AppOpen_IOS_2", _AppOpenAdUnit.IOS_2.Trim());
            _fireBaseRemoteConfigHandler.dict.Add("Max_AppOpen_IOS_3", _AppOpenAdUnit.IOS_3.Trim());

            DictUpdated = true;
            _fireBaseRemoteConfigHandler.CheckInitilization();
            // _fireBaseRemoteConfigHandler.InitializeFirebase_RemoteConfig(_fireBaseRemoteConfigHandler.dict);


#else

            PrintStatus("Import Remote Config", true);
#endif
        }
        else
        {
            StartStuff();
            InitiliazeStuff();
        }

    }
    void StartStuff()
    {
        SetInterstitialAdUnitId(0);
        SetRewardedAdUnitId(0);
        SetRewardedInterstitialAdUnitId(0);
        SetAppOpenAdUnitId(0);
        SetBannerAdUnitId();
    }
    void InitiliazeStuff()
    {
        deviceRam = SystemInfo.systemMemorySize;
        if (StopShowingAdsOnLowEndDevices)
        {
            if (deviceRam >= minRamSize_GB * 1024)
            {
                Initialized_SDK();
            }
            else
            {
                lowRam = true;
            }
        }
        else
        {
            Initialized_SDK();
        }
        // appsFlyerId = AppsFlyer.getAppsFlyerId();
    }


    public void RemoteConfigUpdate()
    {
        if (_UseRemoteConfig && !_TestMode)
        {
#if Remote_Config_Rizwan
            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {
#if UNITY_ANDROID
                _BannerAdId[i] = _fireBaseRemoteConfigHandler.AddOrUpdateKey(_BannerAdId[i], "Max_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
#elif UNITY_IOS
            _BannerAdId[i]=  _fireBaseRemoteConfigHandler.AddOrUpdateKey(_BannerAdId[i], "Max_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());         
#endif
            }
#if UNITY_ANDROID
            InterstitialAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_ANDROID_1", InterstitialAdUnitId, _InterstitialAdUnit.ANDROID_1.Trim());
            _InterstitialAdUnit.ANDROID_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_ANDROID_1", _InterstitialAdUnit.ANDROID_1, _InterstitialAdUnit.ANDROID_1.Trim());
            _InterstitialAdUnit.ANDROID_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_ANDROID_2", _InterstitialAdUnit.ANDROID_2, _InterstitialAdUnit.ANDROID_2.Trim());
            _InterstitialAdUnit.ANDROID_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_ANDROID_3", _InterstitialAdUnit.ANDROID_3, _InterstitialAdUnit.ANDROID_3.Trim());
#elif UNITY_IOS
            InterstitialAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "Max_Interstitial_IOS_1",InterstitialAdUnitId, _InterstitialAdUnit.IOS_1.Trim());
            _InterstitialAdUnit.IOS_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_IOS_1", _InterstitialAdUnit.IOS_1, _InterstitialAdUnit.IOS_1.Trim());
            _InterstitialAdUnit.IOS_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_IOS_2",_InterstitialAdUnit.IOS_2,  _InterstitialAdUnit.IOS_2.Trim());
            _InterstitialAdUnit.IOS_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Interstitial_IOS_3",_InterstitialAdUnit.IOS_3,  _InterstitialAdUnit.IOS_3.Trim());
#endif

#if UNITY_ANDROID
            RewardedAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Rewarded_ANDROID_1", RewardedAdUnitId, _RewardedAdUnit.ANDROID_1.Trim());
            _RewardedAdUnit.ANDROID_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Rewarded_ANDROID_1", _RewardedAdUnit.ANDROID_1, _RewardedAdUnit.ANDROID_1.Trim());
            _RewardedAdUnit.ANDROID_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Rewarded_ANDROID_2", _RewardedAdUnit.ANDROID_2, _RewardedAdUnit.ANDROID_2.Trim());
            _RewardedAdUnit.ANDROID_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Rewarded_ANDROID_3", _RewardedAdUnit.ANDROID_3, _RewardedAdUnit.ANDROID_3.Trim());
#elif UNITY_IOS
            RewardedAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "Max_Rewarded_IOS_1",RewardedAdUnitId, _RewardedAdUnit.IOS_1.Trim());
            _RewardedAdUnit.IOS_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Rewarded_IOS_1",_RewardedAdUnit.IOS_1,  _RewardedAdUnit.IOS_1.Trim());
            _RewardedAdUnit.IOS_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_Rewarded_IOS_2",_RewardedAdUnit.IOS_2,  _RewardedAdUnit.IOS_2.Trim());
            _RewardedAdUnit.IOS_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "Max_Rewarded_IOS_3",_RewardedAdUnit.IOS_3, _RewardedAdUnit.IOS_3.Trim());
#endif
#if UNITY_ANDROID
            RewardedInterstitialAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_ANDROID_1", RewardedInterstitialAdUnitId, _RewardedInterstitialAdUnit.ANDROID_1.Trim());
            _RewardedInterstitialAdUnit.ANDROID_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_ANDROID_1", _RewardedInterstitialAdUnit.ANDROID_1, _RewardedInterstitialAdUnit.ANDROID_1.Trim());
            _RewardedInterstitialAdUnit.ANDROID_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_ANDROID_2", _RewardedInterstitialAdUnit.ANDROID_2, _RewardedInterstitialAdUnit.ANDROID_2.Trim());
            _RewardedInterstitialAdUnit.ANDROID_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_ANDROID_3", _RewardedInterstitialAdUnit.ANDROID_3, _RewardedInterstitialAdUnit.ANDROID_3.Trim());
#elif UNITY_IOS
            RewardedInterstitialAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_IOS_1",RewardedInterstitialAdUnitId,  _RewardedInterstitialAdUnit.IOS_1.Trim());
            _RewardedInterstitialAdUnit.IOS_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_IOS_1",_RewardedInterstitialAdUnit.IOS_1, _RewardedInterstitialAdUnit.IOS_1.Trim());
            _RewardedInterstitialAdUnit.IOS_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_IOS_2",_RewardedInterstitialAdUnit.IOS_2,  _RewardedInterstitialAdUnit.IOS_2.Trim());
            _RewardedInterstitialAdUnit.IOS_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_RewardedInterstital_IOS_3",_RewardedInterstitialAdUnit.IOS_3, _RewardedInterstitialAdUnit.IOS_3.Trim());     
#endif
#if UNITY_ANDROID
            AppOpenAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_ANDROID_1", AppOpenAdUnitId, _AppOpenAdUnit.ANDROID_1.Trim());
            _AppOpenAdUnit.ANDROID_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_ANDROID_1", _AppOpenAdUnit.ANDROID_1, _AppOpenAdUnit.ANDROID_1.Trim());
            _AppOpenAdUnit.ANDROID_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_ANDROID_2", _AppOpenAdUnit.ANDROID_2, _AppOpenAdUnit.ANDROID_2.Trim());
            _AppOpenAdUnit.ANDROID_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_ANDROID_3", _AppOpenAdUnit.ANDROID_3, _AppOpenAdUnit.ANDROID_3.Trim());
#elif UNITY_IOS
            AppOpenAdUnitId = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_IOS_1",AppOpenAdUnitId,  _AppOpenAdUnit.IOS_1.Trim());
            _AppOpenAdUnit.IOS_1 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_IOS_1",_AppOpenAdUnit.IOS_1,  _AppOpenAdUnit.IOS_1.Trim());
            _AppOpenAdUnit.IOS_2 = _fireBaseRemoteConfigHandler.AddOrUpdateKey( "Max_AppOpen_IOS_2",_AppOpenAdUnit.IOS_2, _AppOpenAdUnit.IOS_2.Trim());
            _AppOpenAdUnit.IOS_3 = _fireBaseRemoteConfigHandler.AddOrUpdateKey("Max_AppOpen_IOS_3",_AppOpenAdUnit.IOS_3,  _AppOpenAdUnit.IOS_3.Trim());
#endif
            InitiliazeStuff();
#else
            PrintStatus("Import Firebase Remote Config First", true);
#endif
        }
    }
    public void JsonDataUpdate()
    {

#if Remote_Config_Rizwan
        tempdict = new Dictionary<string, object>();
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            tempdict.Add("Max_Banner_ANDROID_" + i, _BannerAdUnit[i].ANDROID.Trim());
            tempdict.Add("Max_Banner_IOS_" + i, _BannerAdUnit[i].IOS.Trim());
        }
        tempdict.Add("Max_Interstitial_ANDROID_1", _InterstitialAdUnit.ANDROID_1.Trim());
        tempdict.Add("Max_Interstitial_ANDROID_2", _InterstitialAdUnit.ANDROID_2.Trim());
        tempdict.Add("Max_Interstitial_ANDROID_3", _InterstitialAdUnit.ANDROID_2.Trim());
        tempdict.Add("Max_Interstitial_IOS_1", _InterstitialAdUnit.IOS_1.Trim());
        tempdict.Add("Max_Interstitial_IOS_2", _InterstitialAdUnit.IOS_2.Trim());
        tempdict.Add("Max_Interstitial_IOS_3", _InterstitialAdUnit.IOS_3.Trim());

        tempdict.Add("Max_Rewarded_ANDROID_1", _RewardedAdUnit.ANDROID_1.Trim());
        tempdict.Add("Max_Rewarded_ANDROID_2", _RewardedAdUnit.ANDROID_2.Trim());
        tempdict.Add("Max_Rewarded_ANDROID_3", _RewardedAdUnit.ANDROID_3.Trim());
        tempdict.Add("Max_Rewarded_IOS_1", _RewardedAdUnit.IOS_1.Trim());
        tempdict.Add("Max_Rewarded_IOS_2", _RewardedAdUnit.IOS_2.Trim());
        tempdict.Add("Max_Rewarded_IOS_3", _RewardedAdUnit.IOS_2.Trim());

        tempdict.Add("Max_RewardedInterstital_ANDROID_1", _RewardedInterstitialAdUnit.ANDROID_1.Trim());
        tempdict.Add("Max_RewardedInterstital_ANDROID_2", _RewardedInterstitialAdUnit.ANDROID_2.Trim());
        tempdict.Add("Max_RewardedInterstital_ANDROID_3", _RewardedInterstitialAdUnit.ANDROID_3.Trim());
        tempdict.Add("Max_RewardedInterstital_IOS_1", _RewardedInterstitialAdUnit.IOS_1.Trim());
        tempdict.Add("Max_RewardedInterstital_IOS_2", _RewardedInterstitialAdUnit.IOS_2.Trim());
        tempdict.Add("Max_RewardedInterstital_IOS_3", _RewardedInterstitialAdUnit.IOS_3.Trim());

        tempdict.Add("Max_AppOpen_ANDROID_1", _AppOpenAdUnit.ANDROID_1.Trim());
        tempdict.Add("Max_AppOpen_ANDROID_2", _AppOpenAdUnit.ANDROID_2.Trim());
        tempdict.Add("Max_AppOpen_ANDROID_3", _AppOpenAdUnit.ANDROID_3.Trim());
        tempdict.Add("Max_AppOpen_IOS_1", _AppOpenAdUnit.IOS_1.Trim());
        tempdict.Add("Max_AppOpen_IOS_2", _AppOpenAdUnit.IOS_2.Trim());
        tempdict.Add("Max_AppOpen_IOS_3", _AppOpenAdUnit.IOS_3.Trim());
#else
        PrintStatus("Import Firebase Remote Config First", true);
#endif
    }

    void Initialized_SDK()
    {
        MaxSdkCallbacks.OnSdkInitializedEvent += sdkConfiguration =>
        {
            // AppLovin SDK is initialized, configure and start loading ads.        
            PrintStatus("MAX SDK Initialized", false);
            InitializeInterstitialAds();
            InitializeRewardedAds();
            InitializeRewardedInterstitialAds();
            InitializeAppOpenAds();

            StartCoroutine(nameof(StartSecond));

            for (int i = 0; i < _BannerAdId.Length; i++)
            {
                _bannerLoaded[i] = false;
                _bannerShowing[i] = false;
                _bannerShowingForAllADs[i] = false;

                bannerAdAttempt[i] = 0;
                InitializeBannerAds(i);
            }

            //for (int i = 0; i < _BannerAdId.Length; i++)
            //{
            //    Load_BannerAd(i);
            //}
            // Initialize Adjust SDK
            //AdjustConfig adjustConfig = new AdjustConfig("YourAppToken", AdjustEnvironment.Sandbox);
            //Adjust.start(adjustConfig);
        };

        MaxSdk.InitializeSdk();
    }

    #region Banner

    //InitializeBannerAds
    private void InitializeBannerAds(int index)
    {
        switch (_BannerAdUnit[index].bannerSize)
        {
            case _bannerAd.BannerSize.Banner:
                MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerLoadedEvent;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerFailedtoLoadEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

                MaxSdk.SetBannerExtraParameter(_BannerAdId[index], "adaptive_banner", "false");
                MaxSdk.CreateBanner(_BannerAdId[index], _BannerAdUnit[index].bannerPosition);
                break;

            case _bannerAd.BannerSize.Adaptive:
                MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerLoadedEvent;
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerFailedtoLoadEvent;
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

                MaxSdkUtils.GetAdaptiveBannerHeight();
                MaxSdk.SetBannerExtraParameter(_BannerAdId[index], "adaptive_banner", "true");
                MaxSdk.CreateBanner(_BannerAdId[index], _BannerAdUnit[index].bannerPosition);
                break;

            case _bannerAd.BannerSize.MediumRectangle:
                MaxSdkCallbacks.MRec.OnAdLoadedEvent += OnBannerLoadedEvent;
                MaxSdkCallbacks.MRec.OnAdLoadFailedEvent += OnBannerFailedtoLoadEvent;
                MaxSdkCallbacks.MRec.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;

                MaxSdk.SetMRecExtraParameter(_BannerAdId[index], "adaptive_banner", "true");
                MaxSdk.CreateMRec(_BannerAdId[index], _BannerAdUnit[index].adviewPosition);
                break;
            default:
                PrintStatus("Nothing _BannerAdUnit[index].bannerSize", true);
                break;
        }
        // Set background or background color for banners to be fully functional.
        MaxSdk.SetBannerBackgroundColor(_BannerAdId[index], Color.black);
    }
    /////
    /// <summary>

    public void Load_BannerAd(int index)
    {
        if (_BannerAdId[index] != string.Empty)
        {
            //MaxSdk.LoadBanner(_BannerAdId[index]);
            switch (_BannerAdUnit[index].bannerSize)
            {
                case _bannerAd.BannerSize.Banner:

                    //  MaxSdk.StopBannerAutoRefresh(_BannerAdId[index]);
                    MaxSdk.LoadBanner(_BannerAdId[index]);
                    MaxSdk.StartBannerAutoRefresh(_BannerAdId[index]);
                    break;
                case _bannerAd.BannerSize.Adaptive:

                    // MaxSdk.StopBannerAutoRefresh(_BannerAdId[index]);
                    MaxSdk.LoadBanner(_BannerAdId[index]);
                    MaxSdk.StartBannerAutoRefresh(_BannerAdId[index]);
                    break;

                case _bannerAd.BannerSize.MediumRectangle:
                    // MaxSdk.StopMRecAutoRefresh(_BannerAdId[index]);
                    MaxSdk.LoadMRec(_BannerAdId[index]);
                    MaxSdk.StartMRecAutoRefresh(_BannerAdId[index]);
                    break;
                default:
                    // Handle the default case if bannerSize doesn't match any specified case
                    break;
            }
        }
        else
        {
            PrintStatus("_BannerAdId at index" + index + " is string.Empty", true);

        }
    }




    public void Show_BannerAd(int index)
    {
        if (CanShowBanner(index) == false)
        {
            PrintStatus("CanShowBanner is False", true);
            return;
        }

        if (index >= 0 && index < _BannerAdUnit.Length)
        {
            if (_BannerAdId[index] != string.Empty && _bannerLoaded[index])
            {
                // MaxSdk.ShowBanner(_BannerAdId[index]);
                switch (_BannerAdUnit[index].bannerSize)
                {
                    case _bannerAd.BannerSize.Banner:
                        MaxSdk.ShowBanner(_BannerAdId[index]);
                        break;
                    case _bannerAd.BannerSize.Adaptive:
                        MaxSdk.ShowBanner(_BannerAdId[index]);
                        break;

                    case _bannerAd.BannerSize.MediumRectangle:
                        MaxSdk.ShowMRec(_BannerAdId[index]);
                        break;
                    default:
                        PrintStatus("Nothing _BannerAdUnit[index].bannerSize", true);
                        break;
                }

                _bannerShowing[index] = true;

                //GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Banner, "ADMOB", " _BannerAdId " + _BannerAdId[index]);
                //FirebaseAnalyticsHandler.Instance.LogFirebaseEvent("Ad Action = Show",
                //                                                   ", Ad Type = Banner ",
                //                                                   ", Formate = " + "Banner" + "" +
                //                                                   ", Banner Ad Id = " + _BannerAdId[index]);
            }
            else
            {
                _bannerShowing[index] = false;
                Load_BannerAd(index);
            }
        }
        else
        {
            PrintStatus("_bannerViews " + index + " is Out of bound", true);
        }
    }

    // Hide a specific banner ad by index
    public void Hide_BannerAd(int index)
    {
        PrintStatus("_Banner Hide called", false);
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
        if (_BannerAdId == null)
        {
            PrintStatus("_BannerAdId is null", true);
            return;
        }
        if (index >= 0 && index < _BannerAdId.Length)
        {
            switch (_BannerAdUnit[index].bannerSize)
            {
                case _bannerAd.BannerSize.Banner:
                    MaxSdk.HideBanner(_BannerAdId[index]);
                    break;
                case _bannerAd.BannerSize.Adaptive:
                    MaxSdk.HideBanner(_BannerAdId[index]);
                    break;

                case _bannerAd.BannerSize.MediumRectangle:
                    MaxSdk.HideMRec(_BannerAdId[index]);
                    break;
                default:
                    PrintStatus("Nothing _BannerAdUnit[index].bannerSize", true);
                    break;
            }
            _bannerShowing[index] = false;
            _bannerShowingForAllADs[index] = false;
        }
        else
        {
            PrintStatus("_bannerViews " + index + " is Out of bound", true);
        }
    }
    public void Hide_BannerAdForADs(int index)
    {

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
        if (_BannerAdId[index] == string.Empty)
        {
            PrintStatus("_BannerAdId is null", true);
            return;
        }
        if (index >= 0 && index < _BannerAdId.Length)
        {
            switch (_BannerAdUnit[index].bannerSize)
            {
                case _bannerAd.BannerSize.Banner:
                    MaxSdk.HideBanner(_BannerAdId[index]);
                    break;
                case _bannerAd.BannerSize.Adaptive:
                    MaxSdk.HideBanner(_BannerAdId[index]);
                    break;

                case _bannerAd.BannerSize.MediumRectangle:
                    MaxSdk.HideMRec(_BannerAdId[index]);
                    break;
                default:
                    PrintStatus("Nothing _BannerAdUnit[index].bannerSize", true);
                    break;
            }

            _bannerShowing[index] = false;


        }
        else
        {
            PrintStatus("_bannerViews " + index + " is Out of bound", true);
        }
    }
    public void HideActiveBanners()
    {
        PrintStatus("HideActiveBanners", false);
        StopCoroutine(nameof(ShowActiveBanners_Co));

        if (_BannerAdId == null)
        {
            PrintStatus("_BannerAdId is null", true);
            return;
        }
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_bannerShowing[i])
            {
                _bannerShowingForAllADs[i] = true;
                Hide_BannerAdForADs(i);
            }
        }
    }

    bool CanShowBanner(int index)
    {
        if (lowRam)
        {
            PrintStatus("LowRAM", true);
            return false;
        }
        if (_BannerAdId.Length == 0)
        {
            PrintStatus("_BannerAdUnit is Null", true);
            return false;
        }
        if (string.IsNullOrWhiteSpace(_BannerAdId[index]))
        {
            PrintStatus("_BannerAdId is IsNullOrWhiteSpace", true);
            return false;
        }
        if (_bannerShowingForAllADs[index] == true)
        {
            PrintStatus("_bannerShowingForAllADs " + _bannerShowingForAllADs[index] + "is true", true);
            return false;
        }

        PrintStatus("_Banner Show called", false);
        return true;

    }
    public void ShowActiveBanners()
    {
        StartCoroutine(nameof(ShowActiveBanners_Co));
    }
    IEnumerator ShowActiveBanners_Co()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        if (AnyFullScreenAdShowing_Max() == false)
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
            PrintStatus("AnyFullScreenAdShowing_Max returned False, means Any Full Screen Ad Already Active", true);
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
        if (_BannerAdUnit.Length == 0)
        {
            PrintStatus("_BannerAdUnit " + index + " is Null", true);
            return;
        }

        if (index >= 0 && index < _BannerAdId.Length)
        {
            switch (_BannerAdUnit[index].bannerSize)
            {
                case _bannerAd.BannerSize.Banner:
                    MaxSdk.DestroyBanner(_BannerAdId[index]);
                    break;
                case _bannerAd.BannerSize.Adaptive:
                    MaxSdk.DestroyBanner(_BannerAdId[index]);
                    break;

                case _bannerAd.BannerSize.MediumRectangle:
                    MaxSdk.DestroyMRec(_BannerAdId[index]);
                    break;
                default:
                    PrintStatus("Nothing _BannerAdUnit[index].bannerSize", true);
                    break;
            }
            _bannerLoaded[index] = false;
            _bannerShowing[index] = false;
            _bannerShowingForAllADs[index] = false;
        }
        else
        {
            PrintStatus("_bannerViews " + index + " is Out of bound", true);
        }
    }



    ///////
    /// </summary>
    /// <param name="adUnitId"></param>
    /// <param name="adInfo"></param>


    private void OnBannerLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_BannerAdId[i] == adUnitId)
            {
                PrintStatus("Banner loaded", false);

                _bannerLoaded[i] = true;
            }
        }
    }

    private void OnBannerFailedtoLoadEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Banner ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_BannerAdId[i] == adUnitId)
            {

                // double retryDelay = Math.Pow(2, Math.Min(2, bannerAdAttempt[i]));
                //Invoke(nameof(RetryBannerAdLoad), (float)retryDelay);
                //StartCoroutine(RetryBannerAdLoad(i, retryDelay));

                _bannerLoaded[i] = false;
                //interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
                PrintStatus("Banner failed to load with error code: " + errorInfo.Code, true);
            }

        }
    }
    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if Firebase_Rizwan
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);

#endif
        // Banner ad revenue paid. Use this callback to track user revenue.
        Debug.Log("Banner ad revenue paid");

        //// Ad revenue
        //double revenue = adInfo.Revenue;

        ////Miscellaneous data
        //string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        //string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        //TrackAdRevenue(adInfo);

        //AppFlyer for Mintegral Roas
        // Replace with your attribution platform name, for example, "Adjust", and replace "userid" with your attribution platform UID
#if MAX
        MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER, appsFlyerId);
        mBridgeRevenueParamsEntity.SetMaxAdInfo(adInfo);
        MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
#endif
    }

    //IEnumerator RetryBannerAdLoad(int index, double delay)
    //{
    //    yield return new WaitForSeconds((float)delay);
    //    bannerAdAttempt[index]++;
    //    bannerAdAttempt[index] = bannerAdAttempt[index] < 3 ? bannerAdAttempt[index] : 0;
    //    // SetBannerAdUnitId(bannerAdAttempt[index]);
    //    Load_BannerAd(index);
    //}


    private void SetBannerAdUnitId()
    {
        _BannerAdId = new string[_BannerAdUnit.Length];
        if (_TestMode)
        {
            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {
#if UNITY_ANDROID

                _BannerAdId[i] = "Test InterStital ID for Android";
#elif UNITY_IOS
            _BannerAdId[i] = "Test InterStital ID for IOS";
#endif
            }
        }
        else
        {
            for (int i = 0; i < _BannerAdUnit.Length; i++)
            {


#if UNITY_ANDROID
                _BannerAdId[i] = _BannerAdUnit[i].ANDROID.Trim();
#elif UNITY_IOS
            _BannerAdId[i] = _BannerAdUnit[i].IOS.Trim();                   
#endif

            }
        }
        _bannerLoaded = new bool[_BannerAdId.Length];
        _bannerShowing = new bool[_BannerAdId.Length];
        bannerAdAttempt = new int[_BannerAdId.Length];
        _bannerShowingForAllADs = new bool[_BannerAdId.Length];

    }

    private void OnBannerDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is hidden. Pre-load the next ad
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_BannerAdId[i] == adUnitId)
            {
                PrintStatus("Max Banner Displayed", false);

                _bannerShowing[i] = true;
            }
        }

    }

    private void OnBannerDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad is hidden. Pre-load the next ad
        for (int i = 0; i < _BannerAdUnit.Length; i++)
        {
            if (_BannerAdId[i] == adUnitId)
            {
                PrintStatus("Max Banner dismissed", true);
                _bannerShowing[i] = false;
                _bannerLoaded[i] = false;
                // SetBannerAdUnitId(0);
                Load_BannerAd(i);
            }
        }
    }
    private void OnBannerRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Banner ad revenue paid. Use this callback to track user revenue.
        PrintStatus("Banner revenue paid", false);
#if Firebase_Rizwan
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);

#endif

#if MAX
        //AppFlyer for Mintegral Roas
        // Replace with your attribution platform name, for example, "Adjust", and replace "userid" with your attribution platform UID
        MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER, appsFlyerId);
        mBridgeRevenueParamsEntity.SetMaxAdInfo(adInfo);
        MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
#endif
        // Ad revenue
        //double revenue = adInfo.Revenue;
        ////Miscellaneous data
        //string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        //string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        //TrackAdRevenue(adInfo);
    }

    #endregion

    #region Interstitial Ad Methods
    private void InitializeInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += InterstitialFailedToDisplayEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialDismissedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialRevenuePaidEvent;
        LoadInterstitial();
        // Load the first interstitial
    }
    public void LoadInterstitial()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max: LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(InterstitialAdUnitId))
        {
            PrintStatus("_BannerAdId is IsNullOrWhiteSpace", true);
            return;
        }
        MaxSdk.LoadInterstitial(InterstitialAdUnitId);
        PrintStatus("Load Interstital for Max Interstitail called", false);
    }
    public void ShowInterstitialAd()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max: LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(InterstitialAdUnitId))
        {
            PrintStatus("_BannerAdId is IsNullOrWhiteSpace", true);
            return;
        }
        if (MaxSdk.IsInterstitialReady(InterstitialAdUnitId))
        {
            MaxSdk.ShowInterstitial(InterstitialAdUnitId);
            PrintStatus("Max Interstitail Ad Showing", false);
            FullScreenAd_Shown?.Invoke();

        }
        else
        {
            PrintStatus("Called Show Max Interstitail Ad But Not Loaded", true);
            LoadInterstitial();
        }
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isInterstitialAdReady = true;
        SetInterstitialAdUnitId(0);
        PrintStatus("Interstitial loaded", false);
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        double retryDelay = Math.Pow(2, Math.Min(2, interstitialAdAttempt));
        Invoke(nameof(RetryInterstitialAdLoad), (float)retryDelay);

        isInterstitialAdReady = false;
        //interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
        PrintStatus("Interstitial Ad Failed to load, Reason: " + errorInfo, true);
        PrintStatus("Interstitial failed to load with error code: " + errorInfo.Code, true);

    }
    void RetryInterstitialAdLoad()
    {
        interstitialAdAttempt++;
        interstitialAdAttempt = interstitialAdAttempt < 3 ? interstitialAdAttempt : 0;
        SetInterstitialAdUnitId(interstitialAdAttempt);
        LoadInterstitial();
    }


    private void SetInterstitialAdUnitId(int index)
    {
        if (_TestMode)
        {
#if UNITY_ANDROID

            InterstitialAdUnitId = "Test InterStital ID for Android";
#elif UNITY_IOS
            InterstitialAdUnitId = "Test InterStital ID for IOS";
#endif
        }
        else
        {
            switch (index)
            {
                case 0:
#if UNITY_ANDROID
                    InterstitialAdUnitId = _InterstitialAdUnit.ANDROID_1.Trim();
#elif UNITY_IOS
            InterstitialAdUnitId = _InterstitialAdUnit.IOS_1.Trim();                   
#endif
                    break;
                case 1:
#if UNITY_ANDROID
                    InterstitialAdUnitId = _InterstitialAdUnit.ANDROID_2.Trim();
#elif UNITY_IOS
            InterstitialAdUnitId = _InterstitialAdUnit.IOS_2.Trim();                   
#endif
                    break;
                case 2:
#if UNITY_ANDROID
                    InterstitialAdUnitId = _InterstitialAdUnit.ANDROID_3.Trim();
#elif UNITY_IOS
            InterstitialAdUnitId = _InterstitialAdUnit.IOS_3.Trim();                   
#endif
                    break;
            }
        }
    }
    private void InterstitialFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. We recommend loading the next ad
        PrintStatus("Interstitial failed to display with error code: " + errorInfo.Code, true);
        SetInterstitialAdUnitId(0);
        LoadInterstitial();
        isShowingInterstitial = false;
        isInterstitialAdReady = false;
        FullScreenAd_Closed?.Invoke();
    }
    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        PrintStatus("Max Interstitial Displayed", false);
        isShowingInterstitial = true;
        FullScreenAd_Shown?.Invoke();

    }

    private void OnInterstitialDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad
        PrintStatus("Max Interstitial dismissed", true);
        isShowingInterstitial = false;
        isInterstitialAdReady = false;
        SetInterstitialAdUnitId(0);
        LoadInterstitial();
        FullScreenAd_Closed?.Invoke();
    }
    private void OnInterstitialRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if Firebase_Rizwan
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif
#if MAX
        //AppFlyer for Mintegral Roas
        // Replace with your attribution platform name, for example, "Adjust", and replace "userid" with your attribution platform UID
        MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER, appsFlyerId);
        mBridgeRevenueParamsEntity.SetMaxAdInfo(adInfo);
        MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
#endif
        //    // Interstitial ad revenue paid. Use this callback to track user revenue.
        //    Debug.Log("Interstitial revenue paid");

        //    // Ad revenue
        //    double revenue = adInfo.Revenue;

        //    // Miscellaneous data
        //    string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        //    string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //    string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //    string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        //    //TrackAdRevenue(adInfo);
    }

    #endregion

    #region Rewarded Ad Methods

    private void InitializeRewardedAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdDismissedEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;

        // Load the first RewardedAd
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(RewardedAdUnitId))
        {
            PrintStatus("RewardedInterstitialID is IsNullOrWhiteSpace", true);
            return;
        }
        MaxSdk.LoadRewardedAd(RewardedAdUnitId);
    }


    public void ShowRewardedAd(Action reward, Action failed)
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(RewardedAdUnitId))
        {
            PrintStatus("RewardedInterstitialID is IsNullOrWhiteSpace", true);
            return;
        }
        if (MaxSdk.IsRewardedAdReady(RewardedAdUnitId))
        {
            MaxSdk.ShowRewardedAd(RewardedAdUnitId);
            _reward = reward;
            _failed = failed;
            rewardGiven = false;
            FullScreenAd_Shown?.Invoke();

        }
        else
        {
            LoadRewardedAd();
        }
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready to be shown. MaxSdk.IsRewardedAdReady(rewardedAdUnitId) will now return 'true'
        //rewardedStatusText.text = "Loaded";
        PrintStatus("Max Rewarded ad loaded", false);
        SetRewardedAdUnitId(0);
        isRewardedAdReady = true;

    }

    private void OnRewardedAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).

        double retryDelay = Math.Pow(2, Math.Min(3, rewardedAdAttempt));
        Invoke(nameof(RetryRewardedAdLoad), (float)retryDelay);
        isRewardedAdReady = false;
        //interstitialStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
        PrintStatus("Max Rewarded failed to load with error Reason: " + errorInfo, true);
        PrintStatus("Max Rewarded failed to load with error code: " + errorInfo.Code, true);

    }

    void RetryRewardedAdLoad()
    {
        rewardedAdAttempt++;
        rewardedAdAttempt = rewardedAdAttempt < 3 ? rewardedAdAttempt : 0;
        SetRewardedAdUnitId(rewardedAdAttempt);
        LoadRewardedAd();
    }

    private void SetRewardedAdUnitId(int index)
    {
        if (_TestMode)
        {
#if UNITY_ANDROID

            RewardedAdUnitId = "Test Rewarded ID for Android";
#elif UNITY_IOS
            RewardedAdUnitId = "Test Rewarded ID for IOS";
#endif
        }
        else
        {
            switch (index)
            {
                case 0:
#if UNITY_ANDROID
                    RewardedAdUnitId = _RewardedAdUnit.ANDROID_1.Trim();
#elif UNITY_IOS
            RewardedAdUnitId = _RewardedAdUnit.IOS_1.Trim();                   
#endif
                    break;
                case 1:
#if UNITY_ANDROID
                    RewardedAdUnitId = _RewardedAdUnit.ANDROID_2.Trim();
#elif UNITY_IOS
            RewardedAdUnitId = _RewardedAdUnit.IOS_2.Trim();                   
#endif
                    break;
                case 2:
#if UNITY_ANDROID
                    RewardedAdUnitId = _RewardedAdUnit.ANDROID_3.Trim();
#elif UNITY_IOS
            RewardedAdUnitId = _RewardedAdUnit.IOS_3.Trim();                   
#endif
                    break;
            }
        }
    }
    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {    // Rewarded ad failed to display. We recommend loading the next ad
        PrintStatus("Max Rewarded ad failed to display with error code: " + errorInfo.Code, true);
        SetRewardedAdUnitId(0);
        LoadRewardedAd();
        _failed?.Invoke();
        isShowingRewarded = false;
        isRewardedAdReady = false;

        FullScreenAd_Closed?.Invoke();

    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        PrintStatus("Max Rewarded ad displayed", false);
        isShowingRewarded = true;
        FullScreenAd_Shown?.Invoke();

    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        PrintStatus("Max Rewarded ad clicked", false);
    }

    private void OnRewardedAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        PrintStatus("Rewarded ad dismissed", true);
        SetRewardedAdUnitId(0);
        LoadRewardedAd();

        if (rewardGiven == false)
        {
            _failed?.Invoke();
        }
        isShowingRewarded = false;
        isRewardedAdReady = false;

        FullScreenAd_Closed?.Invoke();

    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        rewardGiven = true;
        _reward?.Invoke();
        // Rewarded ad was displayed and user should receive the reward
        PrintStatus("Rewarded ad received reward", true);
    }


    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if Firebase_Rizwan
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif
#if MAX
        //AppFlyer for Mintegral Roas
        // Replace with your attribution platform name, for example, "Adjust", and replace "userid" with your attribution platform UID
        MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER, appsFlyerId);
        mBridgeRevenueParamsEntity.SetMaxAdInfo(adInfo);
        MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
#endif

        //    // Rewarded ad revenue paid. Use this callback to track user revenue.
        //    Debug.Log("Rewarded ad revenue paid");

        //    // Ad revenue
        //    double revenue = adInfo.Revenue;

        //    // Miscellaneous data
        //    string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        //    string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //    string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //    string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        //    //TrackAdRevenue(adInfo);
    }

    #endregion

    #region Rewarded Interstitial Ad Methods

    private void InitializeRewardedInterstitialAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.RewardedInterstitial.OnAdLoadedEvent += OnRewardedInterstitialAdLoadedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdLoadFailedEvent += OnRewardedInterstitialAdFailedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayFailedEvent += OnRewardedInterstitialAdFailedToDisplayEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdDisplayedEvent += OnRewardedInterstitialAdDisplayedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdClickedEvent += OnRewardedInterstitialAdClickedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdHiddenEvent += OnRewardedInterstitialAdDismissedEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdReceivedRewardEvent += OnRewardedInterstitialAdReceivedRewardEvent;
        MaxSdkCallbacks.RewardedInterstitial.OnAdRevenuePaidEvent += OnRewardedInterstitialAdRevenuePaidEvent;

        // Load the first RewardedInterstitialAd
        LoadRewardedInterstitialAd();
    }

    public void LoadRewardedInterstitialAd()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load RewardedInterstitial Max LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(RewardedInterstitialAdUnitId))
        {
            PrintStatus("RewardedInterstitialID is IsNullOrWhiteSpace", true);
            return;
        }
        //rewardedInterstitialStatusText.text = "Loading...";
        MaxSdk.LoadRewardedInterstitialAd(RewardedInterstitialAdUnitId);
    }

    public void ShowRewardedInterstitialAd(Action reward, Action failed)
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(RewardedInterstitialAdUnitId))
        {
            PrintStatus("RewardedInterstitialID is IsNullOrWhiteSpace", true);
            return;
        }
        if (MaxSdk.IsRewardedInterstitialAdReady(RewardedInterstitialAdUnitId))
        {
            //rewardedInterstitialStatusText.text = "Showing";
            MaxSdk.ShowRewardedInterstitialAd(RewardedInterstitialAdUnitId);
            _reward = reward;
            _failed = failed;
            rewardGiven = false;
            FullScreenAd_Shown?.Invoke();
        }
        else
        {
            LoadRewardedInterstitialAd();
        }
    }

    private void OnRewardedInterstitialAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad is ready to be shown. MaxSdk.IsRewardedInterstitialAdReady(rewardedInterstitialAdUnitId) will now return 'true'
        //rewardedInterstitialStatusText.text = "Loaded";
        PrintStatus("Rewarded interstitial ad loaded", false);

        SetRewardedInterstitialAdUnitId(0);
        isRewardedInterstitialAdReady = true;


    }

    private void OnRewardedInterstitialAdFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded interstitial ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).    

        double retryDelay = Math.Pow(2, Math.Min(3, rewardedInterstitialAdAttempt));
        Invoke(nameof(RetryRewardedInterstitialAdLoad), (float)retryDelay);
        isRewardedInterstitialAdReady = false;
        isRewardedInterstitialAdReady = false;
        //PrintStatus("Max RewardedInterstitial failed to load with error Reason: " + errorInfo, true);
        //PrintStatus("Max RewardedInterstitial failed to load with error code: " + errorInfo.Code, true);
    }
    void RetryRewardedInterstitialAdLoad()
    {
        rewardedInterstitialAdAttempt++;
        rewardedInterstitialAdAttempt = rewardedInterstitialAdAttempt < 3 ? rewardedInterstitialAdAttempt : 0;
        SetRewardedInterstitialAdUnitId(rewardedInterstitialAdAttempt);
        LoadRewardedInterstitialAd();
    }


    private void SetRewardedInterstitialAdUnitId(int index)
    {
        if (_TestMode)
        {
#if UNITY_ANDROID

            RewardedInterstitialAdUnitId = "Test RewardedInterstital ID for Android";
#elif UNITY_IOS
            RewardedInterstitialAdUnitId = "Test RewardedInterstital ID for IOS";
#endif
        }
        else
        {

            switch (index)
            {
                case 0:
#if UNITY_ANDROID
                    RewardedInterstitialAdUnitId = _RewardedInterstitialAdUnit.ANDROID_1.Trim();
#elif UNITY_IOS
            RewardedInterstitialAdUnitId = _RewardedInterstitialAdUnit.IOS_1.Trim();                   
#endif
                    break;
                case 1:
#if UNITY_ANDROID
                    RewardedInterstitialAdUnitId = _RewardedInterstitialAdUnit.ANDROID_2.Trim();
#elif UNITY_IOS
            RewardedInterstitialAdUnitId = _RewardedInterstitialAdUnit.IOS_2.Trim();                   
#endif
                    break;
                case 2:
#if UNITY_ANDROID
                    RewardedInterstitialAdUnitId = _RewardedInterstitialAdUnit.ANDROID_3.Trim();
#elif UNITY_IOS
            RewardedInterstitialAdUnitId = _RewardedInterstitialAdUnit.IOS_3.Trim();                   
#endif
                    break;
            }
        }
    }
    private void OnRewardedInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad failed to display. We recommend loading the next ad
        Debug.Log("Rewarded interstitial ad failed to display with error code: " + errorInfo.Code);

        SetRewardedInterstitialAdUnitId(0);
        LoadRewardedInterstitialAd();

        _failed?.Invoke();
        isShowingRewardedInterstitial = false;
        isRewardedInterstitialAdReady = false;
        FullScreenAd_Closed?.Invoke();
    }
    private void OnRewardedInterstitialAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        PrintStatus("Rewarded interstitial ad displayed", true);
        isShowingRewardedInterstitial = true;
        FullScreenAd_Shown?.Invoke();
    }

    private void OnRewardedInterstitialAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        PrintStatus("Rewarded interstitial ad clicked", true);
    }

    private void OnRewardedInterstitialAdDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad is hidden. Pre-load the next ad
        PrintStatus("Rewarded interstitial ad dismissed", true);

        SetRewardedInterstitialAdUnitId(0);
        LoadRewardedInterstitialAd();
        if (rewardGiven == false)
        {
            _failed?.Invoke();
        }
        isShowingRewardedInterstitial = false;
        isRewardedInterstitialAdReady = false;
        FullScreenAd_Closed?.Invoke();

    }

    private void OnRewardedInterstitialAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded interstitial ad was displayed and user should receive the reward
        PrintStatus("Rewarded interstitial ad received reward", true);
        rewardGiven = true;
        _reward?.Invoke();
    }

    private void OnRewardedInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if Firebase_Rizwan
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif

#if MAX
        //    //TrackAdRevenue(adInfo);
        //AppFlyer for Mintegral Road
        // Replace with your attribution platform name, for example, "Adjust", and replace "userid" with your attribution platform UID
        MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER, appsFlyerId);
        mBridgeRevenueParamsEntity.SetMaxAdInfo(adInfo);
        MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
#endif

        //    // Rewarded interstitial ad revenue paid. Use this callback to track user revenue.
        //    Debug.Log("Rewarded interstitial ad revenue paid");

        //    // Ad revenue
        //    double revenue = adInfo.Revenue;

        //    // Miscellaneous data
        //    string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        //    string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //    string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //    string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
    }
    #endregion

    #region AppOpen Ad Methods



    private void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus)
        {
            ShowAdIfReady();
        }
    }

    private void InitializeAppOpenAds()
    {
        // Attach callbacks
        MaxSdkCallbacks.AppOpen.OnAdLoadedEvent += OnAppOpenLoadedEvent;
        MaxSdkCallbacks.AppOpen.OnAdLoadFailedEvent += OnAppOpenLoadFailedEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayFailedEvent += AppOpenFailedToDisplayEvent;
        MaxSdkCallbacks.AppOpen.OnAdDisplayedEvent += OnAppOpenDisplayedEvent;
        MaxSdkCallbacks.AppOpen.OnAdHiddenEvent += OnAppOpenDismissedEvent;
        MaxSdkCallbacks.AppOpen.OnAdRevenuePaidEvent += OnAppOpenRevenuePaidEvent;
        LoadAppOpen();
        // Load the first AppOpen
    }
    public void LoadAppOpen()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load AppOpen Max: LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(AppOpenAdUnitId))
        {
            PrintStatus("Max_AppOpen is IsNullOrWhiteSpace", true);
            return;
        }
        if (!CanShowAppOpen)
        {
            PrintStatus("CanShowAppOpen =false", true);
            return;
        }
        MaxSdk.LoadAppOpenAd(AppOpenAdUnitId);
        PrintStatus("Load AppOpen for Max Interstitail called", false);
    }

    public void ShowAdIfReady()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load AppOpen Max: LowRAM", true);
            return;
        }
        if (string.IsNullOrWhiteSpace(AppOpenAdUnitId))
        {
            PrintStatus("_BannerAdId is IsNullOrWhiteSpace", true);
            return;
        }
        if (!CanShowAppOpen)
        {
            PrintStatus("CanShowAppOpen =false", true);
            return;
        }
        if (MaxSdk.IsAppOpenAdReady(AppOpenAdUnitId))
        {
            MaxSdk.ShowAppOpenAd(AppOpenAdUnitId);
            PrintStatus("Max Interstitail Ad Showing", false);
            FullScreenAd_Shown?.Invoke();

        }
        else
        {
            PrintStatus("Called Show Max Interstitail Ad But Not Loaded", true);
            LoadAppOpen();
        }
    }


    private void OnAppOpenLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isAppOpenAdReady = true;
        SetAppOpenAdUnitId(0);
        if (CanShowAppOpenAtStartup)
        {
            ShowAdIfReady();
            CanShowAppOpenAtStartup = false;
        }

        PrintStatus("AppOpen loaded", false);
    }

    private void OnAppOpenLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // AppOpen ad failed to load. We recommend retrying with exponentially higher delays up to a maximum delay (in this case 64 seconds).
        double retryDelay = Math.Pow(2, Math.Min(2, AppOpenAdAttempt));
        Invoke(nameof(RetryAppOpenAdLoad), (float)retryDelay);

        isAppOpenAdReady = false;
        //AppOpenStatusText.text = "Load failed: " + errorInfo.Code + "\nRetrying in " + retryDelay + "s...";
        PrintStatus("AppOpen Ad Failed to load, Reason: " + errorInfo, true);
        PrintStatus("AppOpen failed to load with error code: " + errorInfo.Code, true);



    }
    void RetryAppOpenAdLoad()
    {
        AppOpenAdAttempt++;
        AppOpenAdAttempt = AppOpenAdAttempt < 3 ? AppOpenAdAttempt : 0;
        SetAppOpenAdUnitId(AppOpenAdAttempt);
        LoadAppOpen();
    }


    private void SetAppOpenAdUnitId(int index)
    {
        if (_TestMode)
        {
#if UNITY_ANDROID

            AppOpenAdUnitId = "Test InterStital ID for Android";
#elif UNITY_IOS
            AppOpenAdUnitId = "Test InterStital ID for IOS";
#endif
        }
        else
        {
            switch (index)
            {
                case 0:
#if UNITY_ANDROID
                    AppOpenAdUnitId = _AppOpenAdUnit.ANDROID_1.Trim();
#elif UNITY_IOS
            AppOpenAdUnitId = _AppOpenAdUnit.IOS_1.Trim();                   
#endif
                    break;
                case 1:
#if UNITY_ANDROID
                    AppOpenAdUnitId = _AppOpenAdUnit.ANDROID_2.Trim();
#elif UNITY_IOS
            AppOpenAdUnitId = _AppOpenAdUnit.IOS_2.Trim();                   
#endif
                    break;
                case 2:
#if UNITY_ANDROID
                    AppOpenAdUnitId = _AppOpenAdUnit.ANDROID_3.Trim();
#elif UNITY_IOS
            AppOpenAdUnitId = _AppOpenAdUnit.IOS_3.Trim();                   
#endif
                    break;
            }
        }
    }
    private void AppOpenFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // AppOpen ad failed to display. We recommend loading the next ad
        //PrintStatus("AppOpen failed to display with error code: " + errorInfo.Code, true);
        SetAppOpenAdUnitId(0);
        LoadAppOpen();
        isShowingAppOpen = false;
        isAppOpenAdReady = false;
        FullScreenAd_Closed?.Invoke();



    }
    private void OnAppOpenDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // AppOpen ad is hidden. Pre-load the next ad
        PrintStatus("Max AppOpen Displayed", false);
        isShowingAppOpen = true;
        FullScreenAd_Shown?.Invoke();

    }

    private void OnAppOpenDismissedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // AppOpen ad is hidden. Pre-load the next ad
        PrintStatus("Max AppOpen dismissed", true);
        isShowingAppOpen = false;
        isAppOpenAdReady = false;
        SetAppOpenAdUnitId(0);
        LoadAppOpen();
        FullScreenAd_Closed?.Invoke();
    }
    private void OnAppOpenRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
#if Firebase_Rizwan
        double revenue = adInfo.Revenue;
        var impressionParameters = new[] {
  new Firebase.Analytics.Parameter("ad_platform", "AppLovin"),
  new Firebase.Analytics.Parameter("ad_source", adInfo.NetworkName),
  new Firebase.Analytics.Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
  new Firebase.Analytics.Parameter("ad_format", adInfo.AdFormat),
  new Firebase.Analytics.Parameter("value", revenue),
  new Firebase.Analytics.Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
};
        Firebase.Analytics.FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
#endif
#if MAX
        // Replace with your attribution platform name, for example, "Adjust", and replace "userid" with your attribution platform UID
        MBridgeRevenueParamsEntity mBridgeRevenueParamsEntity = new MBridgeRevenueParamsEntity(MBridgeRevenueParamsEntity.ATTRIBUTION_PLATFORM_APPSFLYER, appsFlyerId);
        mBridgeRevenueParamsEntity.SetMaxAdInfo(adInfo);
        MBridgeRevenueManager.Track(mBridgeRevenueParamsEntity);
#endif
        //    // AppOpen ad revenue paid. Use this callback to track user revenue.
        //    Debug.Log("AppOpen revenue paid");

        //    // Ad revenue
        //    double revenue = adInfo.Revenue;

        //    // Miscellaneous data
        //    string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD"!
        //    string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        //    string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        //    string placement = adInfo.Placement; // The placement this ad's postbacks are tied to

        //    //TrackAdRevenue(adInfo);
    }

    #endregion

    public bool AnyFullScreenAdShowing_Max()
    {
        if (isShowingInterstitial || isShowingRewarded || isShowingRewardedInterstitial || isShowingAppOpen)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    #region Test_Functions

    public void Test_ShowRewardedAd()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max LowRAM", true);
            return;
        }
        ShowRewardedAd(REWARD, FAILED);
    }
    public void Test_ShowRewardedInterstitialAd()
    {
        if (lowRam)
        {
            PrintStatus("Cant Load Interstitial Max LowRAM", true);
            return;
        }
        ShowRewardedInterstitialAd(REWARD, FAILED);
    }

    void REWARD()
    {
        PrintStatus("MAX REWARD", false);
    }
    void FAILED()
    {
        PrintStatus("MAX FAILED  REWARD", false);
    }
    #endregion




    void PrintStatus(string _string, bool errorLog)
    {
        if (AdsController.Instance && AdsController.Instance.DebugMode)
        {
#if UNITY_EDITOR
            if (errorLog)
            {
                Debug.Log("<color=red><b>#MAX_ADs# </b></color> <b>" + _string + "</b> <color=red><b>#MAX_ADs# </b></color>");
            }
            else
            {
                Debug.Log("<color=green><b>#MAX_ADs# </b></color> <b>" + _string + "</b> <color=green><b>#MAX_ADs# </b></color>");
            }
#elif UNITY_ANDROID || UNITY_IOS

            Debug.Log(_string);

#endif
        }
    }

#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MaxMediation))]
class MaxMediationEditor : Editor
{
#if Max_Mediation_Rizwan
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
#else
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Please make sure applovin (MAX) SDK is imported into the project (recommended version Latest)." +
            "Also make sure that Symbol Max_Mediation_Rizwan is added in other Settings", MessageType.Error);
    }

    private void OnEnable()
    {
        CheckMaxFolder();

    }

    public static void CheckMaxFolder()
    {
        string MaxSKPath = Path.Combine(Application.dataPath, "MaxSdk");
        if (Directory.Exists(MaxSKPath))
        {
#if UNITY_2021_1_OR_NEWER
            NamedBuildTarget[] targets = { NamedBuildTarget.Android, NamedBuildTarget.iOS };
            UpdateSymbols("Max_Mediation_Rizwan", true, targets);
#else
            BuildTargetGroup[] targets = { BuildTargetGroup.Standalone,BuildTargetGroup.Android, BuildTargetGroup.iOS };
            UpdateSymbols("Max_Mediation_Rizwan", true, targets);
#endif
        }
        else
        {
            Debug.Log("Debug: MaxSKPath folder Does Not exist.");
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
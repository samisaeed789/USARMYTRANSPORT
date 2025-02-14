using System;
using System.Collections;
using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Build;
using System.Linq;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
#endif

public enum AdNetwork
{
#if Admob_Simple_Rizwan
    AdMob,
#endif
#if UnityAds_Rizwan
    UnityAds,
#endif
#if Max_Mediation_Rizwan
    Max
#endif
}
public class AdsController : MonoBehaviour
{
    public static AdsController Instance;
    public bool TestMode = true;
    public bool DebugMode = false;
    //public GameObject BOX_gameObject;

#if Admob_Simple_Rizwan
    public AdmobManager _admobManager;
#endif
#if Max_Mediation_Rizwan
    public MaxMediation _maxMediation;
#endif
#if UnityAds_Rizwan
    public UnityAdsManager _unityAdsManager;
#endif

    [HideInInspector] public bool isAdMobActive = false;
    [HideInInspector] public bool isUnityAdsActive = false;
    [HideInInspector] public bool isMaxActive = false;

    [HideInInspector] public AdNetwork[] adPriority;

    public _loadingAdStuff _LoadingAdStuff;
    [System.Serializable]
    public class _loadingAdStuff
    {
        public bool ShowWhenLoaded = false;
        public bool ShowBackground = true;
        public bool PauseTimeScale = true;
        [Space(20)]

        public int _CountdownTime = 5;
        public GameObject _Timer_Panel;
        public GameObject _BackGround;
        public TextMeshProUGUI _CountdownDisplay;
    }
    internal bool _adsRemoved = false;

    public GameObject BlackBackgroundImage;



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
#if Admob_Simple_Rizwan
        _admobManager._TestMode = TestMode ? true : false;
        _admobManager.StartFunctions();
#endif
#if Max_Mediation_Rizwan
        _maxMediation._TestMode = TestMode ? true : false;
#endif
#if UnityAds_Rizwan
        _unityAdsManager._TestMode = TestMode ? true : false;
#endif



        if (PlayerPrefs.HasKey("AdsRemoved"))
        {
            _adsRemoved = PlayerPrefs.GetInt("AdsRemoved", 0) == 1;
        }
        else
        {
            PlayerPrefs.SetInt("AdsRemoved", 0);
            _adsRemoved = false;
        }


    }
    private void OnEnable()
    {
#if Max_Mediation_Rizwan
        if (_maxMediation != null)
        {
            _maxMediation.FullScreenAd_Shown += HideActiveBanners_All;
            _maxMediation.FullScreenAd_Closed += ShowActiveBanners_All;
        }
#endif
#if Admob_Simple_Rizwan
        if (_admobManager != null)
        {
            _admobManager.FullScreenAd_Shown += HideActiveBanners_All;
            _admobManager.FullScreenAd_Closed += ShowActiveBanners_All;
        }
#endif
#if UnityAds_Rizwan
        if (_unityAdsManager != null)
        {
            _unityAdsManager.FullScreenAd_Shown += HideActiveBanners_All;
            _unityAdsManager.FullScreenAd_Closed += ShowActiveBanners_All;
        }
#endif
    }

    void HideActiveBanners_All()
    {

#if Max_Mediation_Rizwan
        if (_maxMediation)
        {
            _maxMediation.HideActiveBanners();
        }
#endif
#if Admob_Simple_Rizwan
        if (_admobManager)
        {
            _admobManager.HideActiveBanners();
        }
#endif
#if UnityAds_Rizwan
        if (_unityAdsManager)
        {
            _unityAdsManager.HideActiveBanners();
        }
#endif
        BlackBackgroundImage.SetActive(true);
    }

    void ShowActiveBanners_All()
    {
        if (BlackBackgroundImage) BlackBackgroundImage.SetActive(false);
#if Max_Mediation_Rizwan
        if (_maxMediation)
        {
            _maxMediation.ShowActiveBanners();
        }
#endif
#if Admob_Simple_Rizwan
        if (_admobManager)
        {
            _admobManager.ShowActiveBanners();
        }
#endif
#if UnityAds_Rizwan
        if (_unityAdsManager)
        {
            _unityAdsManager.ShowActiveBanners();
        }
#endif

    }

    private void Start()
    {

        // _LoadingAdStuff.Canvas.SetActive(false);
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        // Screen.sleepTimeout = SleepTimeout.NeverSleep;
        //if (TestMode == true)
        //{
        //    BOX_gameObject.SetActive(true);
        //    Debug.LogError("TEST MODE IS TRUE");
        //}
        //else if (TestMode == false)
        //{
        //    BOX_gameObject.SetActive(false);
        //    Debug.LogError("TEST MODE IS FALSE");
        //}
    }
    //void OnApplicationQuit()
    //{
    //    // Reset sleep timeout to SystemSetting when the app is closed
    //    Screen.sleepTimeout = SleepTimeout.SystemSetting;
    //}
    #region /*****\ ADMOB /*****\

    #region BANNER Ads


    void ShowBanner_Admob(int index)
    {
#if Admob_Simple_Rizwan

        _admobManager.Show_BannerAd(index);

#endif
    }
    void HideBanner_Admob(int index)
    {
#if Admob_Simple_Rizwan
        _admobManager.Hide_BannerAd(index);
#endif
    }
    void DestroyBanner_Admob(int index)
    {
#if Admob_Simple_Rizwan
        _admobManager.Destroy_BannerAd(index);
#endif
    }
    #endregion

    #region Interstitial

    void ShowInterstitial_Admob(int index)
    {
#if Admob_Simple_Rizwan
        if (_admobManager._InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        if (_admobManager._interstitialAd[index] != null && _admobManager._interstitialAd[index].CanShowAd())
        {
            _admobManager.ShowInterstitialAd(index);
        }
        else
        {
            _admobManager.RequestAndLoadInterstitialAd(index);
        }
#endif
    }

    void ShowInterstitial_Loading_Admob(int index)
    {
#if Admob_Simple_Rizwan
        if (_admobManager._InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        StartCoroutine(Countdown_Admob(index));

#endif
    }

    IEnumerator Countdown_Admob(int index)
    {

        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        float _timescale = Time.timeScale;
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
#if Admob_Simple_Rizwan
        if (_admobManager._interstitialAd[index] == null)
        {
            _admobManager.RequestAndLoadInterstitialAd(index);
        }
#else
        PrintStatus($"_admobManager is null", true);
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds

        while (currentTime > 0)
        {

            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0");
            yield return new WaitForSecondsRealtime(1f); // Wait for one second
#if Admob_Simple_Rizwan
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && _admobManager._interstitialAd[index] != null) ? 0 : currentTime - 1;
#endif
        }

        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        if (_timescale != 0)
        {
            Time.timeScale = 1;
        }
#if Admob_Simple_Rizwan
        if (_admobManager._interstitialAd[index] != null && _admobManager._interstitialAd[index].CanShowAd())
        {
            _admobManager.ShowInterstitialAd(index);
        }
#endif

    }
    #endregion

    #region Rewarded

    void ShowRewardedAd_Admob(int index, Action _reward, Action _failed)
    {
#if Admob_Simple_Rizwan
        if (_admobManager._RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }

        if (_admobManager._rewardedAd[index] != null && _admobManager._rewardedAd[index].CanShowAd())
        {
            _admobManager.ShowRewardedAd(index, _reward, _failed);

        }
        else if (_admobManager._RewardedInterstitalAdUnit.Length > 0 && _admobManager._rewardedInterstitialAd[index] != null && _admobManager._rewardedInterstitialAd[index].CanShowAd())
        {
            _admobManager.RequestAndLoadRewardedAd(index);
            _admobManager.ShowRewardedInterstitalAd(index, _reward, _failed);

        }
        //else if (_admobManager._interstitialAd[index] != null && _admobManager._interstitialAd[index].CanShowAd())
        //{
        //    _admobManager.ShowInterstitialAd(index);
        //    _admobManager.canShowAppOpen = false;
        //_admobManager?.RequestAndLoadRewarded_InterstitalAd(index);

        //    _reward?.Invoke();
        //}
        else
        {
            //_admobManager?.RequestAndLoadInterstitialAd(index);
            _failed?.Invoke();
            _admobManager.RequestAndLoadRewarded_InterstitalAd(index);
        }
#endif
    }

    void ShowRewarded_Loading_Admob(int index, Action _reward, Action _failed)
    {
#if Admob_Simple_Rizwan
        if (_admobManager._RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        StartCoroutine(Countdown_Admob(index, _reward, _failed));
#endif
    }
    IEnumerator Countdown_Admob(int index, Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        float _timescale = Time.timeScale;
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
#if Admob_Simple_Rizwan
        if (!String.IsNullOrEmpty(_admobManager.RewardedAdId[index]) && _admobManager._rewardedAd[index] == null)
        {
            _admobManager.RequestAndLoadRewardedAd(index);
        }
        else if (!String.IsNullOrEmpty(_admobManager.RewardedInterstitialAdId[index]) && _admobManager._rewardedInterstitialAd[index] == null)
        {
            _admobManager.RequestAndLoadRewarded_InterstitalAd(index);
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds

        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time
                                                                                //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
#if Admob_Simple_Rizwan
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && (_admobManager._rewardedAd[index] != null || _admobManager._rewardedInterstitialAd[index] != null)) ? 0 : currentTime - 1;
#endif
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        if (_timescale != 0)
        {
            Time.timeScale = 1;
        }
        ShowRewardedAd_Admob(index, _reward, _failed);
    }

    #endregion

    #region Rewarded_Interstitial
    void ShowRewardedInterstitialAd_Admob(int index, Action _reward, Action _failed)
    {
#if Admob_Simple_Rizwan
        if (_admobManager._rewardedInterstitialAd[index] != null && _admobManager._rewardedInterstitialAd[index].CanShowAd())
        {
            _admobManager.ShowRewardedInterstitalAd(index, _reward, _failed);
        }
        else
        {
            _failed?.Invoke();
            _admobManager.RequestAndLoadRewarded_InterstitalAd(index);
        }
#endif

    }

    void ShowRewardedInterstitial_Loading_Admob(int index, Action _reward, Action _failed)
    {
#if Admob_Simple_Rizwan
        if (_admobManager._RewardedInterstitalAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        StartCoroutine(Countdown_Admob_RewardedInterstitial(index, _reward, _failed));
#endif
    }

    IEnumerator Countdown_Admob_RewardedInterstitial(int index, Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        float _timescale = Time.timeScale;
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
#if Admob_Simple_Rizwan
        if (!String.IsNullOrEmpty(_admobManager.RewardedInterstitialAdId[index]) && _admobManager._rewardedInterstitialAd[index] == null)
        {
            _admobManager.RequestAndLoadRewarded_InterstitalAd(index);
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time
            //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
#if Admob_Simple_Rizwan
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && (_admobManager._rewardedInterstitialAd[index] != null)) ? 0 : currentTime - 1;
#endif
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        if (_timescale != 0)
        {
            Time.timeScale = 1;
        }
        ShowRewardedAd_Admob(index, _reward, _failed);
    }

    #endregion

    #endregion


    #region /*****\ MAX /*****\
    #region BANNER Ads


    void ShowBanner_Max(int index)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation._BannerAdUnit.Length == 0)
        {
            PrintStatus("Simple_BannerAdUnit is Null", true);
            return;
        }
        if (_maxMediation._BannerAdUnit[index] != null && _maxMediation._bannerLoaded[index] && !_maxMediation._bannerShowing[index])
        {
            _maxMediation.Show_BannerAd(index);
        }
        else
        {
            PrintStatus("No Admob Banner Ad Loaded so loading LoadAndShow_BannerAd", true);
            _maxMediation._bannerShowing[index] = true;
            _maxMediation.Load_BannerAd(index);
        }
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
    void HideBanner_Max(int index)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation._BannerAdUnit.Length == 0)
        {
            PrintStatus("Simple_BannerAdUnit is Null", true);
            return;
        }
        if (_maxMediation._BannerAdUnit[index] != null)
        {
            _maxMediation.Hide_BannerAd(index);
            PrintStatus("Hide_BannerAd is called ad Index = " + index, false);
        }
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
    void DestroyBanner_Max(int index)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation._BannerAdUnit.Length == 0)
        {
            PrintStatus("Simple_BannerAdUnit is Null", true);
            return;
        }
        if (_maxMediation._BannerAdUnit[index] != null)
        {
            _maxMediation.Destroy_BannerAd(index);
            PrintStatus("Destroy Banner called at Index " + index, true);
        }
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }


    #endregion

    #region Interstitial

    void ShowInterstitialAd_Max(int index)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation == null)
        {
            PrintStatus("_maxMediation Script's Reference is not Added in AdsController ", true);
            return;
        }
        _maxMediation.ShowInterstitialAd();
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }

    void ShowInterstitialAd_Loading_Max(int index)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation == null)
        {
            PrintStatus("_maxMediation Script's Reference is not Added in AdsController ", true);
            return;
        }
        StartCoroutine(Countdown_Max());
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
#if Max_Mediation_Rizwan

    IEnumerator Countdown_Max()
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
        if (!String.IsNullOrEmpty(_maxMediation.InterstitialAdUnitId) && _maxMediation.isInterstitialAdReady == false)
        {
            _maxMediation.LoadInterstitial();
        }
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0");
            //_LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one second
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && (!String.IsNullOrEmpty(_maxMediation.InterstitialAdUnitId) && _maxMediation.isInterstitialAdReady)) ? 0 : currentTime - 1;
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        Time.timeScale = 1;
        if (_maxMediation.isInterstitialAdReady)
        {
            _maxMediation.ShowInterstitialAd();
        }

    }

#endif
    #endregion

    #region Rewarded

    void ShowRewardedAd_Max(int index, Action _reward, Action _failed)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation == null)
        {
            PrintStatus("_maxMediation Script's Reference is not Added in AdsController ", true);
            return;
        }
        if (_maxMediation.isRewardedAdReady)
        {
            _maxMediation.ShowRewardedAd(_reward, _failed);
        }
        else if (_maxMediation.isRewardedInterstitialAdReady)
        {
            _maxMediation.LoadRewardedAd();
            _maxMediation.ShowRewardedInterstitialAd(_reward, _failed);
        }
        else
        {
            _maxMediation?.LoadRewardedInterstitialAd();
            _failed?.Invoke();
            _maxMediation.LoadRewardedAd();
        }
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }

    public void ShowRewardedAd_Loading_Max(int index, Action _reward, Action _failed)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation == null)
        {
            PrintStatus("_maxMediation Script's Reference is not Added in AdsController ", true);
            return;
        }
        StartCoroutine(CountdownRewarded_Max(_reward, _failed));
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
#if Max_Mediation_Rizwan
    IEnumerator CountdownRewarded_Max(Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
        if (!String.IsNullOrEmpty(_maxMediation.RewardedAdUnitId) && _maxMediation.isRewardedAdReady == false)
        {
            _maxMediation.LoadRewardedAd();
        }
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time
                                                                                //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon

            currentTime = (_LoadingAdStuff.ShowWhenLoaded && (!String.IsNullOrEmpty(_maxMediation.RewardedAdUnitId) && _maxMediation.isRewardedAdReady)) ? 0 : currentTime - 1;

        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        Time.timeScale = 1;
        ShowRewardedAd_Max(0, _reward, _failed);
    }

#endif
    public void ShowRewardedInterstitialAd_Max(int index, Action _reward, Action _failed)
    {
#if Max_Mediation_Rizwan

        if (_maxMediation == null)
        {
            PrintStatus("_maxMediation Script's Reference is not Added in AdsController ", true);
            return;
        }
        if (_maxMediation.isRewardedInterstitialAdReady)
        {
            _maxMediation.ShowRewardedInterstitialAd(_reward, _failed);
        }
        else
        {
            //_maxMediation?.RequestAndLoadInterstitialAd(index);
            _failed?.Invoke();
            _maxMediation.LoadRewardedInterstitialAd();
        }
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
    public void ShowRewardedInterstitialAd_Loading_Max(int index, Action _reward, Action _failed)
    {
#if Max_Mediation_Rizwan
        if (_maxMediation == null)
        {
            PrintStatus("_maxMediation Script's Reference is not Added in AdsController ", true);
            return;
        }
        StartCoroutine(CountdownRewardedInterstitial_Max(_reward, _failed));
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
#if Max_Mediation_Rizwan
    IEnumerator CountdownRewardedInterstitial_Max(Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
        if (!String.IsNullOrEmpty(_maxMediation.RewardedAdUnitId) && _maxMediation.isRewardedAdReady == false)
        {
            _maxMediation.LoadRewardedInterstitialAd();
        }
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time
            //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && (!String.IsNullOrEmpty(_maxMediation.RewardedAdUnitId) && _maxMediation.isRewardedAdReady)) ? 0 : currentTime - 1;
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        Time.timeScale = 1;
        ShowRewardedAd_Max(0, _reward, _failed);
    }

#endif
    #endregion

    #endregion

    #region /*****\ UNITYADS /*****\

    #region BANNER Ads

    void ShowBanner_UnityAds(int index)
    {
#if UnityAds_Rizwan
        _unityAdsManager.Show_BannerAd(index);
#endif
    }
    void HideBanner_UnityAds(int index)
    {
#if UnityAds_Rizwan
        _unityAdsManager.Hide_BannerAd(index);
#endif

    }
    void DestroyBanner_UnityAds(int index)
    {
#if UnityAds_Rizwan
        _unityAdsManager.Destroy_BannerAd(index);
#endif

    }

    #endregion

    #region Interstitial
    void ShowInterstitial_UnityAds(int index)
    {
#if UnityAds_Rizwan
        if (_unityAdsManager._InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        _unityAdsManager.ShowInterstitialAd(index);
#endif
    }
    void ShowInterstitial_Loading_UnityAds(int index)
    {
#if UnityAds_Rizwan
        if (_unityAdsManager._InterstitialAdUnit.Length == 0)
        {
            PrintStatus("_InterstitialAdUnit is Null", true);
            return;
        }
        StartCoroutine(Countdown_UnityAds(index));

#endif
    }
    IEnumerator Countdown_UnityAds(int index)
    {
        //_LoadingAdStuff.Canvas.SetActive(true);
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        float _timescale = Time.timeScale;
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
#if UnityAds_Rizwan
        if (_unityAdsManager.InterstitialLoaded[index] == false)
        {
            _unityAdsManager.RequestAndLoadInterstitialAd(index);
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0");
            //_LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one second
#if UnityAds_Rizwan
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && _unityAdsManager.InterstitialLoaded[index] == false) ? 0 : currentTime - 1;
#endif
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        if (_timescale != 0)
        {
            Time.timeScale = 1;
        }
#if UnityAds_Rizwan
        if (_unityAdsManager.InterstitialAdId[index] != null && _unityAdsManager.InterstitialLoaded[index] == true)
        {
            _unityAdsManager.ShowInterstitialAd(index);

        }
#endif

    }
    #endregion

    #region Rewarded

    void ShowRewardedAd_UnityAds(int index, Action _reward, Action _failed)
    {
#if UnityAds_Rizwan
        if (_unityAdsManager._RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        if (_unityAdsManager.RewardedLoaded[index] == true)
        {
            _unityAdsManager.ShowRewardedAd(index, _reward, _failed);
        }
        else
        {
            _failed?.Invoke();
            _unityAdsManager.RequestAndLoadRewardedAd(index);
        }
#endif
    }

    void ShowRewarded_Loading_UnityAds(int index, Action _reward, Action _failed)
    {
#if UnityAds_Rizwan
        if (_unityAdsManager._RewardedAdUnit.Length == 0)
        {
            PrintStatus("_RewardedAdUnit is Null", true);
            return;
        }
        StartCoroutine(Countdown_UnityAds(index, _reward, _failed));
#endif
    }

    IEnumerator Countdown_UnityAds(int index, Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        float _timescale = Time.timeScale;
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;
#if UnityAds_Rizwan
        if (_unityAdsManager.RewardedLoaded[index] == false)
        {
            _unityAdsManager.RequestAndLoadRewardedAd(index);
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time
                                                                                //_LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
#if UnityAds_Rizwan
            currentTime = (_LoadingAdStuff.ShowWhenLoaded && _unityAdsManager.RewardedLoaded[index] == false) ? 0 : currentTime - 1;
#endif
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        if (_timescale != 0)
        {
            Time.timeScale = 1;
        }
        ShowRewardedAd_UnityAds(index, _reward, _failed);
    }
    #endregion

    #endregion
    void PrintStatus(string _string, bool errorLog)
    {
        if (DebugMode)
        {
#if UNITY_EDITOR
            if (errorLog)
            {
                Debug.Log("<color=red><b>#ADs# </b></color> <b>" + _string + "</b> <color=red><b>#ADs# </b></color>");
            }
            else
            {
                Debug.Log("<color=green><b>#ADs# </b></color> <b>" + _string + "</b> <color=green><b>#ADs# </b></color>");
            }
#elif UNITY_ANDROID || UNITY_IOS

            Debug.Log(_string);

#endif
        }
    }


    #region PUBLIC FUNCTIONS

    #region ADMOB
    public void ShowBannerAd_Admob(int index)
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowBanner_Admob(index);

    }
    public void HideBannerAd_Admob(int index)
    {
        HideBanner_Admob(index);
    }
    public void HideAllBanners_Admob()
    {
#if Admob_Simple_Rizwan      
        for (int i = 0; i < _admobManager._BannerAdUnit.Length; i++)
        {
            HideBanner_Admob(i);
        }
#endif
    }


    public void DestroyBannerAd_Admob(int index)
    {
        DestroyBanner_Admob(index);
    }
    public void ShowInterstitialAd_Admob()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowInterstitial_Admob(0);
    }
    public void ShowInterstitialAd_Loading_Admob()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowInterstitial_Loading_Admob(0);
    }
    public void ShowRewardedAd_Admob(Action _reward)
    {
        ShowRewardedAd_Admob(0, _reward, null);
    }
    public void ShowRewardedAd_Admob(Action _reward, Action _failed)
    {
        ShowRewardedAd_Admob(0, _reward, _failed);
    }
    public void ShowRewardedAd_Loading_Admob(Action _reward)
    {
        ShowRewarded_Loading_Admob(0, _reward, null);
    }
    public void ShowRewardedAd_Loading_Admob(Action _reward, Action _failed)
    {
        ShowRewarded_Loading_Admob(0, _reward, _failed);
    }
    public void ShowRewardedInterstitialAd_Admob(Action _reward)
    {
        ShowRewardedInterstitialAd_Admob(0, _reward, null);
    }
    public void ShowRewardedInterstitialAd_Admob(Action _reward, Action _failed)
    {
        ShowRewardedInterstitialAd_Admob(0, _reward, _failed);
    }
    public void ShowRewardedInterstitial_Loading_Admob(Action _reward)
    {
        ShowRewardedInterstitial_Loading_Admob(0, _reward, null);
    }
    public void ShowRewardedInterstitial_Loading_Admob(Action _reward, Action _failed)
    {
        ShowRewardedInterstitial_Loading_Admob(0, _reward, _failed);
    }

    /*******JUST FOR TEST*********/
    public void Test_ShowReward_Admob()
    {

        ShowRewardedAd_Admob(0, Test_Reward, null);
    }
    public void Test_ShowRewardLoading_Admob()
    {

        ShowRewardedAd_Loading_Admob(Test_Reward);
    }
    public void Test_ShowRewardInter_Admob()
    {
        ShowRewardedInterstitialAd_Admob(0, Test_Reward, null);
    }
    public void Test_ShowRewardInterLoading_Admob()
    {
        ShowRewardedInterstitial_Loading_Admob(Test_Reward);
    }

    #endregion

    #region MAX

    public void ShowBannerAd_Max(int index)
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowBanner_Max(index);
    }
    public void HideBannerAd_Max(int index)
    {
        HideBanner_Max(index);
    }
    public void HideAllBanners_Max()
    {
#if Max_Mediation_Rizwan
        for (int i = 0; i < _maxMediation._BannerAdUnit.Length; i++)
        {
            HideBanner_Max(i);
        }
#else
        PrintStatus("Max SDK is removed from project, Please Add", true);
#endif
    }
    public void ShowInterstitialAd_Max()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowInterstitialAd_Max(0);
    }
    public void ShowInterstitialAd_Loading_Max()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowInterstitialAd_Loading_Max(0);
    }
    public void ShowRewardedAd_Max(Action _reward)
    {
        ShowRewardedAd_Max(0, _reward, null);
    }
    public void ShowRewardedAd_Max(Action _reward, Action _failed)
    {
        ShowRewardedAd_Max(0, _reward, _failed);
    }
    public void ShowRewardedAd_Loading_Max(Action _reward, Action _failed)
    {
        ShowRewardedAd_Loading_Max(0, _reward, _failed);
    }

    public void ShowRewardedInterstitialAd_Max(Action _reward)
    {
        ShowRewardedInterstitialAd_Max(0, _reward, null);
    }
    public void ShowRewardedInterstitialAd_Max(Action _reward, Action _failed)
    {
        ShowRewardedInterstitialAd_Max(0, _reward, _failed);
    }



    /*******JUST FOR TEST*********/
    public void Test_ShowReward_Max()
    {
        ShowRewardedAd_Max(0, Test_Reward_Max, null);
    }

    public void Test_ShowRewardedAd_Loading_Max()
    {
        ShowRewardedAd_Loading_Max(Test_Reward_Max, null);
    }
    public void Test_ShowRewardedInsterstitialAd_Max()
    {
        ShowRewardedInterstitialAd_Max(Test_Reward_Max, null);
    }
    void Test_Reward_Max()
    {
        PrintStatus("Reward Given Max", false);
    }

    public void scenechange()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public void ShowDebugger_Max()
    {
#if Max_Mediation_Rizwan
        //MaxSdk.ShowMediationDebugger();
#endif
    }
    #endregion

    #region UNITY_ADS

    public void ShowBannerAd_UnityAds(int index)
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowBanner_UnityAds(index);
    }
    public void HideBannerAd_UnityAds(int index)
    {
        HideBanner_UnityAds(index);
    }
    public void HideAllBanners_UnityAds()
    {
#if UnityAds_Rizwan
        for (int i = 0; i < _unityAdsManager._BannerAdUnit.Length; i++)
        {
            HideBannerAd_UnityAds(i);
        }
#endif
    }
    public void DestroyBannerAd_UnityAds(int index)
    {
        DestroyBanner_UnityAds(index);
    }
    public void ShowInterstitialAd_UnityAds()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowInterstitial_UnityAds(0);
    }
    public void ShowInterstitialAd_Loading_UnityAds()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        ShowInterstitial_Loading_UnityAds(0);
    }
    public void ShowRewardedAd_UnityAds(Action _reward)
    {
        ShowRewardedAd_UnityAds(0, _reward, null);
    }
    public void ShowRewardedAd_UnityAds(Action _reward, Action _failed)
    {
        ShowRewardedAd_UnityAds(0, _reward, _failed);
    }
    public void ShowRewardedAd_Loading_UnityAds(Action _reward, Action _failed)
    {

        ShowRewarded_Loading_UnityAds(0, _reward, _failed);
    }

    /*******JUST FOR TEST*********/
    public void Test_ShowReward_UnityAds()
    {
        ShowRewardedAd_UnityAds(0, Test_Reward_UnityAds, null);
    }

    public void Test_ShowRewardedAd_Loading_UnityAds()
    {

        ShowRewardedAd_Loading_UnityAds(Test_Reward_UnityAds, null);
    }

    void Test_Reward_UnityAds()
    {
        PrintStatus("Reward Given UnityAds", false);
    }


    #endregion

    public void RemoveAds()
    {
        PlayerPrefs.SetInt("AdsRemoved", 1);
        _adsRemoved = true;
        HideAllBanners_ALL();
    }

    #region Universal 

    void Test_Reward()
    {
        PrintStatus("Reward Given", false);
    }

    public void HideAllBanners_ALL()
    {
#if Max_Mediation_Rizwan
        if (_maxMediation)
        {
            HideAllBanners_Max();
        }
#endif
#if Admob_Simple_Rizwan
        if (_admobManager)
        {
            HideAllBanners_Admob();
        }
#endif
#if UnityAds_Rizwan
        if (_unityAdsManager)
        {
            HideAllBanners_UnityAds();
        }
#endif
    }

    public void ShowInterstitialAd_All()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        bool AdShown = false;
        foreach (AdNetwork adNetwork in adPriority)
        {
            if (AdShown == false)
            {
                switch (adNetwork)
                {
#if Admob_Simple_Rizwan
                    case AdNetwork.AdMob:
                        if (_admobManager._interstitialAd[0] != null
                            && _admobManager._interstitialAd[0].CanShowAd())
                        {
                            ShowInterstitialAd_Admob();
                            AdShown = true;
                        }
                        break;
#endif
#if Max_Mediation_Rizwan
                    case AdNetwork.Max:
                        if (_maxMediation.isInterstitialAdReady)
                        {
                            ShowInterstitialAd_Max();
                            AdShown = true;
                        }
                        break;
#endif
#if UnityAds_Rizwan
                    case AdNetwork.UnityAds:

                        if (_unityAdsManager.InterstitialLoaded[0])
                        {
                            Debug.Log("UnityAds");
                            ShowInterstitialAd_UnityAds();
                            AdShown = true;
                        }
                        break;
#endif
                    default:
                        Debug.LogError("No Ad Network is available");
                        break;
                }
            }
        }
    }

    public void ShowInterstitialAd_Loading_All()
    {
        if (_adsRemoved) { PrintStatus("Ads Removed Through RemoveAds Function", false); return; }
        StartCoroutine(CountdownInterstitial_All());
    }

    IEnumerator CountdownInterstitial_All()
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;

#if Admob_Simple_Rizwan
        if (_admobManager._interstitialAd[0] == null)
        {
            _admobManager.RequestAndLoadInterstitialAd(0);
        }
#endif
#if Max_Mediation_Rizwan
        if (_maxMediation.isInterstitialAdReady == false)
        {
            _maxMediation.LoadInterstitial();
        }
#endif
#if UnityAds_Rizwan
        if (_unityAdsManager.InterstitialLoaded[0] == false)
        {
            _unityAdsManager.RequestAndLoadInterstitialAd(0);
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        bool admobReady = false, maxReady = false, unityadsReady = false;
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time
                                                                                //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
#if Admob_Simple_Rizwan
            admobReady = _LoadingAdStuff.ShowWhenLoaded && _admobManager._interstitialAd[0] != null;
#endif
#if Max_Mediation_Rizwan
            maxReady = _LoadingAdStuff.ShowWhenLoaded && _maxMediation.isInterstitialAdReady;
#endif
#if UnityAds_Rizwan
            unityadsReady = _LoadingAdStuff.ShowWhenLoaded && _unityAdsManager.InterstitialLoaded[0];
#endif
            currentTime = admobReady || maxReady || unityadsReady ? 0 : currentTime - 1;

        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        Time.timeScale = 1;

        ShowInterstitialAd_All();
    }


    public void ShowRewardedAd_All(Action reward, Action failed)
    {
        bool AdShown = false;
        foreach (AdNetwork adNetwork in adPriority)
        {
            if (AdShown == false)
            {
                switch (adNetwork)
                {
#if Admob_Simple_Rizwan
                    case AdNetwork.AdMob:
                        if (_admobManager._rewardedAd[0] != null
                            && _admobManager._rewardedAd[0].CanShowAd())
                        {
                            ShowRewardedAd_Admob(reward, failed);
                            AdShown = true;
                        }
                        else if (_admobManager._rewardedInterstitialAd[0] != null
                            && _admobManager._rewardedInterstitialAd[0].CanShowAd())
                        {
                            ShowRewardedInterstitialAd_Admob(reward, failed);
                            AdShown = true;
                        }
                        break;
#endif
#if Max_Mediation_Rizwan
                    case AdNetwork.Max:
                        if (_maxMediation.isRewardedAdReady)
                        {
                            ShowRewardedAd_Max(reward, failed);
                            AdShown = true;
                        }
                        else if (_maxMediation.isRewardedInterstitialAdReady)
                        {
                            ShowRewardedInterstitialAd_Max(reward, failed);
                            AdShown = true;
                        }


                        break;
#endif
#if UnityAds_Rizwan
                    case AdNetwork.UnityAds:
                        if (_unityAdsManager.InterstitialLoaded[0])
                        {
                            ShowRewardedAd_UnityAds(reward, failed);
                            AdShown = true;
                        }
                        break;
#endif
                    default:
                        Debug.LogError("No Ad Network is available");
                        break;

                }
            }
        }
    }
    public void ShowRewardedAd_Loading_All(Action _reward, Action _failed)
    {
        StartCoroutine(CountdownRewarded_All(_reward, _failed));
    }

    IEnumerator CountdownRewarded_All(Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;

#if Admob_Simple_Rizwan
        if (_admobManager._rewardedAd[0] == null)
        {
            _admobManager.RequestAndLoadRewardedAd(0);
        }
        if (_admobManager._rewardedInterstitialAd[0] == null)
        {
            _admobManager.RequestAndLoadRewarded_InterstitalAd(0);
        }
#endif

#if Max_Mediation_Rizwan
        if (_maxMediation.isRewardedAdReady == false)
        {
            _maxMediation.LoadRewardedAd();
        }
        if (_maxMediation.isRewardedInterstitialAdReady == false)
        {
            _maxMediation.LoadRewardedInterstitialAd();
        }
#endif

#if UnityAds_Rizwan
        if (_unityAdsManager.RewardedLoaded[0] == false)
        {
            _unityAdsManager.RequestAndLoadRewardedAd(0);
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        bool admobRewardedReady = false, admobRewardedInterstitialReady = false,
            maxRewardedReady = false, maxRewardedInterstitialReady = false, unityAdsRewardedReady = false;
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time                                                                                //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
#if Admob_Simple_Rizwan
            admobRewardedReady = _LoadingAdStuff.ShowWhenLoaded && _admobManager._rewardedAd[0] != null;
            admobRewardedInterstitialReady = _LoadingAdStuff.ShowWhenLoaded && _admobManager._rewardedInterstitialAd[0] != null;
#endif
#if Max_Mediation_Rizwan
            maxRewardedReady = _maxMediation.isRewardedAdReady;
            maxRewardedInterstitialReady = _maxMediation.isRewardedInterstitialAdReady;
#endif
#if UnityAds_Rizwan
            unityAdsRewardedReady = _unityAdsManager.RewardedLoaded[0];
#endif
            currentTime = admobRewardedReady || admobRewardedInterstitialReady || maxRewardedReady
                         || maxRewardedInterstitialReady || unityAdsRewardedReady ? 0 : currentTime - 1;
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        Time.timeScale = 1;

        ShowRewardedAd_All(_reward, _failed);
    }


    public void ShowRewardedInterstitialAd_All(Action reward, Action failed)
    {
        bool AdShown = false;
        foreach (AdNetwork adNetwork in adPriority)
        {
            if (AdShown == false)
            {
                switch (adNetwork)
                {
#if Admob_Simple_Rizwan
                    case AdNetwork.AdMob:
                        if (_admobManager._rewardedInterstitialAd[0] != null
                            && _admobManager._rewardedInterstitialAd[0].CanShowAd())
                        {
                            ShowRewardedInterstitialAd_Admob(reward, failed);
                            AdShown = true;
                        }
                        break;
#endif
#if Max_Mediation_Rizwan
                    case AdNetwork.Max:
                        if (_maxMediation.isRewardedInterstitialAdReady)
                        {
                            ShowRewardedInterstitialAd_Max(reward, failed);
                            AdShown = true;
                        }
                        break;
#endif

                    default:
                        Debug.LogError("No Ad Network is available");
                        break;

                }
            }
        }
    }
    public void ShowRewardedInterstitialAd_Loading_All(Action _reward, Action _failed)
    {
        StartCoroutine(CountdownRewardedInterstitial_All(_reward, _failed));
    }

    IEnumerator CountdownRewardedInterstitial_All(Action _reward, Action _failed)
    {
        _LoadingAdStuff._Timer_Panel.SetActive(true);
        _LoadingAdStuff._BackGround.SetActive(_LoadingAdStuff.ShowBackground);
        Time.timeScale = _LoadingAdStuff.PauseTimeScale ? 0 : 1;

#if Admob_Simple_Rizwan
        if (_admobManager._rewardedInterstitialAd[0] == null)
        {
            _admobManager.RequestAndLoadRewarded_InterstitalAd(0);
        }
#endif
#if Max_Mediation_Rizwan
        if (_maxMediation.isRewardedInterstitialAdReady == false)
        {
            _maxMediation.LoadRewardedInterstitialAd();
        }
#endif
        float currentTime = _LoadingAdStuff._CountdownTime; // The current countdown time in seconds
        bool admobRewardedInterstitialReady = false, maxRewardedInterstitialReady = false;
        while (currentTime > 0)
        {
            _LoadingAdStuff._CountdownDisplay.text = currentTime.ToString("0"); // Update the UI Text object with the current countdown time                                                                                //  _LoadingAdStuff._CountdownDisplay.color = UnityEngine.Random.ColorHSV(0f, 1f, 0f, 1f, 0.8f, 1f); // Update the UI Text object with the current countdown time
            yield return new WaitForSecondsRealtime(1f); // Wait for one secon
#if Admob_Simple_Rizwan
            admobRewardedInterstitialReady = _LoadingAdStuff.ShowWhenLoaded && _admobManager._rewardedInterstitialAd[0] != null;
#endif
#if Max_Mediation_Rizwan
            maxRewardedInterstitialReady = _maxMediation.isRewardedInterstitialAdReady;
#endif
            currentTime = admobRewardedInterstitialReady || maxRewardedInterstitialReady ? 0 : currentTime - 1;
        }
        _LoadingAdStuff._CountdownDisplay.text = "0"; // Update the UI Text object with "0" to indicate the end of the countdown
        _LoadingAdStuff._Timer_Panel.SetActive(false);
        Time.timeScale = 1;

        ShowRewardedInterstitialAd_All(_reward, _failed);
    }


    ///////### TEST ###/////
    public void Test_ShowReward_All()
    {
        ShowRewardedAd_All(Test_Reward_All, null);
    }

    public void Test_ShowRewardedAd_Loading_All()
    {
        ShowRewardedAd_Loading_All(Test_Reward_All, null);
    }
    public void Test_ShowRewardedInsterstitialAd_All()
    {
        ShowRewardedInterstitialAd_All(Test_Reward_All, null);
    }
    void Test_Reward_All()
    {
        PrintStatus("Reward Given All", false);
    }

    #endregion
    #endregion


}



/// <summary>
/// Editor
/// </summary>



#if UNITY_EDITOR
[CustomEditor(typeof(AdsController))]
class AdsControllerEditor : Editor
{


    private static AdmobManager admobManagerComponent;
    private static MaxMediation maxMediationComponent;
    private static UnityAdsManager unityAdsManagerComponent;
    //private static bool admobPlugin = false, maxPlugin = false, unityadsPlugin = false;


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

    private void Awake()
    {
        // Pre-create textures
        NormalTexture = CreateTexture(Color.gray);
        HoverTexture = CreateTexture(Color.yellow);
        ActiveTexture = CreateTexture(new Color(0f, 0.5f, 0f)); // Dark green
        ActiveTexture2 = CreateTexture(Color.red);
    }

    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        var adcontroller = (AdsController)target;
        if (adcontroller == null)
        {
            Debug.LogError("AdsController is null");
            return;
        }
        if (PrefabUtility.IsPartOfPrefabInstance(adcontroller.gameObject))
        {
            // Unpack the prefab instance completely
            PrefabUtility.UnpackPrefabInstance(adcontroller.gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
            Debug.Log("Prefab unpacked completely.");
        }
        GUILayout.BeginVertical();

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
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


        ///////////////////////////////////////////////////////

        #region  ADMOB
        GUILayout.BeginHorizontal();
        admobManagerComponent = adcontroller.GetComponentInChildren<AdmobManager>(true);
        var consent = adcontroller.GetComponentInChildren<GoogleMobileAdsConsentController>(true);
        if (admobManagerComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("ADMOB", buttonStyle))
            {
                // Check if the AdmobManager script exists in the project
                var admobManagerType = typeof(AdmobManager);
                var consentType = typeof(GoogleMobileAdsConsentController);
                if (admobManagerType == null)
                {
                    Debug.LogError("AdmobManager script not found in the project");
                    return;
                }
                // Create a new GameObject named "AdmobManager"
                GameObject admobManagerGameObject = new GameObject("ADMOB MANAGER");

                // Add the AdmobManager component to the new GameObject
                admobManagerComponent = (AdmobManager)admobManagerGameObject.AddComponent(admobManagerType);
                consent = (GoogleMobileAdsConsentController)admobManagerGameObject.AddComponent(consentType);
                if (admobManagerComponent == null)
                {
                    Debug.LogError("Failed to add AdmobManager component to the GameObject");
                    return;
                }
#if UNITY_2021_1_OR_NEWER
                var pluginmanager = adcontroller.GetComponentInParent<PluginManager>(true);
#else
                var pluginmanager = adcontroller.GetComponentInParent<PluginManager>();
#endif
                if (pluginmanager != null)
                {
#if UNITY_2021_1_OR_NEWER
                    var remoteconfig = pluginmanager.GetComponentInChildren<FirebaseRemoteConfigHandler>(true);
#else
                    var remoteconfig = pluginmanager.GetComponentInChildren<FirebaseRemoteConfigHandler>();
#endif
#if Admob_Simple_Rizwan
#if Remote_Config_Rizwan

                    admobManagerComponent._firebaseRemoteConfigHandler = remoteconfig;
#endif
#endif
                }
                // Insert the new GameObject at the second position
                admobManagerGameObject.transform.SetParent(adcontroller.transform);
                admobManagerGameObject.transform.SetSiblingIndex(0);
                EditorGUIUtility.PingObject(admobManagerGameObject.transform);

                string googleMobileAdsPath = Path.Combine(Application.dataPath, "GoogleMobileAds");

                // Check if the "GoogleMobileAds" folder exists
                if (Directory.Exists(googleMobileAdsPath))
                {
                    Debug.Log("GoogleMobileAds is present in the Assets folder.");
#if UNITY_2021_1_OR_NEWER
                    NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
                    UpdateSymbols("Admob_Simple_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Admob_Simple_Rizwan", true, targets);
#endif
                }
                else
                {
                    Debug.Log("GoogleMobileAds is not present in the Assets folder.");
                    //RemoveScriptingDefineSymbol("Admob_Mediation_Rizwan");
                }
                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(adcontroller);
                Debug.Log("AdmobManager component added and linked successfully");
            }
        }
        else
        {
            // Link the AdmobManager reference in the AdsController
#if Admob_Simple_Rizwan
            if (adcontroller._admobManager == null)
            {
                adcontroller._admobManager = admobManagerComponent;
                adcontroller._admobManager._consentController = consent;
            }
#endif
            buttonStyle.normal.background = ActiveTexture;
            if (GUILayout.Button("ADMOB", buttonStyle))
            {

#if Admob_Simple_Rizwan
                adcontroller._admobManager = null;
#endif
                RemoveScriptingDefineSymbol("Admob_Simple_Rizwan");

                DestroyImmediate(admobManagerComponent.gameObject);


                EditorUtility.SetDirty(adcontroller);
                Debug.Log("AdmobManager component removed successfully");
            }

        }
        GUILayout.EndHorizontal();
        #endregion
        GUILayout.Space(10);
        ///////////////////////////////////////////////////////

        #region    MAX_Mediation
        GUILayout.BeginHorizontal();
#if UNITY_2021_1_OR_NEWER
        maxMediationComponent = adcontroller.GetComponentInChildren<MaxMediation>(true);
#else
        maxMediationComponent = adcontroller.GetComponentInChildren<MaxMediation>();
#endif
        if (maxMediationComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("MAX", buttonStyle))
            {
                // Check if the MaxMediation script exists in the project
                var maxMediationType = typeof(MaxMediation);
                if (maxMediationType == null)
                {
                    Debug.LogError("MaxMediation script not found in the project");
                    return;
                }
                // Create a new GameObject named "MAX MEDIAITION"
                GameObject maxMediationGameObject = new GameObject("MAX MEDIAITION");
                // Add the MaxMediation component to the new GameObject
                maxMediationComponent = (MaxMediation)maxMediationGameObject.AddComponent(maxMediationType);
                if (maxMediationComponent == null)
                {
                    Debug.LogError("Failed to add MaxMediation component to the GameObject");
                    return;
                }

#if UNITY_2021_1_OR_NEWER
                var pluginmanager = adcontroller.GetComponentInParent<PluginManager>(true);
#else
                var pluginmanager = adcontroller.GetComponentInParent<PluginManager>();
#endif
                if (pluginmanager != null)
                {

#if UNITY_2021_1_OR_NEWER
                    var remoteconfig = pluginmanager.GetComponentInChildren<FirebaseRemoteConfigHandler>(true);
#else
                    var remoteconfig = pluginmanager.GetComponentInChildren<FirebaseRemoteConfigHandler>();
#endif
#if Max_Mediation_Rizwan
                    maxMediationComponent._fireBaseRemoteConfigHandler = remoteconfig;
#endif
                }
                // Insert the new GameObject at the second position
                maxMediationGameObject.transform.SetParent(adcontroller.transform);
                maxMediationGameObject.transform.SetSiblingIndex(1);
                EditorGUIUtility.PingObject(maxMediationGameObject.transform);
                string googleMobileAdsPath = Path.Combine(Application.dataPath, "MaxSdk");
                if (Directory.Exists(googleMobileAdsPath))
                {
                    Debug.Log("MaxSdk is present in the Assets folder.");
#if UNITY_2021_1_OR_NEWER
                    NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
                    UpdateSymbols("Max_Mediation_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("Max_Mediation_Rizwan", true, targets);
#endif
                }
                else
                {
                    Debug.Log("MaxSDK is not present in the Assets folder.Please Import");
                }
                EditorUtility.SetDirty(adcontroller);

                Debug.Log("MaxMediation component added and linked successfully");
            }
        }
        else
        {
#if Max_Mediation_Rizwan
            // Link the MaxMediation reference in the AdsController
            if (adcontroller._maxMediation == null)
            {
                adcontroller._maxMediation = maxMediationComponent;
            }
#endif
            buttonStyle.normal.background = ActiveTexture;
            if (GUILayout.Button("MAX", buttonStyle))
            {
                string[] pathsToDelete = {
                    "Assets/MaxSdk"
                         };
                DeletePluginFilesFolders(pathsToDelete);

                RemoveScriptingDefineSymbol("Max_Mediation_Rizwan");
                RemoveScriptingDefineSymbol("gameanalytics_max_enabled");
                DestroyImmediate(maxMediationComponent.gameObject);
#if Max_Mediation_Rizwan
                adcontroller._maxMediation = null;
#endif
                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(adcontroller);

                Debug.Log("MaxMediation component removed successfully");
            }
            //if (!maxPlugin)
            //{
            //    EditorGUILayout.HelpBox("Please import the Max SDK Manually.", MessageType.Error);
            //}
        }
        GUILayout.EndHorizontal();
        #endregion
        GUILayout.Space(10);
        ///////////////////////////////////////////////////////
        GUILayout.BeginHorizontal();
        #region UNITYADS

#if UNITY_2021_1_OR_NEWER
        unityAdsManagerComponent = adcontroller.GetComponentInChildren<UnityAdsManager>(true);
#else
        unityAdsManagerComponent = adcontroller.GetComponentInChildren<UnityAdsManager>();
#endif

        if (unityAdsManagerComponent == null)
        {
            buttonStyle.normal.background = NormalTexture;
            if (GUILayout.Button("UNITYADS", buttonStyle))
            {
                AddRequest request = Client.Add("com.unity.ads");

                while (!request.IsCompleted)
                {
                    //wait krn jab tak request pori na ho
                }

                if (request.Status == StatusCode.Success)
                {
                    Debug.Log("Package " + "InApp purchases" + " added successfully.");
                }
                else
                {
                    Debug.LogError("Failed to add package " + "InApp purchases" + ": " + request.Error.message);
                }

                // Check if the unityAdsManager script exists in the project
                var unityAdsManagerType = typeof(UnityAdsManager);

                if (unityAdsManagerType == null)
                {
                    Debug.LogError("unityAdsManager script not found in the project");
                    return;
                }
                // Create a new GameObject named "unityAdsManager"
                GameObject unityAdsManagerGameObject = new GameObject("UNITY ADS MANAGER");

                // Add the unityAdsManager component to the new GameObject
                unityAdsManagerComponent = (UnityAdsManager)unityAdsManagerGameObject.AddComponent(unityAdsManagerType);

                if (unityAdsManagerComponent == null)
                {
                    Debug.LogError("Failed to add unityAdsManager component to the GameObject");
                    return;
                }
                unityAdsManagerGameObject.transform.SetParent(adcontroller.transform);
                unityAdsManagerGameObject.transform.SetSiblingIndex(0);
                EditorGUIUtility.PingObject(unityAdsManagerGameObject.transform);
                //UpdateSymbols("UnityAds_Rizwan", true, new NamedBuildTarget[] { NamedBuildTarget.Android, NamedBuildTarget.iOS });
#if UNITY_2021_1_OR_NEWER
                NamedBuildTarget[] targets = { NamedBuildTarget.Standalone, NamedBuildTarget.Android, NamedBuildTarget.iOS };
                UpdateSymbols("UnityAds_Rizwan", true, targets);
#else
                BuildTargetGroup[] targets = { BuildTargetGroup.Standalone, BuildTargetGroup.Android, BuildTargetGroup.iOS };
                UpdateSymbols("UnityAds_Rizwan", true, targets);
#endif
                // Mark the AdsController as dirty to ensure changes are saved
                EditorUtility.SetDirty(adcontroller);
                Debug.Log("unityAdsManager component added and linked successfully");
            }
        }

        else
        {
            // Link the unityAdsManager reference in the AdsController
#if UnityAds_Rizwan
            if (adcontroller._unityAdsManager == null)
            {
                adcontroller._unityAdsManager = unityAdsManagerComponent;
            }
#endif
            buttonStyle.normal.background = ActiveTexture;
            if (GUILayout.Button("UNITYADS", buttonStyle))
            {
                RemoveScriptingDefineSymbol("UnityAds_Rizwan");
                DestroyImmediate(unityAdsManagerComponent.gameObject);

                RemoveRequest request = Client.Remove("com.unity.ads");

                while (!request.IsCompleted)
                {
                    //wait krn jab tak request pori na ho
                }
                if (request.Status == StatusCode.Success)
                {
                    Debug.Log("Package " + "Advertisement Legacy" + " Removed successfully.");
                }
                else
                {
                    Debug.LogError("Failed to add package " + "InApp purchases" + ": " + request.Error.message);
                }
                if (unityAdsManagerComponent == null)
                {
                    Debug.LogError("unityAdsManager script not found in the project");
                    return;
                }
#if UNITY_2021_1_OR_NEWER
                var pluginmanager = adcontroller.GetComponentInParent<PluginManager>(true);
#else
                var pluginmanager = adcontroller.GetComponentInParent<PluginManager>();
#endif
                // Mark the AdsController as dirty to ensure changes are saved
                //EditorUtility.SetDirty(pluginmanager);
                //AssetDatabase.Refresh();
                //Debug.Log("unityAdsManager component removed successfully");
            }
            //if (!unityadsPlugin)
            //{
            //    EditorGUILayout.HelpBox("Please import the UnityAds plugin from the Package Manager.", MessageType.Error);
            //}
        }
        #endregion


        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        /////////////////////////////////////////
        /////////////////////////////////////
        if (admobManagerComponent != null) { adcontroller.isAdMobActive = true; } else { adcontroller.isAdMobActive = false; }
        if (maxMediationComponent != null) { adcontroller.isMaxActive = true; } else { adcontroller.isMaxActive = false; }
        if (unityAdsManagerComponent != null) { adcontroller.isUnityAdsActive = true; } else { adcontroller.isUnityAdsActive = false; }

        //adcontroller.isAdMobActive = EditorGUILayout.Toggle("Is AdMob Active", adcontroller.isAdMobActive);
        //adcontroller.isUnityAdsActive = EditorGUILayout.Toggle("Is UnityAds Active", adcontroller.isUnityAdsActive);
        //adcontroller.isMaxActive = EditorGUILayout.Toggle("Is Max Active", adcontroller.isMaxActive);

        EditorGUILayout.Space();

        int activeCount = 0;
        if (adcontroller.isAdMobActive) activeCount++;
        if (adcontroller.isUnityAdsActive) activeCount++;
        if (adcontroller.isMaxActive) activeCount++;

        AdNetwork[] availableAdNetworks = new AdNetwork[activeCount];
        int index = 0;
#if Admob_Simple_Rizwan
        if (adcontroller.isAdMobActive) availableAdNetworks[index++] = AdNetwork.AdMob;
#endif
#if UnityAds_Rizwan
        if (adcontroller.isUnityAdsActive) availableAdNetworks[index++] = AdNetwork.UnityAds;
#endif
#if Max_Mediation_Rizwan
        if (adcontroller.isMaxActive) availableAdNetworks[index++] = AdNetwork.Max;
#endif
        // Resize the adPriority array to match the number of active ad networks
        System.Array.Resize(ref adcontroller.adPriority, activeCount);
        if (index > 1)
        {
            GUILayout.Label("Choose your choice based on Priority ( 1 means Highest)");
            for (int i = 0; i < activeCount; i++)
            {
                adcontroller.adPriority[i] = (AdNetwork)EditorGUILayout.EnumPopup($"Priority {i + 1}", adcontroller.adPriority[i]);
            }
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(adcontroller);
        }

        /////////////////////////////////////////
        ///////////////////////////////////////

        DrawDefaultInspector();
        Repaint();
    }


    //private static bool AssetExistsAtPath(string path)
    //{
    //    return System.IO.Directory.Exists(path) || System.IO.File.Exists(path);
    //}
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
                Debug.Log($"{path} file deleted successfully.");
            }
            else if (Directory.Exists(path))
            {
                FileUtil.DeleteFileOrDirectory(path);
                FileUtil.DeleteFileOrDirectory(path + ".meta");
                Debug.Log($"{path} folder deleted successfully.");
            }

        }

        AssetDatabase.Refresh();
    }
    //private Texture2D MakeTex(int width, int height, Color col)
    //{
    //    Color[] pix = new Color[width * height];
    //    for (int i = 0; i < pix.Length; i++)
    //    {
    //        pix[i] = col;
    //    }
    //    Texture2D result = new Texture2D(width, height);
    //    result.SetPixels(pix);
    //    result.Apply();
    //    return result;
    //}



}



#endif





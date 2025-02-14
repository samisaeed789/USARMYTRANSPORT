using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testingads : MonoBehaviour
{
    public void ShowBanner_Admob(int index)
    {
        AdsController.Instance.ShowBannerAd_Admob(index);
    }
    public void HideBanner_Admob(int index)
    {
        AdsController.Instance.HideBannerAd_Admob(index);
    }
    public void DestroyBanner_Admob(int index)
    {
        AdsController.Instance.DestroyBannerAd_Admob(index);
    }
    public void ShowInterStitial_Admob()
    {
        AdsController.Instance.ShowInterstitialAd_Admob();
    }
    public void ShowInterStitialLoading_Admob()
    {
        AdsController.Instance.ShowInterstitialAd_Loading_Admob();
    }
    public void ShowRewarded_Admob()
    {
        AdsController.Instance.ShowRewardedAd_Admob(REWARD);
    }
    public void ShowRewardedLoading_Admob()
    {
        AdsController.Instance.ShowRewardedAd_Loading_Admob(REWARD);
    }
    public void ShowRewardedInterstitial_Admob()
    {
        AdsController.Instance.ShowRewardedInterstitialAd_Admob(REWARD);
    }
    public void ShowRewardedInterstitialLoading_Admob()
    {
        AdsController.Instance.ShowRewardedInterstitial_Loading_Admob(REWARD);
    }

    public void REWARD()
    {
        Debug.Log("REWARDED");
    }


    //MAX
    public void ShowBanner_MAX(int index)
    {
        AdsController.Instance.ShowBannerAd_Max(index);
    }
    public void HideBanner_MAX(int index)
    {
        AdsController.Instance.HideBannerAd_Max(index);
    }
    //public void DestroyBanner_MAX(int index)
    //{
    //    AdsController.Instance.destr(index);
    //}
    public void ShowInterStitial_MAX()
    {
        AdsController.Instance.ShowInterstitialAd_Max();
    }
    public void ShowRewarded_MAX()
    {
        AdsController.Instance.ShowRewardedAd_Max(REWARD);
    }
}

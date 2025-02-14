using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.VisualElement;
public class RewardReadyCheck : MonoBehaviour
{
    [SerializeField]
    private Button RewardedBtn;
   [Range(0,20)]
    public float RecheckTime=2f;
    Transform[] heirarchy;
    private void Awake()
    {
        if (RewardedBtn == null)
        {
            RewardedBtn = GetComponent<Button>();
        }

    }
    private void OnEnable()
    {
        RewardedBtn.interactable = false;
        if (RewardedBtn.transform.childCount > 0)
        {
            heirarchy = RewardedBtn.transform.GetComponentsInChildren<Transform>(true);
            for (int i = 1; i < heirarchy.Length; i++)
            {
                heirarchy[i].gameObject.SetActive(false);
            }
        }
#if Admob_Simple_Rizwan
        StartCoroutine(nameof(CheckIfRewardReady));
#endif
    }
    private void OnDisable()
    {
#if Admob_Simple_Rizwan
        StopCoroutine(nameof(CheckIfRewardReady));
#endif
    }
#if Admob_Simple_Rizwan
    IEnumerator CheckIfRewardReady()
    {
        while (AdsController.Instance ==null || AdsController.Instance._admobManager._rewardedAd[0] == null
              || AdsController.Instance._admobManager._rewardedAd[0].CanShowAd() == false)
        {
            yield return new WaitForSecondsRealtime(RecheckTime);
        }

        RewardedBtn.interactable = true;
        if (RewardedBtn.transform.childCount > 0)
        {
            for (int i = 0; i < heirarchy.Length; i++)
            {
                heirarchy[i].gameObject.SetActive(true);
            }
        }
    }
#endif
}

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
//BEGIN_IN_APP_REVIEW
/*
#if UNITY_IOS
using UnityEngine.iOS;
#elif UNITY_ANDROID
using Google.Play.Review;
#endif
*/


//END_IN_APP_REVIEW"
public class InAppReview : MonoBehaviour
{
    public static InAppReview Instance { get; private set; }
    //BEGIN_IN_APP_REVIEW
/*
[Tooltip("Check this if you want to show fake review first to controll bad rating")]
    public bool ShowFakeReviewDialogue = true;
    [Tooltip("Show Inapp Review,Note:this is just a request from our side and Playstore controls it")]
    public bool ShowInappReview = true;
    [Tooltip("If InApp Review is not shown by Playstore then Directly Open Game's Playstore Link")]
    [SerializeField] private bool ForceReview = true;

    [Tooltip("If Clicked Stars Count is Less then Allowed Stars then dont proceed")]
    [Range(0, 4)]
    [SerializeField] private int AllowedStarsLimit = 4;

    [Tooltip("True = Auto Fetch the Title from Player Settings")]
    [SerializeField] private bool AutoFetchTitle = true;

    [Tooltip("Playstore Id is autofetched through Bundle id" +
        "\"Appstore Id is something like this: 765436781\"")]
    [SerializeField] private string IOS_AppStoreId;

    private bool CanReviewAgain = true;
*/
  //END_IN_APP_REVIEW"
    [System.Serializable]
    public class ReviewDialogUI
    {
        public GameObject reviewDialog;
        public Button submitButton, laterButton;
        public Text Title;
        public GameObject[] stars;
        public int SelectedStars { get; private set; }

        public void SetSelectedStars(int count)
        {
            SelectedStars = count;
        }
    }

    [System.Serializable]
    public class AudioManager
    {
        public AudioSource audioSource;
        public AudioClip sadClip;
        public AudioClip happyClip;
        public AudioClip clickClip;
        public AudioClip openDialogClip;

        public void PlaySound(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }

    public ReviewDialogUI ui;
    public AudioManager audioManager;


    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(this.gameObject); }
    }
    //BEGIN_IN_APP_REVIEW
/*
#if UNITY_ANDROID
    private ReviewManager _reviewManager;
    private PlayReviewInfo _playReviewInfo;
    private bool _isReviewInitialized;
#endif
    private void Start()
    {
#if UNITY_ANDROID
        StartCoroutine(InitializeAndroidReview());
#endif
        InitializeUI();
    }

    private void InitializeUI()
    {
        if (ui.reviewDialog == null || ui.submitButton == null || ui.stars == null || ui.laterButton == null)
        {
            Debug.LogError("Review UI elements are not properly assigned.");
            return;
        }

        ui.reviewDialog.SetActive(false);
        if (AutoFetchTitle)
        {
            ui.Title.text = Application.productName;
        }

        // Add click listeners to stars
        for (int i = 0; i < ui.stars.Length; i++)
        {
            int index = i; // Prevent closure issues
            var button = ui.stars[i].GetComponent<Button>();
            if (button != null)
                button.onClick.AddListener(() => OnStarClicked(index + 1));
        }

        // Set submit button listener
        ui.submitButton.onClick.AddListener(OnSubmitClicked);
        ui.laterButton.onClick.AddListener(OnLaterClicked);
        ui.submitButton.interactable = false;
    }



    public void ReviewApp()
    {
        if (ShowFakeReviewDialogue)
        {
            ShowDialogBox();
        }
        else if (ShowInappReview)
        {
            RateAndReview_InApp();
        }
        else
        {
            OpenStorePage();
        }
    }

    private void ShowDialogBox()
    {
        if (ui.reviewDialog != null)
        {
            ui.reviewDialog.SetActive(true);
            audioManager.PlaySound(audioManager.openDialogClip);
        }
    }

    private void OnStarClicked(int starCount)
    {
        ui.SetSelectedStars(starCount);
        UpdateStarDisplay();
    }

    private void UpdateStarDisplay()
    {
        if (ui.stars == null || ui.stars.Length == 0) return;
        for (int i = 0; i < ui.stars.Length; i++)
        {
            var highlight = ui.stars[i].transform.GetChild(0);
            if (highlight != null)
                highlight.gameObject.SetActive(i < ui.SelectedStars);
        }
        if (ui.SelectedStars >= AllowedStarsLimit)
        {
            audioManager.PlaySound(audioManager.happyClip);
        }
        else
        {
            audioManager.PlaySound(audioManager.sadClip);
        }
        ui.submitButton.interactable = true;
    }
    private void OnLaterClicked()
    {

        audioManager.PlaySound(audioManager.clickClip);
        ui.reviewDialog.SetActive(false);
    }
    private void OnSubmitClicked()
    {
        audioManager.PlaySound(audioManager.clickClip);

        if (ui.SelectedStars >= AllowedStarsLimit)
        {
            CanReviewAgain = false;

            if (ShowInappReview)
                RateAndReview_InApp();
            else
                OpenStorePage();
        }

        ui.reviewDialog.SetActive(false);
    }

    private void RateAndReview_InApp()
    {
#if UNITY_EDITOR
        Debug.Log($"In-app review requested for {Application.productName} (Bundle: {Application.identifier})");
#elif UNITY_ANDROID
                                StartCoroutine(LaunchAndroidReview());
#elif UNITY_IOS
                                Device.RequestStoreReview();
#endif
    }

#if UNITY_ANDROID
    private IEnumerator InitializeAndroidReview()
    {
        if (_reviewManager == null)
            _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;

        if (requestFlowOperation.Error == ReviewErrorCode.NoError)
        {
            _playReviewInfo = requestFlowOperation.GetResult();
            _isReviewInitialized = true;
        }
        else if (ForceReview)
        {
            OpenStorePage();
        }
    }

    private IEnumerator LaunchAndroidReview()
    {
        if (!_isReviewInitialized)
        {
            yield return StartCoroutine(InitializeAndroidReview());
        }

        if (_playReviewInfo == null)
        {
            Debug.LogWarning("PlayReviewInfo is null, falling back to store page.");
            OpenStorePage();
            yield break;
        }

        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;

        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            Debug.LogWarning("Error launching review flow, opening store page.");
            OpenStorePage();
        }

        _playReviewInfo = null; // Reset after use
    }
#endif

    private void OpenStorePage()
    {
#if UNITY_ANDROID
        Application.OpenURL($"https://play.google.com/store/apps/details?id={Application.identifier}");
#elif UNITY_IOS
                                Application.OpenURL($"https://apps.apple.com/app/id{IOS_AppStoreId}");
#endif
    }
*/
  //END_IN_APP_REVIEW"
}

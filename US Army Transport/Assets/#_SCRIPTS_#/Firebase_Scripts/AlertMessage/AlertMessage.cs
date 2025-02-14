using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class AlertMessage : MonoBehaviour
{
    public static AlertMessage Instance;

    public GameObject alertPanel;
    public RectTransform titleBoxRect;
    public TextMeshProUGUI titleText;
    public RectTransform dialogueBoxRect;
    public TextMeshProUGUI messageText;

    [Range(0, 100)]
    public int paddingPercentageHorizontal = 20;
    public Action Yes, No;
    private Coroutine alertCoroutine;


    public Button yesButton;
    public Button noButton;
    public Button okayButton;
    private bool showAlertCalled;

    public RectTransform canvasRect;
    private TextMeshProUGUI tempText; // Cached temporary TextMeshProUGUI component

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        // Add listeners to the buttons
        yesButton.onClick.AddListener(OnClick_YesButton);
        noButton.onClick.AddListener(OnClick_NoButton);
        okayButton.onClick.AddListener(HideAlert);
    }
    private void OnDestroy()
    {
        // Destroy the temporary TextMeshProUGUI component when the script is destroyed
        //if (tempText != null)
        //{
        //    Destroy(tempText.gameObject);
        //}
    }

    private void OnEnable()
    {
        RegisterCloseEvent(); // Always register the event trigger on enable
    }

    private void OnDisable()
    {
        UnregisterCloseEvent(); // Always unregister the event trigger on disable
    }

    private void RegisterCloseEvent()
    {
        EventTrigger trigger = alertPanel.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = alertPanel.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventData) =>
        {
            HideAlert();
        });

        trigger.triggers.Add(entry);
    }
    private void UnregisterCloseEvent()
    {
        EventTrigger trigger = alertPanel.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            trigger.triggers.Clear();
            Destroy(trigger);
        }
    }

     void ShowAlert(string title, string message)
    {
        showAlertCalled = true;
        RegisterCloseEvent();
        SetAlertPanelTexts(title, message);
        SetAlertPanelButtons(false, false, false);
        titleText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(title));
        ShowAlertPanel();
    }

    public void ShowAlert_Ok(string title, string message)
    {

        showAlertCalled = true;
        UnregisterCloseEvent();
        SetAlertPanelTexts(title, message);
        SetAlertPanelButtons(false, false, true);
        titleText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(title));
        ShowAlertPanel();
    }

    public void ShowAlert_YesNo(string title, string message, Action yes, Action no)
    {

        showAlertCalled = true;
        UnregisterCloseEvent();
        SetAlertPanelTexts(title, message);
        SetAlertPanelButtons(true, true, false);
        titleText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(title));

        Yes = yes;
        No = no;
        ShowAlertPanel();
    }

    public void ShowAlert_AutoHide( string title, string message, float maxDisplayTime)
    {

        showAlertCalled = false; // Reset the showAlertCalled flag
        RegisterCloseEvent();
        SetAlertPanelTexts(title, message);
        SetAlertPanelButtons(false, false, false);
        titleText.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(title));

        if (alertCoroutine != null)
        {
            StopCoroutine(alertCoroutine);
        }

        alertCoroutine = StartCoroutine(ShowAndHideAlert(maxDisplayTime));
    }

    private System.Collections.IEnumerator ShowAndHideAlert(float displayTime)
    {
        ShowAlertPanel();

        // Wait for user interaction (click) or the display time, whichever comes first
        float elapsedTime = 0f;
        while (elapsedTime < displayTime && !showAlertCalled)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Hide the alert panel immediately if it wasn't manually closed
        if (!showAlertCalled)
        {
            HideAlert();
        }
    }

    public void HideAlert()
    {
        showAlertCalled = false; // Reset the flag
        alertPanel.SetActive(false);
    }

    private void ShowAlertPanel()
    {
        alertPanel.SetActive(true);    
        // Enable word wrapping to get the proper preferred size.
        messageText.enableWordWrapping = true;
        messageText.overflowMode = TextOverflowModes.Overflow;
  
      
       dialogueBoxRect.sizeDelta = new Vector2(GetCanvasMaxWidth(), messageText.preferredHeight);
        dialogueBoxRect.sizeDelta = new Vector2(GetCanvasMaxWidth(), messageText.preferredHeight);       
        
    }

    //private string originalMessage;

    private void SetAlertPanelTexts(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;
    }
    private void SetAlertPanelButtons(bool showYesButton, bool showNoButton, bool showOkayButton)
    {
        yesButton.gameObject.SetActive(showYesButton);
        noButton.gameObject.SetActive(showNoButton);
        okayButton.gameObject.SetActive(showOkayButton);
    }

    private float GetCanvasMaxWidth()
    {
        float canvasWidth = canvasRect.rect.width;
        // float canvasWidth = alertPanel.;
        float padding = canvasWidth * (paddingPercentageHorizontal / 100f); // Calculate padding value

        float maxCanvasWidth = canvasWidth - padding;
        return maxCanvasWidth;
    }

    public void OnClick_YesButton()
    {
        Yes?.Invoke();
        HideAlert();
    }

    public void OnClick_NoButton()
    {
        No?.Invoke();
        HideAlert();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MMManager : MonoBehaviour
{
    public static MMManager Instance;

    public GameObject mainMenuPanel;
    public GameObject modeSelectionPanel;
    public GameObject levelSelectionPanel;
    public GameObject loadingScreenPanel;
    public GameObject exitPanel;


    public Button[] LvlCards;


    private AsyncOperation async;
    public Image loadingBar;

    public Text[] Coins;

    public static int Levelno;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;


        if (PlayerPrefs.GetInt("UnlockedLevels") == 0)
        {
            PlayerPrefs.SetInt("UnlockedLevels", 1);
        }
        CheckUnlocked();
    }

    private void Start()
    {
        if (MySoundManager.instance)
            MySoundManager.instance.SetMainMenuMusic(true, 0.5f);

        SetCoins();
        Time.timeScale = 1f;

        Application.targetFrameRate = 120;

        AdsController.Instance?.ShowBannerAd_Admob(0);


    }

    public void GoToModeSelection()
    {
        if (MySoundManager.instance)
            MySoundManager.instance.PlayButtonClickSound(1f);

        StartCoroutine(LoadPanel("ModeSelection"));
    }




    public void GoToLevelSelection()
    {
        if (MySoundManager.instance)
            MySoundManager.instance.PlayButtonClickSound(1f);


        PlayInter();
        StartCoroutine(LoadPanel("LevelSelection"));


    }

    public void GoToMainMenu()
    {
        if (MySoundManager.instance)
            MySoundManager.instance.PlayButtonClickSound(1f);
        StartCoroutine(LoadPanel("MainMenu"));
    }




    IEnumerator LoadPanel(string sceneName)
    {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(false);
        modeSelectionPanel.SetActive(false);
        loadingScreenPanel.SetActive(true);

        yield return new WaitForSeconds(2f);
        if (sceneName == "MainMenu")
        {

        }
        if (sceneName == "LevelSelection")
        {
            levelSelectionPanel.SetActive(true);
        }

        if (sceneName == "ModeSelection")
        {
            modeSelectionPanel.SetActive(true);
        }

        loadingScreenPanel.SetActive(false);
    }
   



    public void LevelSel(int i) 
    {

        if (MySoundManager.instance)
            MySoundManager.instance.PlayButtonClickSound(1f);


        Levelno = i;

        PlayInter();
        StartCoroutine(LoadAsyncScene("GamePlay"));
    }


    //IEnumerator LoadAsyncScene(string sceneName)
    //{
    //    loadingScreenPanel.SetActive(true);
    //    loadingBar.gameObject.SetActive(true);
    //    async = SceneManager.LoadSceneAsync(sceneName);
    //    async.allowSceneActivation = false;
    //    while (!async.isDone)
    //    {
    //        if (async.progress >= 0.9f && loadingBar.fillAmount ==1f)
    //        {
    //            async.allowSceneActivation = true;
    //        }
    //        yield return null;
    //    }
    //}

    IEnumerator LoadAsyncScene(string sceneName)
    {
        yield return new WaitForSeconds(0.2f);
        PlayRect(true);
        loadingScreenPanel.SetActive(true);
        Transform parent = loadingBar.transform.parent.parent;
        Animator animator= parent.GetComponent<Animator>();
        animator.enabled = false;
        float timer = 0f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (timer < 5f)
        {
            if (timer < 5f)
            {
                timer += Time.deltaTime;
                float progress = Mathf.Clamp01(timer / 5f); 
                loadingBar.fillAmount = progress;
            }
            else
            {
               
                loadingBar.fillAmount = 1f;

                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

    

        yield return new WaitForSeconds(0.1f);
        PlayRect(false);
        asyncLoad.allowSceneActivation = true;
    }


    public void BackBtn(string S) 
    {
        if (S == "ModeSel")  
        {
            PanelActivity(ModeSel: true);
        }
        if (S == "LvlSel") 
        {
            PanelActivity(LvlSel: true);

        }
        if (S == "MM")
        {
            PanelActivity(MM: true);
        }
    }





    public void PanelActivity(bool MM = false, bool ModeSel = false, bool LvlSel = false)
    {

        if (mainMenuPanel)
        {
            mainMenuPanel.SetActive(MM);
        }

        if (modeSelectionPanel)
        {
            mainMenuPanel.SetActive(ModeSel);
        }

        if (levelSelectionPanel)
        {
            mainMenuPanel.SetActive(LvlSel);
        }

    }

    void SetCoins() 
    {
        foreach (Text txt in Coins) 
        {
            txt.text = PlayerPrefs.GetInt("Coins").ToString();
        }
    }

    void CheckUnlocked() 
    {
        int numUnlockedLevels = PlayerPrefs.GetInt("UnlockedLevels", 0);
        // Loop through all the level buttons in your UI
        for (int i = 1; i <= LvlCards.Length; i++)
        {
            // Get a reference to the button
            Button levelButton = LvlCards[i-1];

            if (levelButton != null)
            {
                // If this level is unlocked, make the button interactable
                if (i <= numUnlockedLevels)
                {
                    levelButton.interactable = true;
                    levelButton.transform.GetChild(1).gameObject.SetActive(false);

                }
                else
                {
                    // If this level is locked, make the button not interactable
                    levelButton.interactable = false;
                    levelButton.transform.GetChild(1).gameObject.SetActive(true);
                }
            }
        }

    }


    public void Exit(bool val) 
    {
        if (exitPanel.activeSelf)
        {

            if (val == true)
            {
                Application.Quit();
            }
            else
            {
                exitPanel.SetActive(false);
            }
        }
 
    }


    public void BackBtn()
    {

        if (mainMenuPanel.activeSelf)
        {
            exitPanel.SetActive(true);
        }

        if (modeSelectionPanel.activeSelf)
        {
            PlayInter();
            modeSelectionPanel.SetActive(false);
            mainMenuPanel.SetActive(true);
        }

        if (levelSelectionPanel.activeSelf)
        {
            levelSelectionPanel.SetActive(false);
            modeSelectionPanel.SetActive(true);
        }
    }

    public void PlayRect(bool val) 
    {
        if (val)
            AdsController.Instance.ShowBannerAd_Admob(1);

        else
            AdsController.Instance.HideBannerAd_Admob(1);
    }

    public void PlayInter()
    {
        AdsController.Instance?.ShowInterstitialAd_Admob();
    }

}

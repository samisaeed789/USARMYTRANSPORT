using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance;

    public GameObject RccCam;

    [Header("UI Related")]
    public GameObject CpFloat;
    public GameObject CpFloatYellow;
    public GameObject Filler;
    public GameObject loadingScreenPanel;
    public GameObject CompletePanel;
    public GameObject FailPanel;
    public GameObject cntrSel;
    public GameObject PausePnl;
    public GameObject blockInteract;
    public GameObject Objective;
    public CanvasGroup[] gpcanvas;
    public Text coinTxt;
    public Text timerTxt;
    public Image loadingBar;




    [Header("Pickups")]
    public GameObject Coins;
    public GameObject Gift;
    public GameObject Emoji;
    public ParticleSystem CheckpointbLUE;
    public ParticleSystem CheckpointYllw;


    [Header("Level Related")]
    public GameObject CurrVeh;
    public GameObject[] BothPlyrs;
    public GameObject DummyTruck;
    public GameObject TruckCam;
    public GameObject TempPark;
    public LevelStats[] lvlstats;
    GameObject Celebcam;
    public GameObject TEMP;



    [Header("CompltePnl")]
    public Text Timetxt;
    public Text CoinsEarnedlvltxt;


   [HideInInspector]public bool Shipload;
    [HideInInspector]private bool isRunning;
    public bool IsPressing;
    public bool test;
    

    public RCC_UIController Gas;
    public RCC_UIController Brake;



    public int ParkCount;
    int lvlnmbr;
    public int level;
    private float startTime;
    int nextlvl;
    int CoinsEarnedInLvl;
    float elapsedTime;


    public RCC_Demo RccCanv;

    Animator TruckAnim;
    private AsyncOperation async;
    public RCC_Camera Cam;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }


    private void Start()
    {

        Application.targetFrameRate = 60;

        if (test)
            lvlnmbr = level;

        else
            lvlnmbr =MMManager.Levelno;

        if (lvlnmbr>0)lvlstats[lvlnmbr - 1].gameObject.SetActive(true);

        coinTxt.text = PlayerPrefs.GetInt("Coins").ToString();

        SetMobileController(0);

        StartStopwatch();

    }

    GameObject UpperPlank;
    public void OnLevelStatsLoadedHandler(LevelStats levelStats)
    {
        //Setting Player Cars
        for (int i = 0; i < levelStats.Players.Length; i++)
        {
            levelStats.Players[i].SetActive(true);
            levelStats.Players[i].transform.position = levelStats.SpawnPoints[i].position;
            levelStats.Players[i].transform.rotation = levelStats.SpawnPoints[i].rotation;
            
        }
        //Setting Still Objects
        for (int i = 0; i < levelStats.StillObjects.Length; i++)
        {
            levelStats.StillObjects[i].SetActive(true);
            levelStats.StillObjects[i].transform.position = levelStats.StillObjectsPos[i].position;
            levelStats.StillObjects[i].transform.rotation = levelStats.StillObjectsPos[i].rotation;
        }

        //Sets Enable Animations At the start of level (if any)
        for (int i = 0; i < levelStats.EnableAnim.Length; i++)
        {
            levelStats.EnableAnim[i].gameObject.SetActive(true);
            levelStats.EnableAnim[i].enabled = true;
        }

        //Setting Dummy Carriage Cars(if any) 
        for (int i = 0; i < levelStats.CarraigeCars.Length; i++)
        {
            levelStats.CarraigeCars[i].gameObject.SetActive(true);
        }

        BothPlyrs = new GameObject[levelStats.Players.Length]; 

        for (int i = 0; i < levelStats.Players.Length; i++)
        {
            BothPlyrs[i] = levelStats.Players[i];
        }


        if (levelStats.TruckAnim)
            TruckAnim = levelStats.TruckAnim;


        CurrVeh = levelStats.Players[0];

        if (levelStats.CelebCam)
            Celebcam = levelStats.CelebCam;


        if (levelStats.collOcean)
            levelStats.collOcean.isTrigger = false;

        if (levelStats.Temppark)
            TempPark = levelStats.Temppark;


        if (levelStats.UpperPlank)
            levelStats.UpperPlank.SetActive(true);

        
        if (levelStats.ShipLoad)
            Shipload = levelStats.ShipLoad;
        
        
        if (levelStats.TruckCam)
            TruckCam = levelStats.TruckCam;

        if (levelStats.UpperPlank)
            UpperPlank = levelStats.UpperPlank;



        if (MySoundManager.instance)
            MySoundManager.instance.SetBGM(true,0.25f);


        if (loadingScreenPanel)
            loadingScreenPanel.SetActive(false);


        if (levelStats.EnableAnim.Length==0)
        {
            Canv(false);
            Objective.SetActive(true);
            Objective.GetComponent<Objectives>().SetObjective(lvlnmbr);
        }
       
    }


    private void Update()
    {
        if (IsPressing)
        {
            Gas.pressing = true;
            
        }

        if (isRunning)
        {
            // Calculate the elapsed time
            elapsedTime = Time.time - startTime;

            // Convert the elapsed time to minutes and seconds
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            // Display the time
            timerTxt.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        }
    }


    public void DelayCall(bool start = false,bool Off=false,bool RunCar=false) 
    {
        if (start) 
        {
            Invoke(nameof(StartFinishCam),1.5f);
        }
        
        if (Off) 
        {
            Invoke(nameof(OffFinishCam),1f);
        }
        
        if (RunCar) 
        {
            Invoke(nameof(RunCar),2f);
        }
    }


    public void RunCar() 
    {

        CurrVeh.GetComponent<Rigidbody>().isKinematic = false;
        CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = true;
        IsPressing = true;

    }


    public void StartFinishCam() 
    {
        TruckAnim.enabled = true;
        RccCam.GetComponent<RCC_Camera>().enabled = false;
        TruckCam.SetActive(true);
    }
    public void OffFinishCam() 
    {
        CurrVeh.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.Instance.CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = true;
        TruckAnim.enabled = false;
        RccCam.GetComponent<RCC_Camera>().enabled = true;
        TruckCam.SetActive(false);
        Canv(true);
        Objective.SetActive(true);
        Objective.GetComponent<Objectives>().SetObjective(lvlnmbr);
    }
        

    

    public void DelayNextCar() 
    {
       
        Invoke(nameof(ToNextCar),2f);
    }
    public void ToNextCar() 
    {
        Gas.pressing = false;
        CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = false;
        CurrVeh = BothPlyrs[1];
        CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = true;
        RccCam.GetComponent<RCC_Camera>().enabled = true;
        TruckCam.SetActive(false);

        if(TempPark)
            TempPark.SetActive(true);

        if (UpperPlank)
            UpperPlank.SetActive(false);


        for (int i = 0; i < lvlstats[lvlnmbr-1].ToOff.Length; i++)
        {
            lvlstats[lvlnmbr-1].ToOff[i].SetActive(false);
        }


        foreach (Transform child in lvlstats[lvlnmbr-1].Props.transform)
        {
            child.gameObject.SetActive(true);
        }

        foreach (Transform child in lvlstats[lvlnmbr-1].CPs.transform)
        {
            child.gameObject.SetActive(true);
        }

       // blockInteract.SetActive( false);

        for (int i = 0; i < gpcanvas.Length; i++)
        {
            gpcanvas[i].interactable = true;
            gpcanvas[i].alpha = 1;
        }


        Canv(true);
    }
    
    public void DelayLvlComp()
    {

        StartCoroutine(LevelComplete());
    }

    public IEnumerator  LevelComplete() 
    {


        StopStopwatch();


        yield return new WaitForSeconds(2f);
        RccCam.GetComponent<RCC_Camera>().enabled = false;
        if (TruckCam)
            TruckCam.SetActive(false);


        Celebcam.SetActive(true);





        if (MySoundManager.instance)
        {
            MySoundManager.instance.PlayFireworkSound(1f);
            MySoundManager.instance.PlayApplaudSound(true, 0.8f);
        }
        yield return new WaitForSeconds(8f);
        Canv(true);

        if (MySoundManager.instance)
        {

            MySoundManager.instance.PlayLevelCompleteSound(true,1f);
            MySoundManager.instance.Effectsource.Stop();
           
        }

        Celebcam.SetActive(false);
        PlayInter();

        CompletePanel.SetActive(true);
        PlayRect(true);
        SetCoinsinPanel();





        if (MMManager.Levelno == PlayerPrefs.GetInt("UnlockedLevels"))
        {
         //   MMManager.Levelno = MMManager.Levelno + 1;
            PlayerPrefs.SetInt("UnlockedLevels", PlayerPrefs.GetInt("UnlockedLevels") + 1);
        }



    }

    void SetCoinsinPanel()
    {
        int timetxt = Mathf.FloorToInt(elapsedTime * 2);
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 200+ timetxt);
        CoinsEarnedlvltxt.text = 0.ToString();
        Timetxt.text = 0.ToString();
        StartCoroutine(CounterAnimation(200,CoinsEarnedlvltxt));
        StartCoroutine(CounterAnimation(timetxt, Timetxt));
    }

   
    bool stopAnimation;
    private IEnumerator CounterAnimation(int totalCoins, Text coinstxt)
    {

        yield return new WaitForSeconds(1f);
        int duration = 3; // Total duration for the animation
        float elapsedTime = 0f; // Time elapsed since the start of the animation
        int currentCoins = 0;

        // Play sound if available
        if (MySoundManager.instance)
            MySoundManager.instance.PlayCounterAnim(1f);


           

        // Calculate the number of coins per second
        int coinsPerSecond = totalCoins / duration;

        // Loop until the animation reaches the total coins
        while (elapsedTime < duration && !stopAnimation)
        {
            elapsedTime += Time.deltaTime; // Accumulate elapsed time
            currentCoins = Mathf.FloorToInt(coinsPerSecond * elapsedTime); // Increment coins

            // Make sure currentCoins does not exceed totalCoins
            currentCoins = Mathf.Min(currentCoins, totalCoins);

            // Update the UI or text with the current number of coins
            coinstxt.text = currentCoins.ToString();

            yield return null; // Wait until the next frame
        }

        // Ensure the final count is exactly totalCoins
        coinstxt.text = totalCoins.ToString();

        // Stop sound if available
        if (MySoundManager.instance)
            MySoundManager.instance.StopCounterAnim();

    }
    public IEnumerator LevelFail()
    {
        if (MySoundManager.instance)
        {
            MySoundManager.instance.levelfailed = true;
            MySoundManager.instance.BGM.clip = null;
        }

        StopStopwatch();

        Canv(false);

        yield return new WaitForSeconds(3f);


        CurrVeh.SetActive(false);
        

        
        if (TruckCam)
            TruckCam.SetActive(false);

        

        if (MySoundManager.instance)
        {


            MySoundManager.instance.PlayLevelFailSound(true, 1f);
            MySoundManager.instance.Effectsource.Stop();
        }

        Celebcam.SetActive(false);
        FailPanel.SetActive(true);
        PlayRect(true);



    }



    public void Home() 
    {
        MySoundManager.instance.PlayButtonClickSound(1);
        PlayInter();

        StartCoroutine(LoadAsyncScene("MainMenu"));

    }

    public void Next()
    {
        MySoundManager.instance.PlayButtonClickSound(1);

        MMManager.Levelno = MMManager.Levelno + 1;
        PlayInter();
        StartCoroutine(LoadAsyncScene("GamePlay"));
    }
    
    public void Pause() 
    {

        MySoundManager.instance.PlayButtonClickSound(1);

        PausePnl.SetActive(true);

        if (MySoundManager.instance)
            MySoundManager.instance.SetBGM(false,0);

        PlayInter();
        PlayRect(true);


        Time.timeScale = 0f;
    }
    public void Resume()
    {
        MySoundManager.instance.PlayButtonClickSound(1);

        if (MySoundManager.instance)
            MySoundManager.instance.SetBGM(true, 0.25f);
       
        
        PlayRect(false);


        PausePnl.SetActive(false);
        Time.timeScale = 1f;


       
    }


    public void Restart()
    {
        MySoundManager.instance.PlayButtonClickSound(1);
        PlayInter();
        StartCoroutine(LoadAsyncScene("GamePlay"));
    }




    IEnumerator LoadAsyncScene(string sceneName)
    {
        yield return new WaitForSeconds(0.1f);
        Time.timeScale += 1f;
        loadingScreenPanel.SetActive(true);
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

    public void FellOcean() 
    {

        Cam.RemoveTarget();
        if (MySoundManager.instance)
            MySoundManager.instance.PlaySplash(1.0f);
        StartCoroutine(LevelFail());

    }


    public void Canv(bool val) 
    {

        if (val==false)
        {
            //blockInteract.SetActive(!val);

            for (int i = 0; i < gpcanvas.Length; i++)
            {
                gpcanvas[i].interactable = false;
                gpcanvas[i].alpha = 0;
            }
        }


        if (val)
        {
           // blockInteract.SetActive(val);
            for (int i = 0; i < gpcanvas.Length; i++)
            {
                gpcanvas[i].interactable = true;
                gpcanvas[i].alpha = 1;
            }
        }

    }


    public void StartStopwatch()
    {
        // Start the stopwatch
        startTime = Time.time;
        isRunning = true;
    }

    public void StopStopwatch()
    {
        // Stop the stopwatch
        isRunning = false;
    }

    public void ResetStopwatch()
    {
        // Reset the stopwatch
        startTime = Time.time;
    }



    public void AddCoins() 
    {
        PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + 10);
        coinTxt.text = PlayerPrefs.GetInt("Coins").ToString();
    }



    public void OpenController()
    {
        MySoundManager.instance.PlayButtonClickSound(1);
        cntrSel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void SetMobileController(int val) 
    {

        RccCanv.SetMobileController(val);
        cntrSel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Cpfloat(String S) 
    {
        if (S == "Blue") 
        {
            CheckpointbLUE.Play();
            CpFloat.SetActive(true);

        }

        if (S == "Yellow")
        {
            CheckpointYllw.Play();
            CpFloatYellow.SetActive(true);
        }
        MySoundManager.instance.PlayCPSound(1f);
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

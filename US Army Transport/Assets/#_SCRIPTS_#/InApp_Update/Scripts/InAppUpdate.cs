using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//BEGIN_IN_APP_UPDATE
/*
using Google.Play.AppUpdate;
using Google.Play.Common;
*/


//END_IN_APP_UPDATE
using UnityEngine.UI;

public class InAppUpdate : MonoBehaviour
{
    //BEGIN_IN_APP_UPDATE
/*
private AppUpdateManager appUpdateManager;
    public static InAppUpdate Instance { get; private set; }
    [SerializeField] bool AskToUpdate = true;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (this.transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            this.appUpdateManager = new AppUpdateManager();
        }
    }
#if UNITY_ANDROID
    private void Start()
    {
        if (AskToUpdate)
        {

            if (Application.platform == RuntimePlatform.Android)
            {

                StartCoroutine(CheckForUpdate());
            }
        }
    }

    IEnumerator CheckForUpdate()
    {

        PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> appUpdateInfoOperation =
          appUpdateManager.GetAppUpdateInfo();

        yield return appUpdateInfoOperation;

        if (appUpdateInfoOperation.Error == AppUpdateErrorCode.ErrorUnknown)
        {
            Debug.LogError("there is some errors");
        }
        if (appUpdateInfoOperation.IsSuccessful)
        {
            var appUpdateInfoResult = appUpdateInfoOperation.GetResult();

            if (appUpdateInfoResult.UpdateAvailability == UpdateAvailability.UpdateAvailable && appUpdateInfoResult.IsUpdateTypeAllowed(AppUpdateOptions.ImmediateAppUpdateOptions()))
            {
                var appUpdateOptions = AppUpdateOptions.ImmediateAppUpdateOptions();
                StartCoroutine(StartImmediateUpdate(appUpdateInfoResult, appUpdateOptions));
            }
        }
        else
        {
            Debug.LogError("there is no update for now");
        }
    }

    IEnumerator StartImmediateUpdate(AppUpdateInfo appUpdateInfo_i, AppUpdateOptions appUpdateOptions_i)
    {
        var startUpdateRequest = appUpdateManager.StartUpdate(appUpdateInfo_i, appUpdateOptions_i);
        yield return startUpdateRequest;
    }
#endif
*/
  //END_IN_APP_UPDATE
}



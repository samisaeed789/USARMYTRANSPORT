#if Admob_Simple_Rizwan
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
#endif
using System;
using UnityEngine;
using UnityEngine.UI;
public class GoogleMobileAdsConsentController : MonoBehaviour
{

    public static Action Showform;

    /// <summary>
    /// If true, it is safe to call MobileAds.Initialize() and load Ads.
    /// </summary>
#if Admob_Simple_Rizwan

    public bool CanRequestAds =>
          ConsentInformation.ConsentStatus == ConsentStatus.Obtained ||
          ConsentInformation.ConsentStatus == ConsentStatus.NotRequired;



    public void GatherConsent(Action<string> onComplete)
    {
        Debug.Log("Gathering consent.");
        var requestParameters = new ConsentRequestParameters
        {
            // False means users are not under age.
            TagForUnderAgeOfConsent = false

            //ConsentDebugSettings = new ConsentDebugSettings
            //{
            //    // For debugging consent settings by geography.
            //    DebugGeography = DebugGeography.Disabled,
            //    // https://developers.google.com/admob/unity/test-ads
            //    //TestDeviceHashedIds = GoogleMobileAdsController.TestDeviceIds,
            //}


        };

        if (onComplete == null)
        {
            return;
        }

        ConsentInformation.Update(requestParameters, (FormError updateError) =>
        {
            if (updateError != null)
            {
                onComplete(updateError.Message);
                return;
            }

            Debug.Log("Consent information updated.");

            // Determine the consent-related action to take based on the ConsentStatus.
            if (CanRequestAds)
            {
                // Consent has already been gathered or not required.
                // Return control back to the user.
                onComplete(null);
                return;
            }

            // Consent not obtained and is required.
            // Load the initial consent request form for the user.
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
            {
                if (showError != null)
                {
                    // Consent gathering failed.
                    if (onComplete != null)
                    {
                        onComplete(showError.Message);
                    }
                    return;
                }
                else
                {
                    Showform?.Invoke();
                }
            });
        });
    }


    public void ShowPrivacyOptionsForm(Action<string> onComplete)
    {
        Debug.Log("Showing privacy options form.");

        if (onComplete == null)
        {
            return;
        }

        ConsentForm.LoadAndShowConsentFormIfRequired((FormError showError) =>
        {

            if (showError != null)
            {
                // Consent gathering failed.
                if (onComplete != null)
                {
                    onComplete(showError.Message);
                }
                return;
            }
            // Form showing succeeded.
            else if (onComplete != null)
            {
                Showform?.Invoke(); //update
                onComplete(null);
            }
        });
    }

#endif
}
using UnityEngine;
#if UNITY_IOS 
// Include the IosSupport namespace if running on iOS:
using Unity.Advertisement.IosSupport;

#endif

public class AttPermissionRequest : MonoBehaviour
{
    void Awake()
    {
#if UNITY_IOS 
        // Check the user's consent status.
        // If the status is undetermined, display the request request:
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
        /******IMPORTANT******/
        //if upper code is showing error then Import "IOS 14 Advertising Support" from Package Manager Unity Registry.
        //Some times a unity Restart is also required
#endif
    }
}
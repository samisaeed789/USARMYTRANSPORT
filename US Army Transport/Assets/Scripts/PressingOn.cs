using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressingOn : MonoBehaviour
{
    public void PressOn() 
    {

        if (MySoundManager.instance)
            MySoundManager.instance.StopRampSound();

        GameManager.Instance.CurrVeh.GetComponent<Rigidbody>().isKinematic = false;
        GameManager.Instance.CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = true;
        GameManager.Instance.IsPressing = true;

    }

    public void OffAnim()
    {
       GameManager.Instance.DelayCall(Off:true);
    }
    public void StartSound()
    {
        if (MySoundManager.instance)
            MySoundManager.instance.PlayRampSound(0.5f);
    }

    public void OffCanv() 
    {
        GameManager.Instance.Canv(false);
    }




}

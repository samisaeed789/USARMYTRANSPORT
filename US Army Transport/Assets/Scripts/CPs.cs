using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPs : MonoBehaviour
{

    public bool IsBlue;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            if(IsBlue)
                GameManager.Instance.CpFloat.SetActive(true);
            
            else
                GameManager.Instance.CpFloatYellow.SetActive(true);



            MySoundManager.instance.PlayCPSound(1f);
            this.gameObject.SetActive(false);
        }
    }


}

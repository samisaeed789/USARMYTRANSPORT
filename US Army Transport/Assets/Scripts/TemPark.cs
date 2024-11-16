using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemPark : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
         
            GameManager.Instance.CurrVeh.transform.position = this.transform.GetChild(2).position;
            GameManager.Instance.CurrVeh.transform.rotation = this.transform.GetChild(2).rotation;
            GameManager.Instance.CurrVeh.GetComponent<Rigidbody>().isKinematic = true;
            GameManager.Instance.CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = false;
            GameManager.Instance.Canv(false);


            GameManager.Instance.DelayCall(start: true);

            if (GameManager.Instance.ParkCount == 1)
            {
                if (GameManager.Instance.Shipload)
                {

                }
                else
                {
                    GameManager.Instance.DelayCall(RunCar: true);
                }

            }

            this.gameObject.SetActive(false);
       
        }
    }


}

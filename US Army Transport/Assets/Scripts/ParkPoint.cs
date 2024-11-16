using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParkPoint : MonoBehaviour
{



    public GameObject filler;
    public float fillSpeed = 0.5f;

    private float fillAmount = 0.0f;


    public bool FinalPark;

    
    void Start()
    {
       filler = GameManager.Instance.Filler;
       filler.SetActive(false);
    }

    bool once = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {

            if (MySoundManager.instance && !once)
            {
                MySoundManager.instance.PlayParkedSound(1f);
                once = true;
            }
            GameManager.Instance.CurrVeh.transform.position = this.transform.GetChild(2).position;
            GameManager.Instance.CurrVeh.transform.rotation = this.transform.GetChild(2).rotation;
            GameManager.Instance.CurrVeh.GetComponent<Rigidbody>().isKinematic = true;
            GameManager.Instance.CurrVeh.GetComponent<RCC_CarControllerV3>().enabled = false;
            GameManager.Instance.IsPressing = false;
            GameManager.Instance.Gas.pressing = false;
            GameManager.Instance.Canv(false);



        }

    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Increase the fill amount while the car is inside the collider
            fillAmount += fillSpeed * Time.deltaTime;
            fillAmount = Mathf.Clamp01(fillAmount);
            UpdateFiller();
        }
      
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the exiting object is a car
        if (other.CompareTag("Player"))
        {
            // Reset the fill amount when the car exits the collider
            fillAmount = 0.0f;
            UpdateFiller();
        }
    }

    void UpdateFiller()
    {
     
        filler.transform.GetChild(0).GetComponent<Image>().fillAmount = fillAmount;

        filler.SetActive(fillAmount > 0.0f);



        if (fillAmount >= 1.0f)
        {

            if (MySoundManager.instance)
                MySoundManager.instance.PlayVO(1f);


            filler.SetActive(false);

            if (FinalPark) 
            {
                GameManager.Instance.DelayLvlComp();
                this.gameObject.SetActive(false);
                GameManager.Instance.Canv(false);
            }
            else 
            {

                GameManager.Instance.ParkCount++;

                if (GameManager.Instance.ParkCount == 1)
                {
                    GameManager.Instance.DelayNextCar();

                }
                else if (GameManager.Instance.ParkCount == 2)
                {
                    GameManager.Instance.Canv(false);

                    GameManager.Instance.DelayLvlComp();
                }

                this.gameObject.SetActive(false);

            }
         

        }
        else
        {
            // Activate the filler
            filler.SetActive(true);
        }
    }
}




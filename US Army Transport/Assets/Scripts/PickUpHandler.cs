using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpHandler : MonoBehaviour
{

    public bool Gift;
    public bool Emoji;
    public bool Coins;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            if (Gift) 
            {
                if (MySoundManager.instance)
                    MySoundManager.instance.PlayCashSound(0.8f);
                
                GameManager.Instance.Gift.SetActive(true);
            }

            if (Emoji)
            {
                if (MySoundManager.instance)
                    //MySoundManager.instance.PlayCashSound(1f);

                GameManager.Instance.Emoji.SetActive(true);
            }

            if (Coins)
            {
                if (MySoundManager.instance)
                    MySoundManager.instance.PlaycoinSound(1f);
                GameManager.Instance.Coins.SetActive(true);
                GameManager.Instance.AddCoins();

            }
            this.gameObject.SetActive(false);
           // Destroy(this.gameObject,1f);
        }
    }


}

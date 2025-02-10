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
            if (IsBlue)
                GameManager.Instance.Cpfloat("Blue");

            else
                GameManager.Instance.Cpfloat("Yellow");



            this.gameObject.SetActive(false);
        }
    }


}

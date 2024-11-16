using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableVeh : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            GameManager.Instance.FellOcean();
        }
    }




}

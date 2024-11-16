using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierSalute : MonoBehaviour
{


    public Animator[] Soldiers;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player") 
        {
            foreach(Animator anim in Soldiers) 
            {
                anim.Play("Salute");
            }
        }
    }




}

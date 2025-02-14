using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvlselanim : MonoBehaviour
{
    

    private void OnEnable()
    {
        this.GetComponent<Animator>().enabled = true;
    }
    private void OnDisable()
    {
        offAnimator();
    }

    void offAnimator() 
    {
        this.GetComponent<Animator>().enabled = false;

    }

}

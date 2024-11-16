using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableGameObject : MonoBehaviour
{
    public float timeS;
    private void OnEnable()
    {
        Invoke("disableThisObject", timeS);
    }

    public void disableThisObject()
    {
        gameObject.SetActive(false);
    }
}

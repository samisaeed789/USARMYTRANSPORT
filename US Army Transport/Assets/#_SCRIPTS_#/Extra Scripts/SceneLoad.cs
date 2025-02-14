using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       // yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);
    }

   
}

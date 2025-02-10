using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Objectives : MonoBehaviour
{
   // public string[] Objtxt;
    public Text objectiveText;
    public float typingSpeed = 0.05f; // Adjust the typing speed as needed
    public GameObject okbtn;


    public LevelObjectives[] lvlobj;
    int Count = 0;
    public void SetObjective(int i)
    {
       
        // Get the objective text from the input field
       // string newObjective = Objtxt[i];
        objectiveText.text = "";
        // Start the typing animation
        StartCoroutine(TypeObjective(lvlobj[i].objectives[Count]));
        Count++;

        // You can store the objective text for later use, such as in game logic
        // For now, let's just print it to the console
    }

    IEnumerator TypeObjective(string objective)
    {
        // Clear the existing text
        //objectiveText.text = "Objective: ";

        // Type the objective text letter by letter
        foreach (char letter in objective)
        {
            objectiveText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        okbtn.SetActive(true);
    }


    public void Ok() 
    {
        MySoundManager.instance.PlayButtonClickSound(1);
        this.gameObject.SetActive(false);
        GameManager.Instance.Canv(true);
        
    }
}
[System.Serializable]
public class LevelObjectives
{
    public string[] objectives;
}

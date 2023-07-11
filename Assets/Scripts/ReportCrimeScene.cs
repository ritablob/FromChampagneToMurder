using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportCrimeScene : MonoBehaviour
{
    public List<string> possibleCorrectAnswers = new List<string>();
    public TMP_Text inputField;
    public GameObject ExitButton;
    public Animator reportAnimator;
    public bool DoesTextMatch()
    { 
        for (int i = 0; i < possibleCorrectAnswers.Count; i++)
        {

            if (CompareWords(inputField.text, possibleCorrectAnswers[i]))
            {
                Debug.Log("its a mathc!!!");
                return true;
            }
            Debug.Log(inputField.text == possibleCorrectAnswers[i]);
            Debug.Log("not a match :(, text: " + "-"+inputField.text+ "-"+ possibleCorrectAnswers[i] + "-");
            //Debug.Log(inputField.text.GetType() + " " + possibleCorrectAnswers[i].GetType());// it does go through and read the possible answers
        }
        Debug.Log("not a match :(, text: "+ inputField.text);
        ClearTextBox();
        return false;

    }
    public void ClearTextBox()
    {
        inputField.text = string.Empty;
    }
    public void HideExitButton()
    {
        ExitButton.SetActive(false);
    }
    bool CompareWords(string wordOne, string wordTwo)
    {
        wordOne = wordOne.ToLower();
        wordOne = wordOne.TrimEnd();
        wordOne = wordOne.TrimStart();
        wordOne = wordOne.Trim();
        wordTwo = wordTwo.TrimEnd();
        wordTwo = wordTwo.TrimStart();
        wordTwo = wordTwo.Trim();
        wordTwo = wordTwo.ToLower();

        return wordOne == wordTwo;
    }
}
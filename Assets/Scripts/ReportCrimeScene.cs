using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportCrimeScene : MonoBehaviour
{
    public List<string> possibleCorrectAnswers = new List<string>();
    public TMP_InputField inputField;
    public bool DoesTextMatch()
    { 
        for (int i = 0; i < possibleCorrectAnswers.Count; i++)
        {
            if (inputField.text.ToLower() == possibleCorrectAnswers[i].ToLower())
            {
                Debug.Log("its a mathc!!!");
                return true;
            }
        }
        Debug.Log("not a match :(");
        ClearTextBox();
        return false;

    }
    public void ClearTextBox()
    {
        inputField.text = string.Empty;
    }
}

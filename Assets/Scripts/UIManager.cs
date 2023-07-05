using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject minimap;
    public GameObject map;
    public GameObject textBox;
    public GameObject reportScreen;

    public void SwitchToMinimap()
    {
        minimap.SetActive(true);
        map.SetActive(false);
    }
    public void SwitchToMap()
    {
        map.SetActive(true);
        minimap.SetActive(false);
    }

    // ----- report crime screen --------
    public void ReportScreenOpen()
    {
        reportScreen.SetActive(true);
        DisableInputReportScreen();
    }
    public void ReportScreenClose()
    {
        reportScreen.SetActive(false);
        EnableInputReportScreen();
    }
    public void ReportScreenConfirm()
    {
        // takes text from input field
        // parses it 
        // checks if it is correct
        // if wrong, gives you one more try
        // if wrong twice, game over
        // if correct, win game 

        Debug.Log("pressed Report scene");
    }
    public void DisableInputReportScreen() // so that there is no overlapping input
    {
        textBox.SetActive(false);
    }
    public void EnableInputReportScreen()
    {
        textBox.SetActive(true);
    }
}

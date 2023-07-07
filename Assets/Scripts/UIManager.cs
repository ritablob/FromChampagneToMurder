using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Color buttonSelectedColor = Color.cyan; 
    public Color buttonNormalcolor = Color.blue;
    public GameObject minimap;
    public GameObject map;
    public GameObject textBox;
    public ReportCrimeScene reportScreen;
    public CameraMovingManager minimapCamera;
    private Image previouslySelectedButtonImage;

    int triesTaken;

    public void SwitchToMinimap(Image buttonImage)
    {
        SwitchColor(buttonImage);
        minimap.SetActive(true);
        map.SetActive(false);
        minimapCamera.SwitchToMinimap();
    }
    public void SwitchToMap(Image buttonImage)
    {
        SwitchColor(buttonImage);
        map.SetActive(true);
        minimap.SetActive(false);
        minimapCamera.SwitchToMap();
    }

    public void SwitchColor(Image buttonImage)
    {
        buttonImage.color = buttonSelectedColor;
        if (previouslySelectedButtonImage != null)
        {
            previouslySelectedButtonImage.color = buttonNormalcolor;
        }
        previouslySelectedButtonImage = buttonImage;
    }
    // ----- report crime screen --------
    public void ReportScreenOpen()
    {
        reportScreen.gameObject.SetActive(true);
        DisableInputReportScreen();
    }
    public void ReportScreenClose()
    {
        reportScreen.gameObject.SetActive(false);
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
        if (reportScreen.DoesTextMatch())
        {
            //SceneManager.LoadScene(1); load ending win scene
        }
        else
        {
            triesTaken++;
            if (triesTaken > 1)
            {
                //SceneManager.LoadScene(0); load ending lose scene
            }
        }
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

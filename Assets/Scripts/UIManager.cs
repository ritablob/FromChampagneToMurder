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
    public GameObject pinButtons;
    public GameObject map;
    public GameObject textBox;
    public ReportCrimeScene reportScreen;
    public CameraMovingManager minimapCamera;
    [SerializeField] Image previouslySelectedButtonImage;



    public void Start()
    {
        SwitchToMinimap();
    }

    public void SwitchToMinimap(Image buttonImage = null)
    {
        if (buttonImage != null) SwitchColor (buttonImage);
        minimap.SetActive(true);
        map.SetActive(false);
        minimapCamera.SwitchToMinimap();
        pinButtons.SetActive(false);
    }
    public void SwitchToMap(Image buttonImage = null)
    {
        if (buttonImage != null) SwitchColor(buttonImage);
        map.SetActive(true);
        minimap.SetActive(false);
        minimapCamera.SwitchToMap();
        pinButtons.SetActive(true);
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

        Debug.Log("pressed Report scene");
        if (reportScreen.DoesTextMatch())
        {
            SceneManager.LoadScene(2); // ending win scene
        }
        else
        {
            SceneManager.LoadScene(3); // ending lose scene
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

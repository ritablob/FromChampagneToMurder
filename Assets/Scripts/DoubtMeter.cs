using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoubtMeter : MonoBehaviour
{
    public GameManager gameManager;
    public int doubtLimit;

    public bool dontReduceDoubtAtMaxValue = false;

    Slider slider;
    private int sliderValue;
    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = doubtLimit;
        slider.value = 0;
    }
    public void AddDoubt(int doubtImpact)
    {
        if (slider.value < slider.maxValue) // below max value
        {
            slider.value += doubtImpact;
            sliderValue += doubtImpact;
        }
        else // equal max value (slider doesnt go above max value)
        {
            if (doubtImpact > 0)
            {
                Debug.LogError("Starting ending");
                gameManager.startingEnding = true;
            }
            else if (dontReduceDoubtAtMaxValue && doubtImpact < 0)
            {
                doubtImpact = 0;
            }
            else
            {
                slider.value += doubtImpact;
                sliderValue += doubtImpact;
            }
        }

    }
    public void ValueChange() // probably redundant but just in case
    {
        if (sliderValue > slider.maxValue)
        {
            gameManager.startingEnding = true;
        }
    }
}

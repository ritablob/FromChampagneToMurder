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

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.maxValue = doubtLimit;
        slider.value = 0;
    }
    public void AddDoubt(int doubtImpact)
    {
        if (slider.value <= slider.maxValue) // below or equal max value 
        {
            slider.value += doubtImpact;
        }
        else // above max value 
        {
            if (doubtImpact > 0)
            {
                gameManager.startingEnding = true;
            }
            else if (dontReduceDoubtAtMaxValue && doubtImpact < 0)
            {
                doubtImpact = 0;
            }
            else
            {
                slider.value += doubtImpact;
            }
        }

    }
}

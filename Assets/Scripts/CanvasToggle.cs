using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasToggle : MonoBehaviour
{
    public GameObject minimap;
    public GameObject map;

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
}

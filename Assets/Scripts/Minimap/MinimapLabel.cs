using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MinimapLabel : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    public void EditLabelText(string text)
    {
        tmp.text = text;
    }
    private void OnEnable()
    {
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }
}

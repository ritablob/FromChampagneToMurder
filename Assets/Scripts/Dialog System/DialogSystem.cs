using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogTextBox;
    // Start is called before the first frame update
    public int dialogueTracker;
    void Start()
    {
        dialogueTracker = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void DialogNode(string text, int dialogIndex)
    {
        if (Input.GetKeyDown(KeyCode.Space) && dialogIndex == dialogueTracker)
        {
            dialogTextBox.text = text;
            dialogueTracker++;
        }
    }
}

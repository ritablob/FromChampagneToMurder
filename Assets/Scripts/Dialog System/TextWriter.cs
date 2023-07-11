using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static TestAnimationsCharacter;

public class TextWriter : MonoBehaviour
{
    public float typingSpeed = 0.1f;
    public float typingSpeedPrevious = 0f;
    GameManager parser;
    [HideInInspector]
    public bool finishedTyping = false;
    public CharacterAnimator characterAnimator;

    public TMP_StyleSheet default_style_sheet;
    public TMP_StyleSheet previous_text_style_sheet;

    [SerializeField] string starting_emotion = "Normal";

    public int current_node_index;
    private bool displaying_previous;

    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    private void Awake()
    {
        parser = GetComponent<GameManager>();
    }

    public void WriteText(string emotion = "Normal")
    {
        tmp.styleSheet = default_style_sheet;
        current_node_index = parser.traversedNodesList.Count - 1;
        //Debug.LogWarning("typing Start");
        tmp.maxVisibleCharacters = 0;
        finishedTyping = false;

        characterAnimator.StartTalking(emotion);                                               // Character Animations

        tmp.text = parser.graph.nodes[parser.currentNodeIndex].attributes.characterDialogue; // this is the problem
        //Debug.Log(nodeID);
        StartCoroutine(AnimateTypewriter(tmp, typingSpeed));
    }

    IEnumerator AnimateTypewriter(TextMeshProUGUI tmp, float typing_speed) // simple typing animation
    {
        yield return new WaitForEndOfFrame();
        if (tmp.maxVisibleCharacters < tmp.textInfo.characterCount)
        {
            tmp.maxVisibleCharacters++;
            yield return new WaitForSeconds(typing_speed);
            StartCoroutine(AnimateTypewriter(tmp, typing_speed));
        }
        else
        {
            finishedTyping=true;                                                            
            //TalkingFinish.Invoke();
            //Debug.LogWarning("finished typing");

            characterAnimator.FinishTalking();                                                // Character Animations

            yield return null;
        }
        yield return null;
    }

    private void WritePreviousText(int node_index)
    {
        tmp.styleSheet = previous_text_style_sheet;
        finishedTyping = false;
        tmp.text = parser.traversedNodesList[node_index].attributes.characterDialogue;
    }

    public void WriteTraversedPrevious()
    {
        if (current_node_index <= 0) return;
        
        displaying_previous = true;
        current_node_index--;
        WritePreviousText(current_node_index);
    }

    public void WriteTraversedNext()
    {
        if (current_node_index >= parser.traversedNodesList.Count - 1)
        {
            return;
        }
        if (current_node_index == parser.traversedNodesList.Count - 2)
        {
            WriteActiveAgain();
            return;
        }
        displaying_previous = true;
        current_node_index++;
        WritePreviousText(current_node_index);
    }

    public void WriteActiveAgain()
    {
        tmp.styleSheet = default_style_sheet;
        displaying_previous = false;
        current_node_index++;
        tmp.text = parser.graph.nodes[parser.currentNodeIndex].attributes.characterDialogue;
        finishedTyping = true;
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.LeftArrow)) { WriteTraversedPrevious(); }
        if (Input.GetKeyUp(KeyCode.RightArrow)) { WriteTraversedNext(); }
    }
}

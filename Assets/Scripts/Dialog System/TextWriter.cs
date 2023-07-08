using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextWriter : MonoBehaviour
{
    public float typingSpeed = 0.1f;
    GameManager parser;
    [HideInInspector]
    public bool finishedTyping = false;
    public TestAnimationsCharacter testAnimations;

    [SerializeField] string starting_emotion = "Normal";


    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    private void Awake()
    {
        parser = GetComponent<GameManager>();
    }
    void Start()
    {
        WriteText();
    }

    public void WriteText(string emotion = "Normal")
    {
        //Debug.LogWarning("typing Start");
        tmp.maxVisibleCharacters = 0;
        finishedTyping = false;

        //testAnimations.StartTalking(emotion);                                               // Character Animations

        tmp.text = parser.graph.nodes[parser.currentNodeIndex].attributes.characterDialogue; // this is the problem
        //Debug.Log(nodeID);
        StartCoroutine(AnimateTypewriter(tmp));
    }

    IEnumerator AnimateTypewriter(TextMeshProUGUI tmp) // simple typing animation
    {
        yield return new WaitForEndOfFrame();
        if (tmp.maxVisibleCharacters < tmp.textInfo.characterCount)
        {
            tmp.maxVisibleCharacters++;
            yield return new WaitForSeconds(typingSpeed);
            StartCoroutine(AnimateTypewriter(tmp));
        }
        else
        {
            finishedTyping=true;                                                            
            //TalkingFinish.Invoke();
            //Debug.LogWarning("finished typing");

            testAnimations.FinishTalking();                                                // Character Animations

            yield return null;
        }
        yield return null;
    }
}

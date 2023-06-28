using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextWriter : MonoBehaviour
{
    public float typingSpeed = 0.1f;
    ParseJson parser;
    [HideInInspector]
    public bool finishedTyping = false;


    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    private void Awake()
    {
        parser = GetComponent<ParseJson>();
    }
    void Start()
    {
        WriteText();
    }

    public void WriteText()
    {
        Debug.LogWarning("typing Start");
        tmp.maxVisibleCharacters = 0;
        finishedTyping = false;
        tmp.text = parser.graph.nodes[parser.nodeID].attributes.characterDialogue;
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
            Debug.LogWarning("finished typing");
            yield return null;
        }
        yield return null;
    }
}

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
    //[HideInInspector]
    public int nodeID;
    public int previousNodeID;

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

    void WriteText()
    {
        tmp.maxVisibleCharacters = 0;
        tmp.text = parser.graph.nodes[nodeID].attributes.characterDialogue;
        //Debug.Log(nodeID);
        StartCoroutine(AnimateTypewriter(tmp));
    }
    public void FindNextNodeID(string linkName)  // move to different script (maybe GameManager or ParseJson)
    {
        int nextNodeID;
        for (int i = 0; i < parser.graph.edges.Length; i++)
        {
            // check if the edge is right + make sure we are clicking on the right link
            if (parser.graph.edges[i].source == nodeID && parser.graph.edges[i].attributes.label == linkName)
            {
                nextNodeID = parser.graph.edges[i].target;
                previousNodeID = nodeID;
                nodeID = nextNodeID;
                WriteText();
            }
        }
        //Debug.LogWarning("Next node does not exist! Node ID - " + nodeID + ", clicked link - " + linkName);
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
        yield return null;
    }
}

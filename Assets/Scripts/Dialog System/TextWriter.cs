using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextWriter : MonoBehaviour
{
    ParseJson parser;
    int nodeID;
    string clickedLinkText;

    public TextMeshProUGUI tmp;

    // Start is called before the first frame update
    private void Awake()
    {
        parser = GetComponent<ParseJson>();
    }
    void Start()
    {
        tmp.text = parser.graph.nodes[0].attributes.characterDialogue;
        nodeID = 1;
    }

    // test code
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            tmp.text = parser.graph.nodes[nodeID].attributes.characterDialogue;
            nodeID++;
        }
    }

    void WriteText(int nodeID, TextMeshProUGUI tmp)
    {
        tmp.text = parser.graph.nodes[nodeID].attributes.characterDialogue;
    }
    /// <summary>
    /// Get the node next in the graph, based on which link has been clicked. 
    /// </summary>
    /// <param name="currentNodeID"></param>
    /// <returns></returns>
    Nodes GetNextNode(int currentNodeID)
    {
        int nextNodeID;
        for (int i = 0; i < parser.graph.edges.Length; i++)
        {
            // check if the edge is right + make sure we are clicking on the right link
            if (parser.graph.edges[i].source == currentNodeID && parser.graph.edges[i].attributes.label == clickedLinkText)
            {
                nextNodeID = parser.graph.edges[i].target;
                return parser.graph.nodes[nextNodeID];
            }
        }
        Debug.LogWarning("Next node does not exist! Node ID - " + currentNodeID +", clicked link - " + clickedLinkText);
        return null;
    }
}

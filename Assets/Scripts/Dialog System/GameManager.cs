using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using UnityEngine.Networking.Types;
using System.Text.RegularExpressions;
using UnityEditorInternal;

/// <summary>
/// parses the json file and puts all the data into the Nodegraph variable :)
/// <para>Basically the game manager script lol</para>
/// </summary>
public class GameManager : MonoBehaviour
{
    public NodeGraph graph;
    public TextAsset jsonFile;
    public DoubtMeter doubtm;

    [HideInInspector] public int currentNodeKey;
    [HideInInspector] public int previousNodeKey;
    [HideInInspector] public int currentNodeIndex;

    TextWriter writer;


    void Awake()
    {
        graph = NodeGraph.CreateFromJSON(jsonFile.text);
        writer = GetComponent<TextWriter>();
    }

    public void FindNextNodeID(string linkName)
    {
        //Debug.Log("Finding node ID, link - " + linkName);
        int nextNodeKey;
        for (int i = 0; i < graph.edges.Length; i++)
        {
            // check if the edge is right + make sure we are clicking on the right link
            if (graph.edges[i].source == currentNodeKey && graph.edges[i].attributes.label.ToLower() == linkName)
            {
                nextNodeKey = graph.edges[i].target;
                doubtm.ChangeDoubtMeter(nextNodeKey);
                CheckIfEdgesAreValid(nextNodeKey);
                previousNodeKey = currentNodeKey;
                currentNodeKey = nextNodeKey;
                writer.WriteText();
                break;
            }
        }
    }
    public int FindNodeIndex(int key) // finds node position in the array based on its key
    {
        for (int i = 0; i < graph.nodes.Length;i++)
        {
            if (graph.nodes[i].key == key)
            {
                return i;
            }
        }
        return -1;
    }
    public void CheckIfEdgesAreValid(int nodeKey)
    {
        //Debug.LogError(nodeKey); // node key
        string[] splitText = new string[100];
        currentNodeIndex = FindNodeIndex(nodeKey); // currentNodeIndex - position in the array
        //Debug.LogError("node index "+currentNodeIndex);
        splitText = graph.nodes[currentNodeIndex].attributes.characterDialogue.Split(' ');
        List<string> keywords = new List<string>();
        List<string> edgeLabels = new List<string>();

        // clean up hyperlink keywords
        for (int i = 0; i < splitText.Length;i++)
        {
            if (splitText[i].StartsWith('<'))
            {
                //Debug.Log("found link - " + splitText[i]);
                string keyword = splitText[i].Replace("<style=\"Link\">", "");
                keyword.Replace("</style>", "");
                keywords.Add(keyword);
            }
            else if (splitText[i].EndsWith('>') && splitText[i-1].StartsWith('<')) // in case of two-word keywords (if 3 or more - i guess we will die)
            {
                string previousKeyword = splitText[i - 1].Replace("<style=\"Link\">", "");
                string keyword = previousKeyword + " " + splitText[i].Replace("</style>", "");
                keywords.Remove(previousKeyword);
                keywords.Add(keyword);
            }
        }

        // count edges for the node
        int edgesCount = 0;
        for (int i = 0; i < graph.edges.Length; i++)
        {
            if (graph.edges[i].source == nodeKey)
            {
                edgeLabels.Add(graph.edges[i].attributes.label);
                edgesCount++;
            }
        }


        if (edgesCount > keywords.Count) // if not enough hyperlinks
        {
            Debug.Log("Case 1 - More edges than hyperlinks, "+ edgesCount +" "+ keywords.Count);
            string newText = graph.nodes[currentNodeIndex].attributes.characterDialogue;
            for (int i = keywords.Count; i < edgesCount; i++) // adds more hyperlinks
            {
                 newText += " <style=\"Link\">" + edgeLabels[i] + "</style>";
            }
            //Debug.Log(newText);
            graph.nodes[currentNodeIndex].attributes.characterDialogue = newText;
           // Debug.Log(graph.nodes[currentNodeIndex].attributes.characterDialogue);
        }
        else if (edgesCount < keywords.Count) // if not enough edges
        {
            Debug.Log("Case 2 - More hyperlinks than edges" + edgesCount + " " + keywords.Count);
            string newText1 = null;

            for (int i = 0; i < splitText.Length; i++)
            {
                if (splitText[i].StartsWith('<'))
                {
                    string keyword2 = splitText[i].Replace("<style=\"Link\">", "");
                    keyword2.Replace("</style>", "");
                    for (int z = edgesCount; z < keywords.Count; z++) // changes the stylization of the keyword, for now its black 
                    {
                        if (keywords[z] == keyword2)
                        {
                            keyword2 = "<u><link><color=\"black\">" + keyword2 + "</u></link></color>";
                            splitText[i] = keyword2;
                        }
                    }
                }
                newText1 += splitText[i] + " ";
            }
            graph.nodes[currentNodeIndex].attributes.characterDialogue = newText1;
        }
        else
        {
            Debug.Log("Edges and hyperlinks match :)");
        }
    }

}

// node graph classes based on the json data groups vvv

[Serializable]
public class NodeGraph
{
    public Nodes[] nodes;
    public Edges[] edges;

    public static NodeGraph CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<NodeGraph>(jsonString);
    }
}
[Serializable]
public class Nodes
{
    public int key;
    public NodeAttributes attributes;
}
[Serializable]
public class NodeAttributes
{
    public string label;
    public double x;
    public double y;
    public string characterDialogue;
    public string Type;
}
[Serializable]
public class Edges
{
    public int key;
    public int source;
    public int target;
    public EdgeAttributes attributes;
}
[Serializable]
public class EdgeAttributes
{
    public string label;
}
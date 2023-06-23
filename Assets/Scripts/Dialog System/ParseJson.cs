using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;


/// <summary>
/// parses the json file and puts all the data into the Nodegraph variable :)
/// </summary>
public class ParseJson : MonoBehaviour
{
    public NodeGraph graph;
    public TextAsset jsonFile;



    void Awake()
    {
        graph = NodeGraph.CreateFromJSON(jsonFile.text);
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
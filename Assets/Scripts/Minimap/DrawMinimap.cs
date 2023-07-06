using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class DrawMinimap : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject dotPrefab;
    public GameObject labelPrefab;

    public GameManager gameManager;

    [Header("Map Adjustables")]
    public float mapScale = 1f;
    public float lineWidth;
    public float nodeSize;
    [SerializeField] private bool spawn_labels = true;

    
    [Header("Colors - Nodes")]
    [Tooltip("Unused nodes, nodes you havent traversed yet, etc.")]
    public Color defaultColorNodes = Color.grey;

    [SerializeField] bool show_about_to_pass_nodes = true;
    [SerializeField] bool mark_about_to_pass_nodes_as_traversed = false;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color aboutToPassColorNodes = Color.red;

    [SerializeField] bool show_active_node = true;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color activeColorNodes = Color.red;

    [SerializeField] bool show_just_passed_nodes = true;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color justPassedColorNodes = Color.red;

    [SerializeField] bool show_traversed_nodes = true;
    [Tooltip("Nodes you have traversed already")]
    public Color traversedColorNodes = Color.black;

    [Header("Colors - Edges")]

    [Tooltip("Unused nodes, nodes you havent traversed yet, etc.")]
    public Color defaultColorEdges = Color.grey;

    [SerializeField] bool show_about_to_pass_edges = true;
    [SerializeField] bool mark_about_to_pass_edges_as_traversed = false;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color aboutToPassColorEdges = Color.red;

    [SerializeField] bool show_just_passed_edges = true;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color justPassedColorEdges = Color.red;

    [SerializeField] bool show_traversed_edges = true;
    [Tooltip("Nodes you have traversed already")]
    public Color traversedColorEdges = Color.black;

    private GameObject lineRef;
    private GameObject dotRef;
    LineRenderer lineRenderer;

    private Dictionary<int, GameObject> lineDict = new();
    private Dictionary<int, GameObject> dotDict = new();

    private Dictionary<int, Vector2> nodePositions = new();
    private Dictionary<int, GameObject> traversedLineDict = new();
    private Dictionary<int, GameObject> traversedNodeDict = new();

    /* To do:
     * - edit scale of the map
     * - line / dot highlights when hovered (probably not gonna be needed, talk to design first pls)
     */


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawFullMinimap();
    }

    public GameObject DrawMinimapLine(Vector2 startPos, Vector2 endPos)
    {
        startPos = new Vector2(startPos.x*mapScale, startPos.y*mapScale);
        endPos = new Vector2 (endPos.x*mapScale, endPos.y*mapScale);   

        // spawn line prefab
        lineRef = Instantiate(linePrefab, gameObject.transform);
        lineRenderer = lineRef.GetComponent<LineRenderer>();

        // draw line
        lineRenderer.startColor = defaultColorEdges;
        lineRenderer.endColor = defaultColorEdges;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        lineRenderer.useWorldSpace = false;

        return lineRef;
    }
    public GameObject DrawMinimapDot(Vector2 pos)
    {
        pos = new Vector2(pos.x*mapScale, pos.y*mapScale);

        dotRef = Instantiate(dotPrefab, gameObject.transform);
        dotRef.transform.localPosition = pos;
        dotRef.GetComponent<Image>().color = defaultColorNodes;

        return dotRef;
    }
    public void DrawFullMinimap()
    {

        if (gameManager != null)
        {

            Nodes lastNode = gameManager.graph.nodes[gameManager.graph.nodes.Length-1];

            // draw dots
            for (int i = 0; i < gameManager.graph.nodes.Length; i++)
            {
                Vector2 pos = new Vector2((float)gameManager.graph.nodes[i].attributes.x, (float)gameManager.graph.nodes[i].attributes.y);
                nodePositions[gameManager.graph.nodes[i].key] = pos;
                dotDict[gameManager.graph.nodes[i].key] = DrawMinimapDot(pos);
            }
            // draw lines
            for (int i = 0; i < gameManager.graph.edges.Length; i++)
            {
                lineDict[gameManager.graph.edges[i].key] = DrawMinimapLine(nodePositions[gameManager.graph.edges[i].source], nodePositions[gameManager.graph.edges[i].target]);
                //Debug.LogError(parseJsonRef.graph.edges[i].key);
            }
        }
    }
    public void RecolorMinimap()
    {
        // nodes
        for (int i = 0; i < dotDict.Count; i++)
        {
            int key = gameManager.graph.nodes[i].key;
            if (key == gameManager.previousNodeKey)
            {
                if (show_just_passed_nodes) ColourDot(dotDict[key].GetComponent<Image>(), justPassedColorNodes);    //Just Passed Nodes
                traversedNodeDict[key] = dotDict[key];
            }
            else if (key == gameManager.currentNodeKey)
            {
                if (show_active_node) ColourDot(dotDict[key].GetComponent<Image>(), activeColorNodes);              //Active Color Nodes
                if (dotDict[key].GetComponentInChildren<MinimapLabel>() == null && spawn_labels)                    //Spawning Label
                    SpawnLabel(dotDict[key], key);
            }
            else if (show_about_to_pass_nodes && traversedNodeDict.ContainsKey(key)) ColourDot(dotDict[key].GetComponent<Image>(), traversedColorNodes);
           
            else ColourDot(dotDict[key].GetComponent<Image>(), defaultColorNodes);
        }

        //edges 
        GameObject[] activeLineArray = new GameObject[gameManager.graph.edges.Length + 1];
        for (int j = 0; j < gameManager.graph.edges.Length; j++)
        {
            // traversed nodes
            if (gameManager.graph.edges[j].source == gameManager.previousNodeKey && gameManager.graph.edges[j].target == gameManager.currentNodeKey)
            {
                if (show_just_passed_edges) ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), justPassedColorEdges);
                traversedLineDict[gameManager.graph.edges[j].key] = lineDict[gameManager.graph.edges[j].key];
            }
            // upcoming / active nodes
            else if (gameManager.graph.edges[j].source == gameManager.currentNodeKey)
            {
                if (show_about_to_pass_edges) 
                { 
                    if (show_about_to_pass_edges) ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), aboutToPassColorEdges);
                    if (mark_about_to_pass_edges_as_traversed) traversedLineDict[gameManager.graph.edges[j].key] = lineDict[gameManager.graph.edges[j].key];
                }
                if (show_about_to_pass_nodes)
                {
                    for (int k = 0; k < dotDict.Count; k++)
                    {
                        int key = gameManager.graph.nodes[k].key;
                        if (key == gameManager.graph.edges[j].target)
                        {
                            ColourDot(dotDict[gameManager.graph.nodes[k].key].GetComponent<Image>(), aboutToPassColorNodes);            //About to Pass Nodes
                            if (mark_about_to_pass_nodes_as_traversed) traversedNodeDict[key] = dotDict[key];
                        }
                    }
                }
                activeLineArray[j] = lineDict[gameManager.graph.edges[j].key];
                //Debug.Log("upcoming line - " + parseJsonRef.graph.edges[j].key);
            } // previously coloured lines

            else if (lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>().startColor != justPassedColorEdges)
            {
                //if (lineDict[gameManager.graph.edges[j].key] == activeLineArray[j])
                //{
                //    ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), activeColor);
                //}
                if (traversedLineDict.ContainsKey(gameManager.graph.edges[j].key))
                {
                    if (show_traversed_edges) ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), traversedColorEdges);
                }
                else
                {
                    ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), defaultColorEdges);
                }
            }
            //Debug.LogError(j + " "+parseJsonRef.graph.edges[j].key + " " + parseJsonRef.graph.edges[j].attributes.label);


        }
    }
    public void ColourLine(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        //Debug.Log("Line coloured, game object - " +  lineRenderer.gameObject.name);
    }
    public void ColourDot(Image image, Color color)
    {
        image.color = color;
    }

    public void SpawnLabel(GameObject dot, int index) // activates label prefab and adds text to it
    {
        GameObject label = Instantiate(labelPrefab, dot.transform);

        TextMeshProUGUI textmesh = label.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < gameManager.graph.nodes.Length; i++)
        {
            if (gameManager.graph.nodes[i].key == index)
            {
                textmesh.text = gameManager.graph.nodes[i].attributes.label;
                break;
            }
        }
    }
    //-----------------------------------------------------

}

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
    public float mapScale = 1f;
    [Tooltip("Unused nodes, nodes you havent traversed yet, etc.")]
    public Color defaultColor = Color.grey;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color activeColor = Color.red;
    [Tooltip("Nodes you have traversed already")]
    public Color traversedColor = Color.black;

    public float lineWidth;
    public float nodeSize;

    public GameObject linePrefab;
    public GameObject dotPrefab;
    public GameObject labelPrefab;

    public ParseJson parseJsonRef;

    private GameObject lineRef;
    private GameObject dotRef;
    LineRenderer lineRenderer;

    private Dictionary<int, GameObject> lineDict = new();
    private Dictionary<int, GameObject> dotDict = new();

    private Dictionary<int, Vector2> nodePositions = new();
    private Dictionary<int, GameObject> traversedLineDict = new();

    /* To do:
     * - edit scale of the map
     * - line / dot highlights when hovered
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
        lineRenderer.startColor = defaultColor;
        lineRenderer.endColor = defaultColor;
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
        dotRef.GetComponent<Image>().color = defaultColor;

        return dotRef;
    }
    public void DrawFullMinimap()
    {

        if (parseJsonRef != null)
        {

            Nodes lastNode = parseJsonRef.graph.nodes[parseJsonRef.graph.nodes.Length-1];

            // draw dots
            for (int i = 0; i < parseJsonRef.graph.nodes.Length; i++)
            {
                Vector2 pos = new Vector2((float)parseJsonRef.graph.nodes[i].attributes.x, (float)parseJsonRef.graph.nodes[i].attributes.y);
                nodePositions[parseJsonRef.graph.nodes[i].key] = pos;
                dotDict[parseJsonRef.graph.nodes[i].key] = DrawMinimapDot(pos);
            }
            // draw lines
            for (int i = 0; i < parseJsonRef.graph.edges.Length; i++)
            {
                lineDict[parseJsonRef.graph.edges[i].key] = DrawMinimapLine(nodePositions[parseJsonRef.graph.edges[i].source], nodePositions[parseJsonRef.graph.edges[i].target]);
                //Debug.LogError(parseJsonRef.graph.edges[i].key);
            }
        }
    }
    public void RecolorMinimap()
    {
        // nodes
        for (int i = 0; i < dotDict.Count; i++)
        {
            if (parseJsonRef.graph.nodes[i].key == parseJsonRef.previousNodeKey)
            {
                ColourDot(dotDict[parseJsonRef.graph.nodes[i].key].GetComponent<Image>(), traversedColor);
            }
            else if (parseJsonRef.graph.nodes[i].key == parseJsonRef.currentNodeKey)
            {
                ColourDot(dotDict[parseJsonRef.graph.nodes[i].key].GetComponent<Image>(), activeColor);
                if (dotDict[parseJsonRef.graph.nodes[i].key].GetComponentInChildren<MinimapLabel>() == null)
                    SpawnLabel(dotDict[parseJsonRef.graph.nodes[i].key], parseJsonRef.graph.nodes[i].key);
            }
        }

        //edges 
        GameObject[] activeLineArray = new GameObject[parseJsonRef.graph.edges.Length + 1];
        for (int j = 0; j < parseJsonRef.graph.edges.Length; j++)
        {
            // traversed nodes
            if (parseJsonRef.graph.edges[j].source == parseJsonRef.previousNodeKey && parseJsonRef.graph.edges[j].target == parseJsonRef.currentNodeKey)
            {
                ColourLine(lineDict[parseJsonRef.graph.edges[j].key].GetComponent<LineRenderer>(), traversedColor);
                traversedLineDict[parseJsonRef.graph.edges[j].key] = lineDict[parseJsonRef.graph.edges[j].key];
            }
            // upcoming / active nodes
            else if (parseJsonRef.graph.edges[j].source == parseJsonRef.currentNodeKey)
            {
                ColourLine(lineDict[parseJsonRef.graph.edges[j].key].GetComponent<LineRenderer>(), activeColor);
                activeLineArray[j] = lineDict[parseJsonRef.graph.edges[j].key];
                //Debug.Log("upcoming line - " + parseJsonRef.graph.edges[j].key);
            } // previously coloured lines

            if (lineDict[parseJsonRef.graph.edges[j].key].GetComponent<LineRenderer>().startColor != traversedColor)
            {
                if (lineDict[parseJsonRef.graph.edges[j].key] == activeLineArray[j])
                {
                    ColourLine(lineDict[parseJsonRef.graph.edges[j].key].GetComponent<LineRenderer>(), activeColor);
                }
                else if (traversedLineDict.ContainsKey(parseJsonRef.graph.edges[j].key))
                {
                    ColourLine(lineDict[parseJsonRef.graph.edges[j].key].GetComponent<LineRenderer>(), traversedColor);
                }
                else
                {
                    ColourLine(lineDict[parseJsonRef.graph.edges[j].key].GetComponent<LineRenderer>(), defaultColor);
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
        for (int i = 0; i < parseJsonRef.graph.nodes.Length; i++)
        {
            if (parseJsonRef.graph.nodes[i].key == index)
            {
                textmesh.text = parseJsonRef.graph.nodes[i].attributes.label;
                break;
            }
        }
    }
    //-----------------------------------------------------

}

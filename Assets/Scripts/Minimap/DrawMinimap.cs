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

    private Vector2[] nodePositions;

    private GameObject[] lineArray;
    private GameObject[] dotArray;

    private GameObject[] traversedLineArray;

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

        return dotRef;
    }
    public void DrawFullMinimap()
    {

        if (parseJsonRef != null)
        {
            lineArray = new GameObject[parseJsonRef.graph.edges.Length+2];
            dotArray = new GameObject[parseJsonRef.graph.nodes.Length + 2];
            traversedLineArray = new GameObject[lineArray.Length];

            Nodes lastNode = parseJsonRef.graph.nodes[parseJsonRef.graph.nodes.Length-1];
            nodePositions = new Vector2[lastNode.key+2];

            // draw dots
            for (int i = 0; i < parseJsonRef.graph.nodes.Length; i++)
            {
                Vector2 pos = new Vector2((float)parseJsonRef.graph.nodes[i].attributes.x, (float)parseJsonRef.graph.nodes[i].attributes.y);
                nodePositions[parseJsonRef.graph.nodes[i].key] = pos;
                dotArray[i] = DrawMinimapDot(pos);
            }
            // draw lines
            for (int i = 0; i < parseJsonRef.graph.edges.Length; i++)
            {
                lineArray[i]=DrawMinimapLine(nodePositions[parseJsonRef.graph.edges[i].source], nodePositions[parseJsonRef.graph.edges[i].target]);
            }
        }
    }
    public void RecolorMinimap()
    {
        // nodes
        for (int i = 0; i < dotArray.Length; i++)
        {
            if (i == parseJsonRef.previousNodeID)
            {
                ColourDot(dotArray[i].GetComponent<Image>(), traversedColor);
            }
            else if (i == parseJsonRef.nodeID)
            {
                ColourDot(dotArray[i].GetComponent<Image>(), activeColor);
                if (dotArray[i].GetComponentInChildren<MinimapLabel>() == null)
                    SpawnLabel(dotArray[i], i);
            }
        }

        //edges 
        GameObject[] activeLineArray = new GameObject[lineArray.Length];
        for (int j = 0; j < parseJsonRef.graph.edges.Length; j++)
        {
            // traversed edges
            if (parseJsonRef.graph.edges[j].source == parseJsonRef.previousNodeID && parseJsonRef.graph.edges[j].target == parseJsonRef.nodeID)
            {
                ColourLine(lineArray[j].GetComponent<LineRenderer>(), traversedColor);
                traversedLineArray[j] = lineArray[j];
            }
            // upcoming edges
            else if (parseJsonRef.graph.edges[j].source == parseJsonRef.nodeID)
            {
                ColourLine(lineArray[j].GetComponent<LineRenderer>(), activeColor);
                activeLineArray[j] = lineArray[j];
            }
            // clearing the previous mistakes
            if (lineArray[j].GetComponent<LineRenderer>().startColor != traversedColor && lineArray[j] != traversedLineArray[j] && lineArray[j] != activeLineArray[j])
            {
                ColourLine(lineArray[j].GetComponent<LineRenderer>(), defaultColor);
            }
            else if (lineArray[j] == activeLineArray[j])
            {
                ColourLine(lineArray[j].GetComponent<LineRenderer>(), activeColor);
            }
            else
            {
                ColourLine(lineArray[j].GetComponent<LineRenderer>(), traversedColor);
            }
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

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

    public GameManager gameManager;

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
            if (gameManager.graph.nodes[i].key == gameManager.previousNodeKey)
            {
                ColourDot(dotDict[gameManager.graph.nodes[i].key].GetComponent<Image>(), traversedColor);
            }
            else if (gameManager.graph.nodes[i].key == gameManager.currentNodeKey)
            {
                ColourDot(dotDict[gameManager.graph.nodes[i].key].GetComponent<Image>(), activeColor);
                if (dotDict[gameManager.graph.nodes[i].key].GetComponentInChildren<MinimapLabel>() == null)
                    SpawnLabel(dotDict[gameManager.graph.nodes[i].key], gameManager.graph.nodes[i].key);
            }
        }

        //edges 
        GameObject[] activeLineArray = new GameObject[gameManager.graph.edges.Length + 1];
        for (int j = 0; j < gameManager.graph.edges.Length; j++)
        {
            // traversed nodes
            if (gameManager.graph.edges[j].source == gameManager.previousNodeKey && gameManager.graph.edges[j].target == gameManager.currentNodeKey)
            {
                ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), traversedColor);
                traversedLineDict[gameManager.graph.edges[j].key] = lineDict[gameManager.graph.edges[j].key];
            }
            // upcoming / active nodes
            else if (gameManager.graph.edges[j].source == gameManager.currentNodeKey)
            {
                ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), activeColor);
                activeLineArray[j] = lineDict[gameManager.graph.edges[j].key];
                //Debug.Log("upcoming line - " + parseJsonRef.graph.edges[j].key);
            } // previously coloured lines

            if (lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>().startColor != traversedColor)
            {
                if (lineDict[gameManager.graph.edges[j].key] == activeLineArray[j])
                {
                    ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), activeColor);
                }
                else if (traversedLineDict.ContainsKey(gameManager.graph.edges[j].key))
                {
                    ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), traversedColor);
                }
                else
                {
                    ColourLine(lineDict[gameManager.graph.edges[j].key].GetComponent<LineRenderer>(), defaultColor);
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

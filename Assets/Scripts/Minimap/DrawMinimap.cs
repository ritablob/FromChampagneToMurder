using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TerrainUtils;
using UnityEngine.UI;

public class DrawMinimap : MonoBehaviour
{
    public GameObject linePrefab;
    public GameObject dotPrefab;
    public GameObject labelPrefab;

    public GameObject Dots;
    public GameObject Lines;
    public GameObject Labels;

    public GameManager gameManager;
    public CameraMovingManager cameraMovingManager;

    [Header("Map Adjustables")]
    public float mapScale = 1f;
    public float lineWidth = 5f;
    [Tooltip("default is 1")]
    public float nodeSize = 1f;
    [SerializeField] private bool spawn_all_labels_at_start = false;
    [SerializeField] private bool spawn_labels = true;
    [SerializeField] private bool render_lines_on_top_of_labels = true;
    [SerializeField] private Vector2 ShadowOffset;
    [SerializeField] bool track_nodes_with_camera;


    [Header("Random Values")]
    [SerializeField][Tooltip("In what range the Nodes will randomly rotate in, e.g. from x = -10 to y = 15. The value will be added to the rotation, - is left")] 
    private Vector2 Random_rotation_range_nodes = new Vector2(-45f,12.5f);
    [SerializeField][Tooltip("In what range the Nodes will Scale, from x to y, the value will be added to the scale")] 
    private Vector2 Random_scale_range_nodes = new Vector2(-0.2f, 0.2f);
    [SerializeField]
    [Tooltip("In what range the Labels will randomly rotate in, e.g. from x = -10 to y = 15. The value will be added to the rotation, - is left")]
    private Vector2 Random_rotation_range_labels = new Vector2(12.5f, 12.5f);
    [SerializeField][Tooltip("In what range the Labels will randomly Scale, from x to y, the value will be added to the scale")] 
    private Vector2 Random_scale_range_labels = new Vector2(-0.1f, 0.1f);

    [Header("Colors - Nodes")]
    [Tooltip("Unused nodes, nodes you havent traversed yet, etc.")]
    public Color defaultColorNodes = Color.grey;
    public Color LineColorShadow = Color.Lerp(Color.clear,Color.black,0.5f);

    [SerializeField] bool show_about_to_pass_nodes = true;
    [SerializeField] bool mark_about_to_pass_nodes_as_traversed = true;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color aboutToPassColorNodes = Color.cyan;

    [SerializeField] bool show_active_node = true;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color activeColorNodes = Color.red;

    [SerializeField] bool show_just_passed_nodes = true;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color justPassedColorNodes = Color.magenta;

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
    public Color justPassedColorEdges = Color.magenta;

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

    public List<Vector2> NodesToTrack = new List<Vector2>();
    public Vector2 NodeToCentreOnTRack;

    private Dictionary<int, GameObject> labelDict = new();


    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawFullMinimap();
        SpawnLabel(dotDict[gameManager.graph.nodes[0].key], gameManager.graph.nodes[0].key);
        ColourDot(dotDict[gameManager.graph.nodes[0].key].GetComponent<Image>(), activeColorNodes);
    }

    public GameObject DrawMinimapLine(Vector2 startPos, Vector2 endPos)
    {
        startPos = new Vector2(startPos.x*mapScale, startPos.y*mapScale);
        endPos = new Vector2 (endPos.x*mapScale, endPos.y*mapScale);   

        // spawn line prefab
        lineRef = Instantiate(linePrefab, Lines.transform);
        lineRenderer = lineRef.GetComponent<LineRenderer>();

        // draw line
        lineRenderer.startColor = defaultColorEdges;
        lineRenderer.endColor = defaultColorEdges;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        lineRenderer.useWorldSpace = false;
        if (render_lines_on_top_of_labels) lineRenderer.sortingOrder = 6;

        //Shadow
        GameObject lineRefShadow = Instantiate(linePrefab, lineRef.transform);
        LineRenderer lineRendererShadow = lineRefShadow.GetComponent<LineRenderer>();

        lineRendererShadow.startColor = LineColorShadow;
        lineRendererShadow.endColor = LineColorShadow;
        lineRendererShadow.startWidth = lineWidth;
        lineRendererShadow.endWidth = lineWidth;
        lineRendererShadow.SetPosition(0, startPos + ShadowOffset);
        lineRendererShadow.SetPosition(1, endPos + ShadowOffset);

        lineRendererShadow.useWorldSpace = false;
        if (render_lines_on_top_of_labels) lineRendererShadow.sortingOrder = 5;

        //Copied from ColourLine
        if (defaultColorEdges.a == 0)
        {
            lineRendererShadow.startColor = Color.clear;
            lineRendererShadow.endColor = Color.clear;
        }
        else
        {
            lineRendererShadow.startColor = LineColorShadow;
            lineRendererShadow.endColor = LineColorShadow;
        }

        return lineRef;
    }
    public GameObject DrawMinimapDot(Vector2 pos)
    {
        pos = new Vector2(pos.x*mapScale, pos.y*mapScale);

        dotRef = Instantiate(dotPrefab, Dots.transform);
        dotRef.transform.localPosition = pos;
        dotRef.GetComponent<Image>().color = defaultColorNodes;

        //RandomRotation
        RectTransform dotRefRect = dotRef.GetComponent<RectTransform>();
        float random_rotation = UnityEngine.Random.Range(Random_rotation_range_nodes.x, Random_rotation_range_nodes.y);
        dotRef.transform.Rotate(0,0,-random_rotation);
        //RandomScale
        float random_scale = UnityEngine.Random.Range(Random_scale_range_nodes.x, Random_scale_range_nodes.y);
        dotRefRect.localScale += new Vector3(random_scale * nodeSize, random_scale * nodeSize, random_scale * nodeSize);

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
                if (i == 0)
                {
                    gameManager.currentNodeKey = gameManager.graph.nodes[i].key;
                }
                Vector2 pos = new Vector2((float)gameManager.graph.nodes[i].attributes.x, (float)gameManager.graph.nodes[i].attributes.y);
                int key = gameManager.graph.nodes[i].key;
                nodePositions[key] = pos;
                dotDict[gameManager.graph.nodes[i].key] = DrawMinimapDot(pos);
                if (spawn_all_labels_at_start) SpawnLabel(dotDict[key], key);

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
                if (show_just_passed_nodes) ColourDot(dotDict[key].GetComponent<Image>(), justPassedColorNodes);                  //Just Passed Nodes
                traversedNodeDict[key] = dotDict[key];
            }
            else if (key == gameManager.currentNodeKey)
            {
                Debug.Log("Active node key - " + key);
                if (show_active_node)
                {//Active Color Nodes
                    ColourDot(dotDict[key].GetComponent<Image>(), activeColorNodes, dotDict[key]);
                    NodeToCentreOnTRack = new Vector2(dotDict[key].transform.localPosition.x, dotDict[key].transform.position.y);
                }              
                if (dotDict[key].GetComponentInChildren<MinimapLabel>() == null && spawn_labels)                                  //Spawning Label
                    if (!spawn_all_labels_at_start) SpawnLabel(dotDict[key], key);
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
                            ColourDot(dotDict[gameManager.graph.nodes[k].key].GetComponent<Image>(), aboutToPassColorNodes, dotDict[gameManager.graph.nodes[k].key]);            //About to Pass Nodes
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

        //Camera
        if (track_nodes_with_camera)
        {
            cameraMovingManager.SetNewTargetForCamera(NodesToTrack, NodeToCentreOnTRack);
            NodesToTrack.Clear();
            NodeToCentreOnTRack = Vector2.zero;
        }

    }
    public void ColourLine(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        LineRenderer shadowLineRenderer = lineRenderer.gameObject.GetComponentInChildren<LineRenderer>();
        if (color.a == 0)
        {
            shadowLineRenderer.startColor = Color.clear;
            shadowLineRenderer.endColor = Color.clear;
        }
        else
        {
            shadowLineRenderer.startColor = LineColorShadow;
            shadowLineRenderer.endColor = LineColorShadow;
        }
        //Debug.Log("Line coloured, game object - " +  lineRenderer.gameObject.name);
    }
    public void ColourDot(Image image, Color color, GameObject node_if_tracked = null)
    {
        image.color = color;
        if (track_nodes_with_camera && node_if_tracked != null)
        {
            NodesToTrack.Add(new Vector2(node_if_tracked.transform.localPosition.x, node_if_tracked.transform.localPosition.y));
            //Debug.Log(node_if_tracked.transform.localPosition.x);
        }

    }

    public void SpawnLabel(GameObject dot, int index) // activates label prefab and adds text to it
    {
        if (!labelDict.TryGetValue(index, out GameObject label1)) // check if label already exists
        {
            GameObject label = Instantiate(labelPrefab, dot.transform.position, Quaternion.identity, Labels.transform);

            TextMeshProUGUI textmesh = label.GetComponentInChildren<TextMeshProUGUI>();
            for (int i = 0; i < gameManager.graph.nodes.Length; i++)
            {
                if (gameManager.graph.nodes[i].key == index)
                {
                    textmesh.text = gameManager.graph.nodes[i].attributes.label;
                    //Random Rotation
                    float random_rotation = UnityEngine.Random.Range(Random_rotation_range_labels.x, Random_rotation_range_labels.y);
                    label.transform.Rotate(0, 0, 0);
                    //Random Scale
                    float random_scale = UnityEngine.Random.Range(Random_scale_range_nodes.x, Random_scale_range_nodes.y);
                    label.transform.localScale += new Vector3(random_scale, random_scale, random_scale);
                    labelDict[index] = label;
                    break;
                }
            }
        }
    }
    //-----------------------------------------------------

}

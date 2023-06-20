using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawMinimap : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float mapScale = 1f;
    [Tooltip("Unused nodes, nodes you havent traversed yet, etc.")]
    public Color defaultLineColor = Color.grey;
    [Tooltip("Nodes that you are about to traverse + currently are standing on")]
    public Color activeLineColor = Color.red;
    [Tooltip("Nodes you have traversed already")]
    public Color traversedLineColor = Color.black;
    public Color hoverLineColor = Color.blue;

    public float lineWidth;
    public Color defaultNodeColor;
    public float nodeSize;

    public GameObject linePrefab;
    public GameObject dotPrefab;

    public ParseJson parseJsonRef;

    private GameObject lineRef;
    private GameObject dotRef;
    LineRenderer lineRenderer;

    private Vector2[] nodePositions;

    private GameObject[] lineArray;
    private GameObject[] dotArray;

    /* To do:
     * - edit scale of the map
     * - line / dot highlights when hovered
     */
    public GameObject DrawMinimapLine(Vector2 startPos, Vector2 endPos)
    {
        startPos = new Vector2(startPos.x*mapScale, startPos.y*mapScale);
        endPos = new Vector2 (endPos.x*mapScale, endPos.y*mapScale);   

        // spawn line prefab
        lineRef = Instantiate(linePrefab, gameObject.transform);
        lineRenderer = lineRef.GetComponent<LineRenderer>();

        // draw line
        lineRenderer.startColor = defaultLineColor;
        lineRenderer.endColor = defaultLineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, endPos);

        lineRenderer.useWorldSpace = false;

        /* ----------- FIX THIS, GIVES ERROR: 
         * Converting invalid MinMaxAABB
         * UnityEngine.LineRenderer:BakeMesh (UnityEngine.Mesh,bool)


        Mesh lineBakedMesh = new Mesh();
        lineRenderer.BakeMesh(lineBakedMesh, true);
        lineRef.GetComponent<MeshCollider>().sharedMesh = lineBakedMesh;
        lineRef.GetComponent<MeshCollider>().convex = true;
        */

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
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawFullMinimap();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("hover");
        if (eventData.pointerEnter.GetComponent<MeshCollider>() != null)
        {
            LineRenderer renderer = eventData.pointerEnter.GetComponent<LineRenderer>();
            renderer.startColor = hoverLineColor;
            renderer.endColor = hoverLineColor;
        }
        else if (eventData.pointerEnter.GetComponent<Image>() != null)
        {
            Image image = eventData.pointerEnter.GetComponent<Image>();
            image.color = hoverLineColor;
        }

        //throw new NotImplementedException();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }
}

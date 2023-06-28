using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableElement : MonoBehaviour
{
    BoxCollider2D boxCollider;
    RectTransform rect;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rect = GetComponent<RectTransform>();
        // scales the box collider to object size
        boxCollider.size = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseDrag()
    {
        Debug.Log("drag");
        Vector2 mousePosition = Input.mousePosition;
        transform.position = mousePosition;
    }
}

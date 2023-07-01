using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MovableImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;
    public float zoomSpeed = 1f;

    private GameObject draggingObject;
    private RectTransform draggingPlane;

    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
        {
            Debug.Log("OnBeginDrag: couldnt find canvas");
            return;
        }


        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        draggingObject = gameObject;

        if (dragOnSurfaces)
            draggingPlane = transform as RectTransform;
        else
            draggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }
    private void OnMouseOver()
    {

        Debug.Log("is on image");
        OnScroll();
    }
    public void OnDrag(PointerEventData data)
    {
        if (draggingObject != null)
            SetDraggedPosition(data);
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            draggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = draggingObject.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = draggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       // if (m_DraggingIcon != null)
          //  Destroy(m_DraggingIcon);
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
    public void OnScroll()
    {
        float scrollValue = Input.mouseScrollDelta.y;
        var rt = GetComponent<RectTransform>();

        if (rt.localScale.x > 0.2f && scrollValue < 0)
        {
            Vector2 scale = new(rt.localScale.x + (scrollValue * zoomSpeed), rt.localScale.y + (scrollValue * zoomSpeed));
            rt.localScale = scale;
        }
        else if (rt.localScale.x < 30f && scrollValue > 0)
        {
            Vector2 scale = new(rt.localScale.x + (scrollValue * zoomSpeed), rt.localScale.y + (scrollValue * zoomSpeed));
            rt.localScale = scale;
        }
    }
}

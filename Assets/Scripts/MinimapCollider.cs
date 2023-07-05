using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MinimapCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hoveringOverMinimap = false;
    public InputManager inputManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.LogWarning("has entered");
        if (eventData.pointerEnter == gameObject)
        {
            hoveringOverMinimap = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!inputManager.isDragging)
        {
            hoveringOverMinimap = false;
        }
    }
}

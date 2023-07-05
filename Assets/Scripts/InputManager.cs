using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public CameraMovingManager cameraManager;
    public MinimapCollider minimapCollider;

    public bool isDragging;

    void LateUpdate()
    {

        CheckMinimapInput();
    }
    void CheckMinimapInput()
    {
        if (minimapCollider.hoveringOverMinimap)
        {
            if (Input.GetMouseButton(0)) // hovering over image, pressing left mouse button
            {
                if (!isDragging)
                {
                    isDragging = true;
                    cameraManager.SetMouseOrigin();
                }
                cameraManager.DragCamera();
            }
            /*else if (Input.GetMouseButtonUp(0)) // if the hovering boolean is still active, but mouse button has been released
                                                  // Bad: doesnt allow players to release button while still hovering the image
            {
                minimapCollider.hoveringOverMinimap = false;
                isDragging = false;
            }*/
            else // hovering over image, not pressing left button
            {
                isDragging = false;
            }

            if (Input.GetMouseButtonDown(1)) // hovering over image, pressing right mouse button
            {
                cameraManager.ResetCameraValues();
            }

            cameraManager.CameraZoom(); // you can zoom as long as you hover 
        }
    }
}

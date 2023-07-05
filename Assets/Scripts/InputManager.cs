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
            if (Input.GetMouseButton(0))
            {
                if (!isDragging)
                {
                    isDragging = true;
                    cameraManager.SetMouseOrigin();
                }
                cameraManager.DragCamera();
            }
            else
            {
                isDragging = false;
                minimapCollider.hoveringOverMinimap = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                cameraManager.ResetCameraValues();
            }

            cameraManager.CameraZoom();
        }
    }
}

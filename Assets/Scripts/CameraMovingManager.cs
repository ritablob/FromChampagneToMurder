using System.Security.Cryptography;
using UnityEngine;

public class CameraMovingManager : MonoBehaviour
{
    public Camera minimapCam;
    public float zoomSpeed = 1f;
    public float maxZoomIn = 10f;
    public float maxZoomOut = 1000f;
    public Transform lowerLeftCorner;
    public Transform upperRightCorner;

    public float dragSpeed = 1f;

    private Vector3 originalCameraPos;
    private Vector3 mouseOrigin;
    private Vector3 diference;
    private float currentOrtho;
    private float originalOrtho;
    private Vector3 cameraPosition;

    void Start()
    {
        minimapCam = GetComponent<Camera>();
        originalCameraPos = minimapCam.transform.position;
        currentOrtho = minimapCam.orthographicSize;
        originalOrtho = currentOrtho;
    }
    public void SetMouseOrigin()
    {
        mouseOrigin = minimapCam.ScreenToWorldPoint(Input.mousePosition);
        Debug.LogWarning("camera - " + minimapCam.transform.position + ", mouse origin - " + mouseOrigin);
    }
    public void DragCamera()
    {
        diference = minimapCam.ScreenToWorldPoint(Input.mousePosition) - minimapCam.transform.position;

        cameraPosition = mouseOrigin - diference;
        Debug.Log(cameraPosition.x + ", " + cameraPosition.y);

        // clamp values 
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, lowerLeftCorner.position.x, upperRightCorner.position.x);
        cameraPosition.y = Mathf.Clamp(cameraPosition.y, lowerLeftCorner.position.y, upperRightCorner.position.y);

        minimapCam.transform.position = cameraPosition;
    }
    public void ResetCameraValues()
    {
        minimapCam.transform.position = originalCameraPos;
        minimapCam.orthographicSize = originalOrtho;
        currentOrtho = originalOrtho;
    }
    public void CameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            if (currentOrtho < maxZoomOut)
            {
                if (currentOrtho > maxZoomIn)
                {
                    currentOrtho += (Input.mouseScrollDelta.y * zoomSpeed);
                    minimapCam.orthographicSize = currentOrtho;
                }
                else
                {
                    //Debug.LogWarning("zoom hit the zone");
                    currentOrtho += zoomSpeed;
                }
            }
            else
            {
               // Debug.LogWarning("zoom hit the zone");
                currentOrtho -= zoomSpeed;
            }
        }
    }
}

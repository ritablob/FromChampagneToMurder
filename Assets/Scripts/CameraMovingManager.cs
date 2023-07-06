using System.Security.Cryptography;
using UnityEngine;

public class CameraMovingManager : MonoBehaviour
{
    public Camera minimapCam;
    public Transform lowerLeftCorner;
    public Transform upperRightCorner;

    [Header("Adjustables")]
    public float dragSpeed = 1f;
    public float zoomSpeed = 50f;
    public float maxZoomIn = 150f;
    public float maxZoomOut = 500f;
    public bool inverted_mouse_wheel = true;

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
        Vector2 my_mouseScrollDelta = Input.mouseScrollDelta;
        if (inverted_mouse_wheel) my_mouseScrollDelta *= -1; //Inverts Data

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

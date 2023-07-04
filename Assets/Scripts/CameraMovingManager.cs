using System.Security.Cryptography;
using UnityEngine;

public class CameraMovingManager : MonoBehaviour
{
    public Camera minimapCam;
    public float zoomSpeed = 1f;
    public float maxZoomIn = 10f;
    public float maxZoomOut = 1000f;

    public float dragSpeed = 1f;

    private Vector3 originalCameraPos;
    private Vector3 mouseOrigin;
    private Vector3 diference;
    private bool dragging = false;
    private float currentOrtho;
    private float originalOrtho;

    void Start()
    {
        minimapCam = GetComponent<Camera>();
        originalCameraPos = minimapCam.transform.position;
        currentOrtho = minimapCam.orthographicSize;
        originalOrtho = currentOrtho;
    }
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            diference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - minimapCam.transform.position;
            if (dragging == false)
            {
                dragging = true;
                mouseOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
               // Debug.Log("camera - "+Camera.main.transform.position+", mouse origin - "+ mouseOrigin);
            }
            //Debug.Log("camera - " + Camera.main.transform.position + ", difference - " + diference);
        }
        else
        {
            dragging = false;
        }
        if (dragging == true)
        {
            minimapCam.transform.position = mouseOrigin - diference;
        }
        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1))
        {
            minimapCam.transform.position = originalCameraPos;
            minimapCam.orthographicSize = originalOrtho;
            currentOrtho = originalOrtho;
        }

        CameraZoom();
    }
    void CameraZoom()
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
                    Debug.LogWarning("zoom hit the zone");
                    currentOrtho += zoomSpeed;
                }
            }
            else
            {
                Debug.LogWarning("zoom hit the zone");
                currentOrtho -= zoomSpeed;
            }
        }
    }
}

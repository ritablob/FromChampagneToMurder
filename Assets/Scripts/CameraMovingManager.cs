using UnityEngine;

public class CameraMovingManager : MonoBehaviour
{
    public float zoomSpeed = 1f;
    private Vector3 originalCameraPos;
    private Vector3 mouseOrigin;
    private Vector3 diference;
    private bool dragging = false;
    private float currentOrtho;
    private float originalOrtho;
    void Start()
    {
        originalCameraPos = Camera.main.transform.position;
        currentOrtho = Camera.main.orthographicSize;
        originalOrtho = currentOrtho;
    }
    void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            diference = (Camera.main.ScreenToWorldPoint(Input.mousePosition)) - Camera.main.transform.position;
            if (dragging == false)
            {
                dragging = true;
                mouseOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            dragging = false;
        }
        if (dragging == true)
        {
            Camera.main.transform.position = mouseOrigin - diference;
        }
        //RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        if (Input.GetMouseButton(1))
        {
            Camera.main.transform.position = originalCameraPos;
            Camera.main.orthographicSize = originalOrtho;
            currentOrtho = originalOrtho;
        }

        CameraZoom();
    }
    void CameraZoom()
    {
        if (Input.mouseScrollDelta.y != 0)
        {
            currentOrtho += (Input.mouseScrollDelta.y*zoomSpeed);
            Camera.main.orthographicSize = currentOrtho;
        }
    }
}

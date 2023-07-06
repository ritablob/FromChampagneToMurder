using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;

public class CameraMovingManager : MonoBehaviour
{
    public Camera minimapCam;
    public RectTransform minimapCamRect;
    public Transform lowerLeftCorner;
    public Transform upperRightCorner;
    public RectTransform backgroundRef;

    [Header("Adjustables")]
    public float dragSpeed = 1f;
    public float zoomSpeed = 50f;
    public float maxZoomIn = 150f;
    public float maxZoomOut = 500f;
    public bool inverted_mouse_wheel = true;
    public bool inverted_drag = true;

    private Vector3 originalCameraPos;
    private Vector3 MouseStartPos;

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
        //mouseOrigin = minimapCam.ScreenToWorldPoint(Input.mousePosition);
        //Debug.LogWarning("camera - " + minimapCam.transform.position + ", mouse origin - " + mouseOrigin);

        //Copied Code
        MouseStartPos = Input.mousePosition;
    }
    public void DragCamera()
    {
        //diference = minimapCam.ScreenToWorldPoint(Input.mousePosition) - minimapCam.transform.position;

        //cameraPosition = mouseOrigin - diference;
        //Debug.Log(cameraPosition.x + ", " + cameraPosition.y);

        //// clamp values 
        //cameraPosition *= dragSpeed;
        //cameraPosition.x = Mathf.Clamp(cameraPosition.x, lowerLeftCorner.position.x, upperRightCorner.position.x);
        //cameraPosition.y = Mathf.Clamp(cameraPosition.y, lowerLeftCorner.position.y, upperRightCorner.position.y);

        //minimapCam.transform.position = cameraPosition;


        //Fixed Code
        diference = Input.mousePosition - MouseStartPos; //Vector from origin to mouse position
        diference = new Vector3(diference.x * dragSpeed, diference.y * dragSpeed, 0);    //Add Drag Speed & Get rid of z
        if (inverted_drag) diference *= -1; //invert drag
        // clamp values 
        //diference.x = Mathf.Clamp(diference.x, lowerLeftCorner.position.x, upperRightCorner.position.x);
        //diference.y = Mathf.Clamp(diference.y, lowerLeftCorner.position.y, upperRightCorner.position.y);
        if (backgroundRef == null)
        {
            Debug.LogAssertion("BackgroundRef not set, couldn't clamp");
            return;
        }
        //diference.x = Mathf.Clamp(diference.x, backgroundRef.rect.xMin, backgroundRef.rect.xMax);
        //diference.y = Mathf.Clamp(diference.y, backgroundRef.rect.yMin, backgroundRef.rect.yMax);
        Debug.Log(minimapCamRect.position.x.ToString() +" + "+ diference.x.ToString() +" "+ backgroundRef.rect.xMin.ToString() + " " + backgroundRef.rect.xMax.ToString());
        diference.x = Mathf.Clamp(minimapCamRect.localPosition.x + diference.x, backgroundRef.rect.xMin + minimapCam.orthographicSize, backgroundRef.rect.xMax - minimapCam.orthographicSize) - minimapCamRect.localPosition.x;
        diference.y = Mathf.Clamp(minimapCamRect.localPosition.y + diference.y, backgroundRef.rect.yMin + minimapCam.orthographicSize, backgroundRef.rect.yMax - minimapCam.orthographicSize) - minimapCamRect.localPosition.y;
        Debug.Log("Diference: " + diference);


        minimapCamRect.position += diference;

        SetMouseOrigin(); //To account for main camera's position not being changed
    }
    public void ResetCameraValues()
    {
        minimapCam.transform.position = originalCameraPos;
        minimapCam.orthographicSize = originalOrtho;
        currentOrtho = originalOrtho;
    }
    public void CameraZoom()
    {
        float my_mouseScrollDelta = Input.mouseScrollDelta.y * zoomSpeed;
        if (inverted_mouse_wheel) my_mouseScrollDelta *= -1; //Inverts Data

        if (Input.mouseScrollDelta.y != 0)
        {
            if (currentOrtho < maxZoomOut)
            {
                if (currentOrtho > maxZoomIn)
                {
                    currentOrtho += my_mouseScrollDelta;
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

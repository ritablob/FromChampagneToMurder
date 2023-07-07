using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;

public class CameraMovingManager : MonoBehaviour
{
    public Camera minimapCam;
    public RectTransform minimapCamRect;
    public RectTransform MapBackgroundRef;
    public RectTransform MiniMapBackgroundRef;
    [SerializeField] bool start_with_minimap = true;

    [Header("Adjustables")]
    [SerializeField] float dragSpeed = 1f;
    [SerializeField] float zoomSpeed = 50f;
    [SerializeField] float maxZoomIn = 150f;
    [SerializeField] float maxZoomOut = 500f;
    [SerializeField] [Range(0f, 0.002f)] float zoomDecreaseMultiplicator = 0.001f;
    [SerializeField] [Range(0f, 0.01f)] float dragDecreaseMultiplicator = 0.007f;
    [SerializeField] bool inverted_mouse_wheel = true;
    [SerializeField] bool inverted_drag = true;

    private Vector3 originalCameraPos;
    private Vector3 MouseStartPos;
    private RectTransform backgroundRef;

    private float originalOrtho;
    private Vector3 cameraPosition;
    private Rect originalCameraRect;
    private float originalCameraPositionZ;

    Vector3 current_target_position;
    float current_target_ortho_size;
    [SerializeField] float transition_time = 1f;
    private float timer = 0f;
    private bool is_transitioning = false; //slay


    void Start()
    {
        minimapCam = GetComponent<Camera>();
        originalCameraPos = minimapCam.transform.position;
        originalOrtho = minimapCam.orthographicSize;
        originalCameraPositionZ = minimapCamRect.localPosition.z;
        SwitchToMinimap();
    }

    private void Update()
    {
        originalCameraRect = minimapCamRect.rect;
        UpdateCamera();
    }

    public void SetMouseOrigin()
    {
        MouseStartPos = Input.mousePosition;
    }
    public void DragCamera()
    {

        //Fixed Code
        Vector2 diference;
        diference = Input.mousePosition - MouseStartPos; //Vector from origin to mouse position
        if (inverted_drag) diference *= -1; //invert drag

        float adj_dragSpeed = dragSpeed - ((maxZoomOut - minimapCam.orthographicSize) * dragDecreaseMultiplicator); //Decrease Drag Speed
        if (adj_dragSpeed < 0) adj_dragSpeed = 0;

        diference = new Vector2(diference.x * adj_dragSpeed, diference.y * adj_dragSpeed);    //Add Drag Speed & Get rid of 
        if (backgroundRef == null)
        {
            Debug.LogAssertion("BackgroundRef not set, couldn't clamp");
            return;
        }

        Vector2 current_camera_position = new Vector2(minimapCamRect.localPosition.x, minimapCamRect.localPosition.y);
        Vector2 next_position = current_camera_position + diference;
        //get new position vector
        diference = ClampPositionIntoRectTransform(next_position, backgroundRef, minimapCam.orthographicSize) - current_camera_position;
        //Set new position vecotr
        minimapCamRect.localPosition = new Vector3(minimapCamRect.localPosition.x + diference.x, minimapCamRect.localPosition.y + diference.y, minimapCamRect.localPosition.z);

        SetMouseOrigin(); //To account for main camera's position not being changed
    }
    public void ResetCameraValues()
    {
        SetNewTargetForCamera(originalCameraRect);
        Debug.Log("Reset");
    }
    public void CameraZoom()
    {
        float my_mouseScrollDelta = Input.mouseScrollDelta.y * zoomSpeed; //Get Data
        if (inverted_mouse_wheel) my_mouseScrollDelta *= -1; //Inverts Data

        if (Input.mouseScrollDelta.y != 0)
        {
            float currentOrtho = minimapCam.orthographicSize;

            // Zoom Decrease
            float avgZoom = (maxZoomIn + maxZoomOut) / 2;
            
            if (currentOrtho >= avgZoom && my_mouseScrollDelta > 0) //Zoom Out
            {
                my_mouseScrollDelta -= ((currentOrtho - avgZoom) * zoomDecreaseMultiplicator);
                if (my_mouseScrollDelta < 0) my_mouseScrollDelta = 0;
            }

            if (currentOrtho < avgZoom && my_mouseScrollDelta < 0) //Zoom In
            {
                my_mouseScrollDelta += ((avgZoom - currentOrtho) * zoomDecreaseMultiplicator);
                if (my_mouseScrollDelta > 0) my_mouseScrollDelta = 0;
            }

            float newOrtho = currentOrtho + my_mouseScrollDelta; 
            
            if (newOrtho < maxZoomIn) newOrtho = maxZoomIn; //Max Values
            if (newOrtho > maxZoomOut) newOrtho = maxZoomOut;
            
            minimapCam.orthographicSize = newOrtho; //Set new Size

            CheckPositionInRect(); //Fix Position Issues
        }
    }

    public Vector2 ClampPositionIntoRectTransform(Vector2 position, RectTransform borders, float orthographicSize = 0) //Input a Position, Outputs the position clambed into the given Borders
    {
        float ConfinesXMin = borders.rect.xMin + orthographicSize; //The Borders
        float ConfinesXMax = borders.rect.xMax - orthographicSize;
        float ConfinesYMin = borders.rect.yMin + orthographicSize;
        float ConfinesYMax = borders.rect.yMax - orthographicSize;

        ConfinesXMin += borders.localPosition.x; //account for changed position of background
        ConfinesXMax += borders.localPosition.x;
        ConfinesYMin += borders.localPosition.y;
        ConfinesYMax += borders.localPosition.y;

        Vector2 new_position;
        
        new_position.x = Mathf.Clamp(position.x, ConfinesXMin, ConfinesXMax);
        new_position.y = Mathf.Clamp(position.y, ConfinesYMin, ConfinesYMax);

        return(new_position);
    }

    public void SwitchToMap()
    {
        backgroundRef = MapBackgroundRef;
        CheckPositionInRect();

    }

    public void SwitchToMinimap()
    {
        backgroundRef = MiniMapBackgroundRef;
        CheckPositionInRect();
    }

    //will apply cramp
    private void CheckPositionInRect()
    {
        Vector2 current_camera_position = minimapCamRect.localPosition; //Fix Position Issues
        Vector2 new_position = ClampPositionIntoRectTransform(current_camera_position, backgroundRef, minimapCam.orthographicSize);
        minimapCamRect.localPosition = new Vector3(new_position.x, new_position.y, minimapCamRect.localPosition.z);
    }


    Rect CreateRectToFitPositions(List<Vector2> position_list, Vector2 center, float zoom_out_value = 10)
    {
        Rect new_rect = Rect.zero;

        List<float> x_positions = new List<float>();
        List<float> y_positions = new List<float>();

        // Fit Points
        foreach (Vector2 position in position_list)
        {
            x_positions.Add(position.x);
            y_positions.Add(position.y);
        };

        //new_rect.xMax = Mathf.Max(x_positions.ToArray());
        //new_rect.yMax = Mathf.Max(y_positions.ToArray());
        //new_rect.xMin = Mathf.Min(x_positions.ToArray());
        //new_rect.yMin = Mathf.Min(y_positions.ToArray());

        new_rect.max = new Vector2(Mathf.Max(x_positions.ToArray()), Mathf.Max(y_positions.ToArray()));
        new_rect.min = new Vector2(Mathf.Min(x_positions.ToArray()), Mathf.Min(y_positions.ToArray()));

        new_rect.center = center;

        //Zoom Out
        new_rect.size += new Vector2(zoom_out_value,zoom_out_value);

        return(new_rect);
    }

    void UpdateCamera()
    {
        //public Transform target;
        //public float speed;

        //float step = speed * Time.deltaTime;
        //transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        if (is_transitioning)
        {
            timer += Time.deltaTime;

            minimapCamRect.localPosition = Vector3.Lerp(minimapCamRect.localPosition, current_target_position, timer);
            minimapCam.orthographicSize = Mathf.Lerp(minimapCam.orthographicSize, current_target_ortho_size , timer);

            if (timer >= transition_time) is_transitioning = false;
        }
    }

    void SetNewTargetForCamera(Rect target)
    {
        current_target_ortho_size = Mathf.Clamp(Mathf.Max(target.xMax, target.yMax),maxZoomIn,maxZoomOut);

        current_target_position = ClampPositionIntoRectTransform(target.position, backgroundRef, current_target_ortho_size);
        current_target_position.z = originalCameraPositionZ;

        timer = 0f;
        is_transitioning = true;
    }

    public void SetNewTargetForCamera(List<Vector2> positions, Vector2 centre)
    {
        if (positions.Count == 0) return;
        SetNewTargetForCamera(CreateRectToFitPositions(positions, centre));
    }
}

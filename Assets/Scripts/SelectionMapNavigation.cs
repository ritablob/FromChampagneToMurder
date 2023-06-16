using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectionMapNavigation : MonoBehaviour
{
    public float zoomSpeed = 0.2f;
    public float moveSpeed = 0.1f;
    Camera cam;
    public float zoomNo = 2f;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    public void NavigateCharacter(Transform camPos)
    {
        /* - zoom in on the player
         * - get character json 
         * - send it to parseJson (maybe create a storage for such things?)
         * - spawn a continue button that allows you to continue after reading 
         */
        StartCoroutine(InterpolateCameraZoom(zoomNo));
        StartCoroutine(InterpolatePosition(camPos.position));
        JsonHolder jholder = camPos.GetComponent<JsonHolder>();
        //Database.currentJson = jholder.json; i should send scene info instead
        //Database.currentJson = jsonFile;
    }
    IEnumerator InterpolateCameraZoom(float endNo)
    {
        if (Math.Round(cam.orthographicSize, 2) > endNo)
        {

            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, endNo, zoomSpeed);
            yield return new WaitForEndOfFrame();
            Debug.Log("Cam zoom " + cam.orthographicSize);
            StartCoroutine(InterpolateCameraZoom(endNo));
        }else
        {
            cam.orthographicSize = endNo;
            StartCoroutine(StartSceneWithDelay());
            yield return null;
        }
        //cam.orthographicSize = endNo;
        yield return null;
    }
    IEnumerator InterpolatePosition(Vector2 endPos)
    {
        float x;
        float y;
        if (Math.Round(Mathf.Abs(cam.transform.position.x), 2)  < Mathf.Abs(endPos.x) )
        {
            x = Mathf.Lerp(cam.transform.position.x, endPos.x, moveSpeed);

            if (Math.Round(Mathf.Abs(cam.transform.position.y), 2) < Mathf.Abs(endPos.y))
            {
                y = Mathf.Lerp(cam.transform.position.y, endPos.y, moveSpeed);
            }
            else
            {
                y = cam.transform.position.y;   
            }
            yield return new WaitForEndOfFrame();
            cam.transform.position = new Vector2(x, y);
            Debug.Log("Cam pos "+x +", "+y);
            StartCoroutine(InterpolatePosition(endPos));
        }
        //cam.transform.position = endPos;
        yield return null;
    }
    IEnumerator StartSceneWithDelay()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(1);
    }
}

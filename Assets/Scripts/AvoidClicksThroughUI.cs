using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AvoidClicksThroughUI : MonoBehaviour
{
    [SerializeField] GameObject thisCanvas;
    [SerializeField] GameObject otherCanvas;
    // Update is called once per frame
    void Update()
    {
        if(thisCanvas.GetComponent<switchStateOpen>().Open == true)
        {
            otherCanvas.GetComponent<switchStateOpen>().active = false;
        }
        if(thisCanvas.GetComponent<switchStateOpen>().Open == false)
        {
            otherCanvas.GetComponent<switchStateOpen>().active = true;
        }
    }


//    if (Input.GetMouseButtonDown(0))
//        {
//            if (EventSystem.current.IsPointerOverGameObject())
//            {
//                return;
//            }
//        }
//Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

//RaycastHit hitInfo;

//        if(Physics.Raycast(ray,out hitInfo, Mathf.Infinity))
//        {
//            GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = hitInfo.point;
//        }
}

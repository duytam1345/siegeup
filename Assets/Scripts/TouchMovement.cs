using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchMovement : MonoBehaviour
{
    public Transform mainCam;

    public float minZoom;
    public float maxZoom;

    public float speedScroll;
    public float maxDistance;


    public float startDistance;
    public Vector3 startPosCam;

    public bool activeSet;

    InputManager.MouseState MouseState;


    Vector3 startPosMouseDown;

    public float moveCamSpeed;

    public Vector2 min;
    public Vector2 max;

    public Text text;

    public GameObject beginT;
    public GameObject endT;

    void Update()
    {
        MouseState = InputManager.GetMouseState();

        text.text = mainCam.position.ToString();

        if (MouseState == InputManager.MouseState.BeginZoomTouch)
        {
            beginT.SetActive(true);
        }
        if (MouseState == InputManager.MouseState.EndZoomTouch)
        {
            endT.SetActive(true);
        }

        if (MouseState == InputManager.MouseState.MouseDown)
        {
            startPosMouseDown = Input.mousePosition;
            startPosCam = mainCam.parent.position;
        }

#if UNITY_EDITOR

        if (MouseState == InputManager.MouseState.LeftDragging && !Input.GetKey(KeyCode.A))
        {
            Vector3 dir = startPosMouseDown - Input.mousePosition;

            dir *= (moveCamSpeed);

            Vector3 pos = new Vector3(
                Mathf.Clamp(((new Vector3(dir.x, 0, dir.y) / 50) + startPosCam).x, min.x, max.x),
                0,
                Mathf.Clamp(((new Vector3(dir.x, 0, dir.y) / 50) + startPosCam).z, min.y, max.y));

            mainCam.parent.position = new Vector3(pos.x, 0, pos.z);
        }

        if (MouseState == InputManager.MouseState.MouseDown)
        {
            if (Input.GetKey(KeyCode.A))
            {
                SetOriginDistance();
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (MouseState == InputManager.MouseState.LeftDragging)
            {
                SetOriginDistance();
            }
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            if (MouseState == InputManager.MouseState.MouseDown)
            {
                SetOriginDistance();
            }
        }

        if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.A))
        {
            startDistance = 0;
            activeSet = false;
        }

        if (activeSet)
        {
            SetPosCam();
        }
#else
        
#endif
        if (MouseState == InputManager.MouseState.LeftDragging && Input.touchCount == 1)
        {
            Vector3 dir = startPosMouseDown - Input.mousePosition;

            dir *= (moveCamSpeed);

            Vector3 pos = new Vector3(
                Mathf.Clamp(((new Vector3(dir.x, 0, dir.y) / 50) + startPosCam).x, min.x, max.x),
                0,
                Mathf.Clamp(((new Vector3(dir.x, 0, dir.y) / 50) + startPosCam).z, min.y, max.y));

            mainCam.parent.position = new Vector3(pos.x, 0, pos.z);
        }

        if (MouseState == InputManager.MouseState.BeginZoomTouch)
        {
            SetOriginDistance();
        }

        if (MouseState == InputManager.MouseState.EndZoomTouch)
        {
            startDistance = 0;
            activeSet = false;
        }

        if (activeSet)
        {
            SetPosCam();
        }

    }
    void SetOriginDistance()
    {
#if UNITY_EDITOR
        startDistance = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), Input.mousePosition);
#else
        startDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
#endif

        startPosCam = mainCam.position;
        activeSet = true;
    }

    void SetPosCam()
    {
        float curDistance = 0;

#if UNITY_EDITOR
        curDistance = Vector2.Distance(new Vector2(Screen.width / 2, Screen.height / 2), Input.mousePosition); ;
#else
        curDistance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position); ;
#endif
        float i = curDistance / startDistance;
        Vector3 newPos = startPosCam * i;

        if (newPos.y < minZoom)
        {
            newPos = new Vector3(0, minZoom, -minZoom);
        }
        if (newPos.y > maxZoom)
        {
            newPos = new Vector3(0, maxZoom, -maxZoom);
        }

        mainCam.position = Vector3.MoveTowards(mainCam.position, newPos, 10);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public enum MouseState
    {
        None,//Không có nút nào đc nhấn
        MouseDown,
        LeftUp,
        RightUp,
        LeftPressing,
        BeginLeftDrag,
        BeginRightDrag,
        LeftDragging,
        Drag_Pause,// Trạng thái drag nhưng chuột đang đứng yên.
        LeftUpEndDrag,
        RightUpEndDrag,
        RightDrag,
        MidDrag,
        BeginZoomTouch,
        ZoomTouching,
        ZoomTouching_Pause,
        EndZoomTouch,
        MultiButton//Có nhiều hơn 1 button dc nhấn
    }
    static Touch touch1;
    public static Touch Touch1
    {
        get
        {
            return touch1;
        }
    }
    static Touch touch2;
    public static Touch Touch2
    {
        get
        {
            return touch2;
        }
    }
    static bool is2Touching;
    static float touch2Distance;
    static bool isMouseDragging;
    static Vector2 mousePosOnMouseDown;
    static float thresHoldMouseDrag = 4f;
    public static bool IsMouseDownOverUI { get; private set; }
    public static bool IsMouseUpOverUI { get; private set; }
    public static bool ShiftKey
    {
        get
        {
            return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        }
    }
    public static bool CtrlKey
    {
        get
        {
            return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        }
    }
    public static bool AltKey
    {
        get
        {
            return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
        }
    }
    public static bool IsRightMouse
    {
        get
        {
            return Input.GetMouseButton(1);
        }
    }
    public static MouseState GetMouseState()
    {
#if !UNITY_EDITOR
        
#endif
        if (Input.touchCount >= 2)
        {
            var t1 = Input.GetTouch(0);
            var t2 = Input.GetTouch(1);
            if (t1.phase == TouchPhase.Moved || t2.phase == TouchPhase.Moved)
            {
                var distance = Vector2.Distance(t1.position, t2.position);
                if (distance != touch2Distance)
                {
                    touch2Distance = distance;
                    if (is2Touching)
                    {
                        return MouseState.ZoomTouching;
                    }
                    else
                    {
                        is2Touching = true;
                        return MouseState.BeginZoomTouch;
                    }
                }
            }
            else
            {
                return MouseState.ZoomTouching_Pause;
            }
        }
        else
        {
            if (is2Touching)
            {
                is2Touching = false;
                return MouseState.EndZoomTouch;
            }
        }
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            mousePosOnMouseDown = Input.mousePosition;
            IsMouseDownOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            return MouseState.MouseDown;
        }
        var leftMouse = Input.GetMouseButton(0);
        var midMouse = Input.GetMouseButton(2);
        var rightMouse = Input.GetMouseButton(1);
        if (leftMouse && midMouse || leftMouse && rightMouse || midMouse && rightMouse)
        {
            return MouseState.MultiButton;
        }
        if (leftMouse || midMouse || rightMouse)
        {
            if (isMouseDragging)
            {
                if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0)
                {
                    return MouseState.Drag_Pause;
                }
                if (leftMouse)
                {
                    return MouseState.LeftDragging;
                }
                else
                {
                    if (rightMouse)
                    {
                        return MouseState.RightDrag;
                    }
                    else
                    {
                        if (midMouse)
                        {
                            return MouseState.MidDrag;
                        }
                    }
                }
            }
            else
            {
                Vector2 newPos = Input.mousePosition;
                float distance = (newPos - mousePosOnMouseDown).magnitude;
                if (distance > thresHoldMouseDrag)
                {
                    isMouseDragging = true;
                    if (leftMouse)
                    {
                        return MouseState.BeginLeftDrag;
                    }
                    else
                    {
                        if (rightMouse)
                        {
                            return MouseState.BeginRightDrag;
                        }
                    }
                }
            }
            return MouseState.LeftPressing;
        }
        var rightUp = Input.GetMouseButtonUp(1);
        var leftUp = Input.GetMouseButtonUp(0);
        var midUp = Input.GetMouseButtonUp(2);
        if (leftUp || rightUp || midUp)
        {
            IsMouseUpOverUI = UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
            if (isMouseDragging)
            {
                isMouseDragging = false;
                if (leftUp)
                {
                    return MouseState.LeftUpEndDrag;
                }
                else
                {
                    return MouseState.RightUpEndDrag;
                }
            }
            else
            {
                isMouseDragging = false;
                if (leftUp)
                {
                    return MouseState.LeftUp;
                }
                else
                {
                    return MouseState.RightUp;
                }
            }
        }
        return MouseState.None;
    }
}

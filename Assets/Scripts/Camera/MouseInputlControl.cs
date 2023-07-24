using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Midea.DigitalTwin
{
    /// <summary>
    /// 鼠标输入控制
    /// </summary>
    public class MouseInputlControl : ICameraInputControl
    {
        public bool can_control_cam = true;//是否可以执行该脚本控制摄像机
        public bool StartMove()
        {
            if (!can_control_cam) return false;
            return Input.GetMouseButtonDown(0);
        }
        public bool Moving()
        {
            if (!can_control_cam) return false;
            return Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);
        }
        public bool EndMove()
        {
            if (!can_control_cam) return false;
            return Input.GetMouseButtonUp(0);
        }

        public Vector3 GetMoveValue() { return -Input.mousePosition; }


        public bool StartRotation()
        {
            if (!can_control_cam) return false;
            return Input.GetMouseButton(1) && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f);
        }
        public Vector2 GetRotation()
        {
            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");
            return new Vector2(x, y);
        }


        public bool StartZoom()
        {
            //if (!can_control_cam) return false; 
            return true;
        }
        public float GetZoom()
        {
            if (!can_control_cam) return 0;
            return Input.GetAxis("Mouse ScrollWheel");
        }
    }
}

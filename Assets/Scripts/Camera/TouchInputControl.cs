using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Midea.DigitalTwin
{
    public class TouchInputControl : ICameraInputControl
    {
        public bool StartMove() { return Input.touchCount > 2 && Input.GetTouch(2).phase == TouchPhase.Began; }
        public bool Moving() { return Input.touchCount > 2 && Input.GetTouch(2).phase == TouchPhase.Moved && Input.GetTouch(2).deltaPosition != Vector2.zero; }
        public bool EndMove() { return Input.touchCount <= 2; }
        public Vector3 GetMoveValue() { return new Vector3(Input.GetTouch(2).position.x, Input.GetTouch(2).position.y, 0); }

        public bool StartRotation() { return Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved; }

        public Vector2 GetRotation()
        {
            float x = Input.GetTouch(0).deltaPosition.x / 10f;
            float y = Input.GetTouch(0).deltaPosition.y / 10f;
            return new Vector2(x, y);
        }


        float cacheDistance = 0;
        float currentDistance = 0;
        public bool StartZoom()
        {
            if (Input.touchCount == 2)
            {
                if (Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    cacheDistance = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
                }
                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    currentDistance = (Input.GetTouch(0).position - Input.GetTouch(1).position).magnitude;
                    return true;
                }
            }
            return false;
        }

        public float GetZoom()
        {
            float value = 0;
            value = cacheDistance - currentDistance;
            cacheDistance = currentDistance;
            Mathf.Clamp(value, -1, 1);
            return -value;
        }
    }
}

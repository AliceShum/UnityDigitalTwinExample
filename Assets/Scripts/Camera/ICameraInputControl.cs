using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Midea.DigitalTwin
{
    /// <summary>接口 相机输入控制</summary>
    public interface ICameraInputControl
    {

        bool StartMove();
        bool Moving();
        bool EndMove();
        Vector3 GetMoveValue();

        bool StartRotation();
        Vector2 GetRotation();

        bool StartZoom();
        float GetZoom();
    }
}

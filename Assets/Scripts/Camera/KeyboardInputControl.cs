using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Midea.DigitalTwin
{
    /// <summary>
    /// 鼠标输入控制
    /// </summary>
    public class KeyboardInputControl : ICameraInputControl
    {
        private Vector3 move_vec = Vector3.zero;//记录移动方向
        //private float timer = 0;//计时器，计算移动了多久
        //private float speed = 10f; //移动速度

        public bool can_control_cam = true;//是否可以执行该脚本控制摄像机

        public bool StartMove()
        {
            if (!can_control_cam) return false;
            bool is_start_move = Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
                                 || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow);
            return is_start_move;
        }
        public bool Moving()
        {
            if (!can_control_cam) return false;
            bool is_moving_up = Input.GetKey(KeyCode.UpArrow);
            if (is_moving_up)
            {
                move_vec += new Vector3(0, 1, 0);
            }
            bool is_moving_down = Input.GetKey(KeyCode.DownArrow);
            if (is_moving_down)
            {
                move_vec += new Vector3(0, -1, 0);
            }
            bool is_moving_left = Input.GetKey(KeyCode.LeftArrow);
            if (is_moving_left)
            {
                move_vec += new Vector3(-1, 0, 0);
            }
            bool is_moving_right = Input.GetKey(KeyCode.RightArrow);
            if (is_moving_right)
            {
                move_vec += new Vector3(1, 0, 0);
            }
            bool is_moving = is_moving_up || is_moving_down || is_moving_left || is_moving_right;
            return is_moving;
        }
        public bool EndMove()
        {
            if (!can_control_cam) return false;
            bool is_end_move = Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.DownArrow)
                                 || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow);

            if (is_end_move)
            {
                move_vec = Vector3.zero;
                //timer = 0;
            }
            return is_end_move;
        }
        public Vector3 GetMoveValue()
        {
            Vector3 final = move_vec;// * speed;// * timer;
            return final;
        }

        public void CountTime(float deltaTime)
        {
            //timer += deltaTime;  //加速度
        }

        //键盘不能缩放 旋转
        public bool StartRotation() { return false; }
        public Vector2 GetRotation()
        {
            return Vector2.zero;
        }
        public bool StartZoom() { return false; }
        public float GetZoom() { return 0f; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Midea.DigitalTwin
{
    /// <summary>
    /// 相机控制外部函数
    /// </summary>
    public partial class CameraControl
    {
        public void SetState(int index)
        {
            if (index >= 0 && index < cameraStateList.Count)
            {
                cameraState.SetState(cameraStateList[index]);
                target.position = cameraState.lookPoint;
                x = cameraState.selfRotationValue.y;
                y = cameraState.selfRotationValue.x;
            }
        }

        public void SetState(Vector3 v3)
        {
            cameraState.lookPoint = v3;
            target.position = v3;
        }

        public void SetState(float dis, float? minDis = null, float? maxDis = null)
        {
            cameraState.dis = dis;
            if (minDis != null)
                cameraState.minDis = minDis.Value;
            if (maxDis != null)
                cameraState.maxDis = maxDis.Value;
        }

        public void SetState(float angle_x, float angle_y)
        {
            cameraState.selfRotationValue.x = angle_x;
            cameraState.selfRotationValue.y = angle_y;
            y = angle_x;
            x = angle_y;
        }

        public void SetState(Vector3 v3, float dis, float? minDis = null, float? maxDis = null)
        {
            SetState(v3);
            SetState(dis, minDis, maxDis);
        }

        public void SetState(Vector3 v3, float x, float y)
        {
            SetState(v3);
            SetState(x, y);
        }

        public void SetState(Vector3 v3, float dis, float x, float y, float? minDis = null, float? maxDis = null)
        {
            SetState(v3);
            SetState(dis, minDis, maxDis);
            SetState(x, y);
        }

        public void SetState(Transform trans)
        {
            Vector3 angles = trans.eulerAngles;
            SetState(trans.position, angles.x, angles.y);
        }

        public void SetState(Transform trans, float dis, float? minDis = null, float? maxDis = null)
        {
            SetState(trans);
            SetState(dis, minDis, maxDis);
        }

        public void SetState(Transform trans, Vector3 offset, float dis, float? minDis = null, float? maxDis = null)
        {
            SetState(trans, dis, minDis, maxDis);

            SetState(target.position + offset);
        }

        public void SetTarget(Transform t)
        {
            target = t;
        }

        //设置摄像机能否移动 缩放 旋转
        public void SetMotion(bool can_move, bool can_zoom, bool can_rotate)
        {
            canMove = can_move;
            canZoom = can_zoom;
            canRotate = can_rotate;
        }

        //检查摄像机是否在操作(移动、旋转、缩放)
        public bool CheckIsCamOperating()
        {
            return isCamOperating;
        }

        //检查摄像机是否在移动
        public bool CheckIsCamMoving()
        {
#if UNITY_STANDALONE_WIN
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow)
                || Input.GetKey(KeyCode.RightArrow))
            { return true; }
            if (Input.GetMouseButton(0) && (Input.GetAxis("Mouse X") != 0f || Input.GetAxis("Mouse Y") != 0f))
            { return true; }
#endif
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 2 && Input.GetTouch(2).phase == TouchPhase.Moved && Input.GetTouch(2).deltaPosition != Vector2.zero)
        { return true; }
#endif
            return false;
        }

        //检查摄像机是否在缩放
        public bool CheckIsCamZooming()
        {
#if UNITY_STANDALONE_WIN
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            { return true; }
#endif
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount == 2 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved))
        { return true; }
#endif
            return false;
        }

        //设置摄像机的操作方式（鼠标、屏幕、键盘）是否可以操作
        public void SetMovePatternCanUse(bool can_cameraInput, bool can_cameraInput2)
        {
#if UNITY_STANDALONE_WIN
            MouseInputlControl cameraInput = this.cameraInput as MouseInputlControl;
            cameraInput.can_control_cam = can_cameraInput;

            if (this.cameraInput2 != null)
            {
                KeyboardInputControl cameraInput2 = this.cameraInput2 as KeyboardInputControl;
                cameraInput2.can_control_cam = can_cameraInput2;
            }
#endif
#if UNITY_ANDROID || UNITY_IOS
            TouchInputControl cameraInput = this.cameraInput as TouchInputControl;
            cameraInput.can_control_cam = can_cameraInput;
#endif
        }

        //设置摄像机的移动限制范围
        public void SetCamMoveArea(float max_x, float min_x, float max_z, float min_z)
        {
            this.cam_move_max_x = max_x;
            this.cam_move_max_z = max_z;
            this.cam_move_min_x = min_x;
            this.cam_move_min_z = min_z;
        }

        //设置当前state摄像机的移动速度
        public void SetCamMoveSpeed(float mSpeed, float mSpeedKeyboard)
        {
            cameraState.mSpeed = mSpeed;
            cameraState.mSpeedKeyboard = mSpeedKeyboard;
        }

        //设置当前 state摄像机的缩放速度
        public void SetCamZoomSpeed(float speed)
        {
            cameraState.sSpeed = speed;
        }

        //设置当前 state摄像机的旋转速度
        public void SetCamRotateSpeed(float speed)
        {
            cameraState.xSpeed = speed;
            cameraState.ySpeed = speed;
        }

        public delegate void AfterDampDelegate();
        //镜头缓慢移动到新的点
        public void DampSetState(int index, float cam_move_time = 1f, AfterDampDelegate del = null)
        {
            if (index >= 0 && index < cameraStateList.Count)
            {
                isCamChangingState = true;
                CameraState new_state = cameraStateList[index];
                Quaternion rotation = Quaternion.Euler(new_state.selfRotationValue);
                Vector3 disVector = new Vector3(0.0f, 0.0f, -new_state.dis);
                Vector3 position = rotation * disVector + new_state.lookPoint;

                target.DOMove(new_state.lookPoint, cam_move_time);
                Tweener tweener = myCamera.DOMove(position, cam_move_time);

                tweener.OnComplete(() => {
                    isCamChangingState = false;
                    SetState(index);
                    if (del != null)
                        del();
                });
            }
        }

        //镜头缓慢移动到新的点(target_pos:target的新位置。dis:新的dis)
        public void DampSetState(Vector3 target_pos, float dis, float min_dis, float max_dis,
                                 float cam_move_time = 1f)
        {
            isCamChangingState = true;
            Vector3 position = GetCamPositionByDistanceToTarget(target_pos, dis);

            target.DOMove(target_pos, cam_move_time);
            Tweener tweener = myCamera.DOMove(position, cam_move_time);

            tweener.OnComplete(() => {
                SetState(target_pos);
                SetState(dis, min_dis, max_dis);
                isCamChangingState = false;
            });
        }

        //获取到某个目标位置之间某个距离的摄像机位置(当前旋转值)
        Vector3 GetCamPositionByDistanceToTarget(Vector3 target_pos, float dis)
        {
            Quaternion rotation = Quaternion.Euler(cameraState.selfRotationValue);
            Vector3 disVector = new Vector3(0.0f, 0.0f, -dis);
            Vector3 position = rotation * disVector + target_pos;
            return position;
        }
    }
}

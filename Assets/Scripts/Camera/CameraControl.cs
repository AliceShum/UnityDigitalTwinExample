using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace Midea.DigitalTwin
{
    /// <summary>
    /// 相机控制
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public partial class CameraControl : MonoBehaviour
    {
        //=================================
        /// <summary>相机本身 </summary>
        public Transform myCamera = null;
        /// <summary>相机要注视的目标 </summary>
        public Transform target = null;
        /// <summary>位置限制开关</summary>
        public bool locationRestrict;
        /// <summary>限制高度 控制相机最低高度</summary>
        public float restrictHeight = 0.2f;
        /// <summary>能否移动 </summary>
        public bool canMove = true;
        /// <summary>能否缩放 </summary>
        public bool canZoom = true;
        /// <summary>能否旋转 </summary>
        public bool canRotate = true;
        /// <summary>可以移动的最大x值 </summary>
        public float cam_move_max_x = float.PositiveInfinity;
        /// <summary>可以移动的最小x值 </summary>
        public float cam_move_min_x = float.NegativeInfinity;
        /// <summary>可以移动的最大z值 </summary>
        public float cam_move_max_z = float.PositiveInfinity;
        /// <summary>可以移动的最小z值 </summary>
        public float cam_move_min_z = float.NegativeInfinity;
        /// <summary>相机当前状态 </summary>
        public CameraState cameraState;
        /// <summary>相机输入控制实例 </summary>
        public ICameraInputControl cameraInput;
        public KeyboardInputControl cameraInput2;
        /// <summary>存储相机状态列表 </summary>
        public List<CameraState> cameraStateList = new List<CameraState>();
        //记录相机角度
        public float x = 0.0f;
        public float y = 0.0f;

        //摄像机是否在跳转其他state
        private bool isCamChangingState = false;

        //=================================

        /// <summary>世界点记录 </summary>
        private Vector3 worldPointRecord;
        /// <summary>屏幕点记录</summary>
        private Vector3 screenPointRecord;
        /// <summary>移动值</summary>
        private Vector3 moveValue;
        /// <summary>激活移动</summary>
        private bool activateMobile;
        /// <summary>检测摄像机是否在移动位置</summary>
        private bool isCamOperating = false;

        //=================================
        ///Unity

        private void Reset()
        {
            if (target == null)
            {
                GameObject obj = GameObject.Find("CameraTarget");
                if (obj == null)
                {
                    obj = new GameObject("CameraTarget");
                }
                obj.transform.position = Vector3.zero;
                target = obj.transform;
            }
        }

        private void Start()
        {
            myCamera = this.transform;
#if UNITY_STANDALONE_WIN
            cameraInput = new MouseInputlControl();
            cameraInput2 = new KeyboardInputControl();
#endif
#if UNITY_ANDROID || UNITY_IOS
            cameraInput = new TouchInputControl();
#endif
            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            if (cameraStateList.Count > 0) { SetState(0); }
        }

        private void Update()
        {
            ConfigData();
            if (!isCamChangingState)
                UsageData();
        }

        //=================================

        /// <summary>配置数据</summary>
        private void ConfigData()
        {
            //==================================================
            //移动
            if (canMove)
            {
                //键盘 
                if (cameraInput2 != null)
                {
                    if (cameraInput2.StartMove())
                    {
                        activateMobile = true;
                        screenPointRecord = cameraInput2.GetMoveValue() * cameraState.mSpeedKeyboard;
                        worldPointRecord = target.position;
                        myCamera.parent = target;
                    }
                    if (cameraInput2.Moving())
                    {
                        cameraInput2.CountTime(Time.deltaTime);
                        moveValue = screenPointRecord - cameraInput2.GetMoveValue() * cameraState.mSpeedKeyboard;
                        //cameraState.lookPoint = worldPointRecord + myCamera.up * moveValue.y * cameraState.mSpeed + myCamera.right * moveValue.x * cameraState.mSpeed;
                        Vector3 newLookPoint = worldPointRecord + Vector3.forward * moveValue.y * cameraState.mSpeed + Vector3.right * moveValue.x * cameraState.mSpeed;
                        CheckIsCamMoveInsideArea(newLookPoint);//cameraState.lookPoint
                    }
                    if (cameraInput2.EndMove())
                    {
                        myCamera.parent = null;
                        activateMobile = false;
                    }
                }

                //鼠标 or 触屏
                if (cameraInput.StartMove())
                {
                    activateMobile = true;
                    screenPointRecord = cameraInput.GetMoveValue();
                    worldPointRecord = target.position;
                    myCamera.parent = target;
                }
                if (cameraInput.Moving())
                {
                    moveValue = screenPointRecord - cameraInput.GetMoveValue();
                    //cameraState.lookPoint = worldPointRecord + myCamera.up * moveValue.y * cameraState.mSpeed + myCamera.right * moveValue.x * cameraState.mSpeed;
                    Vector3 newLookPoint = worldPointRecord + Vector3.forward * moveValue.y * cameraState.mSpeed + Vector3.right * moveValue.x * cameraState.mSpeed;
                    CheckIsCamMoveInsideArea(newLookPoint);//cameraState.lookPoint
                }
                if (cameraInput.EndMove())
                {
                    myCamera.parent = null;
                    activateMobile = false;
                }
            }

            if (!activateMobile)
            {
                //==================================================
                //旋转
                if (canRotate && cameraInput.StartRotation())
                {
                    x += cameraInput.GetRotation().x * cameraState.xSpeed * Time.deltaTime;
                    y -= cameraInput.GetRotation().y * cameraState.ySpeed * Time.deltaTime;
                    y = ClampAngle(y, cameraState.yMinLimit, cameraState.yMaxLimit);
                    cameraState.selfRotationValue = new Vector3(y, x, 0.0f);
                }
                //==================================================
                //缩放
                if (canZoom && cameraInput.StartZoom())
                {
                    cameraState.dis -= cameraInput.GetZoom() * cameraState.sSpeed * Time.deltaTime * cameraState.dis;
                    cameraState.dis = Mathf.Clamp(cameraState.dis, cameraState.minDis, cameraState.maxDis);
                }
            }
        }

        /// <summary>使用数据</summary>
        private void UsageData()
        {
            if (canMove && activateMobile)
            {
                target.position = cameraState.lookPoint; //移动的时候，移动目标物体
                isCamOperating = true;
            }
            else if (canRotate || canZoom)
            {
                Quaternion rotation = Quaternion.Euler(cameraState.selfRotationValue);
                Vector3 disVector = new Vector3(0.0f, 0.0f, -cameraState.dis);
                Vector3 position = rotation * disVector + target.position;

                if (cameraState.needDamping)
                {
                    myCamera.rotation = Quaternion.Lerp(myCamera.rotation, rotation, Time.deltaTime * cameraState.damping);
                    myCamera.position = Vector3.Lerp(myCamera.position, position, Time.deltaTime * cameraState.damping);
                }
                else
                {
                    myCamera.rotation = rotation;
                    myCamera.position = position;
                }

                //限制最低高度
                if (locationRestrict && this.transform.position.y < restrictHeight)
                {
                    this.transform.position = new Vector3(transform.position.x, restrictHeight, transform.position.z);
                }
                isCamOperating = true;
            }
            else
            {
                isCamOperating = false;
            }
        }

        /// <summary>角度锁定</summary>
        private static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }

        /// <summary>检查摄像机移动是否超出范围</summary>
        private void CheckIsCamMoveInsideArea(Vector3 newLookPoint)
        {
            Vector3 result = newLookPoint;
            if (!float.IsPositiveInfinity(cam_move_max_x))
            {
                result.x = (result.x > cam_move_max_x) ? cam_move_max_x : result.x;
            }
            if (!float.IsNegativeInfinity(cam_move_min_x))
            {
                result.x = (result.x < cam_move_min_x) ? cam_move_min_x : result.x;
            }
            if (!float.IsPositiveInfinity(cam_move_max_z))
            {
                result.z = (result.z > cam_move_max_z) ? cam_move_max_z : result.z;
            }
            if (!float.IsNegativeInfinity(cam_move_min_z))
            {
                result.z = (result.z < cam_move_min_z) ? cam_move_min_z : result.z;
            }
            cameraState.lookPoint = result;
        }
    }

    /// <summary>相机状态</summary>
    [System.Serializable]
    public class CameraState : ICloneable
    {
        /// <summary>名字</summary>
        [Header("名字")] public string name = "";
        /// <summary>注视点</summary>
        [Header("注视点")] public Vector3 lookPoint = Vector3.zero;
        /// <summary>自身旋转值</summary>
        [Header("自身旋转值")] public Vector3 selfRotationValue = Vector3.zero;
        /// <summary>x轴旋转速度</summary>
        [Header("横向旋转速度")] public float xSpeed = 200f;
        /// <summary>y轴旋转速度</summary>
        [Header("纵向旋转速度")] public float ySpeed = 200f;
        /// <summary>缩放速度</summary>
        [Header("滚轮缩放速度")] public float sSpeed = 10f;
        /// <summary>移动速度</summary>
        [Header("滚轮按下移动速度")] public float mSpeed = 0.1f;
        /// <summary>键盘移动速度</summary>
        [Header("键盘移动速度")] public float mSpeedKeyboard = 10f;
        /// <summary>y轴最小角度</summary>
        [Header("纵向最小角度")] public float yMinLimit = 0f;
        /// <summary>y轴最大角度</summary>
        [Header("纵向最大角度")] public float yMaxLimit = 60f;
        /// <summary>与目标之间的距离</summary>
        [Header("与目标之间的距离")] public float dis = 40;
        /// <summary>与目标之间最小距离</summary>
        [Header("与目标之间最小距离")] public float minDis = 0f;
        /// <summary>与目标之间最大距离</summary>
        [Header("与目标之间最大距离")] public float maxDis = 100f;
        /// <summary>是否差值</summary>
        [Header("是否差值")] public bool needDamping = true;
        /// <summary>插值系数</summary>
        [Header("插值系数")] public float damping = 5;

        /// <summary>浅克隆</summary>
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>深克隆</summary>
        public CameraState DeepClone()
        {
            object obj = null;

            //将对象序列化成内存中的二进制流
            MemoryStream inputStream;
            using (inputStream = new MemoryStream())
            {
                BinaryFormatter inputFormatter = new BinaryFormatter();
                inputFormatter.Serialize(inputStream, this);
            }

            //将二进制流反序列化为对象
            using (MemoryStream outputStream = new MemoryStream(inputStream.ToArray()))
            {
                BinaryFormatter outputFormatter = new BinaryFormatter();
                obj = outputFormatter.Deserialize(outputStream);
            }

            return (CameraState)obj;
        }

        public void SetState(CameraState state)
        {
            lookPoint = state.lookPoint;
            selfRotationValue = state.selfRotationValue;
            xSpeed = state.xSpeed;
            ySpeed = state.ySpeed;
            sSpeed = state.sSpeed;
            mSpeed = state.mSpeed;
            mSpeedKeyboard = state.mSpeedKeyboard;
            yMinLimit = state.yMinLimit;
            yMaxLimit = state.yMaxLimit;
            dis = state.dis;
            minDis = state.minDis;
            maxDis = state.maxDis;
            needDamping = state.needDamping;
            damping = state.damping;
        }
    }
}
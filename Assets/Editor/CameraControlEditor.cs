using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Midea.DigitalTwin
{
#if UNITY_EDITOR
    [CustomEditor(typeof(CameraControl))]
    public class CameraControlEditor : Editor
    {
        private bool displayContent = false;

        public override void OnInspectorGUI()
        {
            CameraControl control = target as CameraControl;
            SerializedObject serializ = this.serializedObject;

            EditorGUILayout.PropertyField(serializ.FindProperty("myCamera"));
            EditorGUILayout.PropertyField(serializ.FindProperty("target"));
            EditorGUILayout.PropertyField(serializ.FindProperty("locationRestrict"));
            EditorGUILayout.PropertyField(serializ.FindProperty("restrictHeight"));
            EditorGUILayout.PropertyField(serializ.FindProperty("cameraState"), true);

            if (GUILayout.Button("添加注视点"))
            {
                CameraState state = new CameraState();
                state = (CameraState)control.cameraState.Clone();
                state.lookPoint = control.target.position;
                state.selfRotationValue = new Vector3(control.y, control.x, 0.0f);
                state.dis = Vector3.Distance(control.myCamera.transform.position, state.lookPoint);
                state.minDis = Mathf.Ceil(state.dis * 0.3f);
                state.maxDis = Mathf.Ceil(state.dis * 1.7f);
                control.cameraStateList.Add(state);
            }

            if (GUILayout.Button("删除最后一个注视点"))
            {
                if (control.cameraStateList.Count > 0)
                    control.cameraStateList.RemoveAt(control.cameraStateList.Count - 1);
            }

            if (displayContent = EditorGUILayout.Toggle("显示当前所有注视点", displayContent))
            {
                //更改显示方式添加更改按钮 和名字
                //EditorGUILayout.PropertyField(sObj.FindProperty("cameraStateList"), true);
                //sObj.ApplyModifiedProperties();
                SerializedProperty stateList = serializ.FindProperty("cameraStateList");

                for (int i = 0; i < stateList.arraySize; i++)
                {
                    SerializedProperty state = stateList.GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(state, true);
                    if (GUILayout.Button("改变注视点"))
                    {
                        CameraState cs = new CameraState();
                        cs = (CameraState)control.cameraState.Clone();
                        cs.lookPoint = control.target.position;
                        cs.selfRotationValue = new Vector3(control.y, control.x, 0.0f);
                        cs.dis = Vector3.Distance(control.myCamera.transform.position, cs.lookPoint);
                        cs.minDis = Mathf.Ceil(cs.dis * 0.3f);
                        cs.maxDis = Mathf.Ceil(cs.dis * 1.7f);
                        control.cameraStateList[i] = cs;
                    }
                    //GUILayout.Label("运行状态下点击改变以后要在该脚本上右键鼠标选择Copy Component,停止运行后在该脚本上右键鼠标选择Paste Component Values");
                }
            }
            serializ.ApplyModifiedProperties();
        }

        private void OnSceneGUI()
        {
            var control = target as CameraControl;

            if (displayContent)
            {
                if (control.cameraStateList.Count > 0)
                {
                    Undo.RecordObject(control, "CameraControl");

                    //node handle display:
                    for (int i = 0; i < control.cameraStateList.Count; i++)
                    {
                        Handles.Label(control.cameraStateList[i].lookPoint, "'" + i.ToString() + "' target");
                        control.cameraStateList[i].lookPoint = Handles.PositionHandle(control.cameraStateList[i].lookPoint, Quaternion.identity);
                    }
                }
            }
        }
    }
#endif
}

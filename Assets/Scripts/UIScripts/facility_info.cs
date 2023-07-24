using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//记录的设备信息及物体(运行状态按钮)
public class facility_info
{
    public int index;//自身的index
    public int find_index;//从facility_data_list获取数据的index

    public string current_state; //当前的设备运行状态
    public string nong_du; //排污设备的浓度

    public GameObject obj;//设备物体
    public Transform obj_btn;//设备的按钮transform
    public Image icon;//设备按钮的img
    public Image name;//名字的 Text

    public float last_get_data_time;//上次获取设备运行状态的时间
}

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class CommonData : Singleton<CommonData>
{
    public float cam_move_time = 1f; //相机移动时间
    public string get_facility_info_address = "http://10.73.46.53:16000/";//获取设备信息的网址
    public string get_facility_info_interface = "findDeviceAndPointList";//获取设备信息的接口

    public string org_1201_scene_name = "1201";
    public string resources_folder_name;

    public Dictionary<string, Type> panel_map = new Dictionary<string, Type>
    {
        {"main_panel", typeof(MainPanel)},
        {"btns_panel", typeof(BtnsPanel)},
        {"info_panel", typeof(InfoPanel)},
        {"cam_panel", typeof(CamPanel)},
        {"warn_panel", typeof(WarnPanel)},
    };

    //橙色数字
    public Dictionary<int, string> orange_num_dic = new Dictionary<int, string>
    {
        {1, "Sprites/橙 1"},
        {2, "Sprites/橙 2"},
        {3, "Sprites/橙 3"},
    };
    //红色数字
    public Dictionary<int, string> red_num_dic = new Dictionary<int, string>
    {
        {1, "Sprites/红 1"},
        {2, "Sprites/红 2"},
        {3, "Sprites/红 3"},
    };
    //黄色数字
    public Dictionary<int, string> yellow_num_dic = new Dictionary<int, string>
    {
        {1, "Sprites/黄 1"},
        {2, "Sprites/黄 2"},
        {3, "Sprites/黄 3"},
    };

    //环保图片
    public Dictionary<int, string> hb_sprite_dic = new Dictionary<int, string>
    {
        {1, "Sprites/环保 绿"},
        {2, "Sprites/环保 黄"},
        {3, "Sprites/环保 红"},
    };
    //排污图片
    public Dictionary<int, string> pw_sprite_dic = new Dictionary<int, string>
    {
        {1, "Sprites/排污 绿"},
        {2, "Sprites/排污 黄"},
        {3, "Sprites/排污 红"},
    };
    //监控图片
    public Dictionary<int, string> jk_sprite_dic = new Dictionary<int, string>
    {
        {1, "Sprites/监控 绿"},
        {2, "Sprites/监控 黄"},
        {3, "Sprites/监控 红"},
    };
    //开门图片
    public Dictionary<int, string> km_sprite_dic = new Dictionary<int, string>
    {
        {1, "Sprites/开门  绿"},
        {2, "Sprites/开门  黄"},
        {3, "Sprites/开门  红"},
    };

    //public string front_topic = "gw/data/yun/"; //mqtt topic的前半部分
    //public string all_type_name = "全部设备";//全部设备按钮的名字
    public Dictionary<string, string> type_pic_path_dic = new Dictionary<string, string> {
        {"环保设备", "UI/sit_standby"},
        {"排污监测设备", "UI/up_standby"},
        {"开门状态采集设备", "UI/sit_standby"},
        {"画面监控设备", "UI/up_standby"},
    }; //不同类型名字，对应设备类型的照片路径

}



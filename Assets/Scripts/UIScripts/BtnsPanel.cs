using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.JSON.LitJson;

public class BtnsPanel : MonoBehaviour
{
    //string json = CreateJsonFormat.GetRtspInfoJson(cam_name, video_path);
    //EventManager.Instance.DispatchEvent("打开监控面板", json);
    //EventManager.Instance.AddListener("打开监控面板", OpenCamPanel);

    //private UIController ui_controller;

    private Dictionary<Transform, Transform> building_btn_dic = new Dictionary<Transform, Transform>(); //建筑物UI及对应的建筑物transform(跟随移动需要)

    private Transform facility_btn_parent;//设备运行按钮demo的父父物体
    private GameObject hb_demo;//环保设备按钮的demo
    private GameObject pwjc_demo;//排污监测设备按钮的demo
    private GameObject hmjk_demo;//画面监控监测设备按钮的demo
    private GameObject kmzt_demo;//开门状态采集设备按钮的demo

    private Dictionary<string, GameObject> facility_btn_type_parents = new Dictionary<string, GameObject>();//某个设备类型的名字，对应类型全部设备按钮的父物体
    private Dictionary<string, facility_info[]> facility_type_dic = new Dictionary<string, facility_info[]>();//某个设备类型的名字，对应类型全部设备按钮的facility_info   string对应type_names

    public int last_click_facility_index = -1;//上次点击过的设备facility_info中的 index
    public string last_click_facility_type;//上次点击过的设备facility_info中的 设备类型

    /*public BtnsPanel(UIController ui_con)
    {
        this.ui_controller = ui_con;
    }*/

    private void Start() {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        transform.localPosition = Vector3.zero;
        UIController.Instance.BtnsPanel = this;
        //EventManager.Instance.AddListener("摄像机移动", RefreshAllBuildingUIPos);
        EventManager.Instance.AddListener("更新设备信息", OnGetSingleFacilityBtnInfo);
        EventManager.Instance.AddListener("关闭设备信息面板", OnCloseInfoPanel);

        CreateBuildingUI();
        RefreshAllBuildingUIPos();

        InitFacilityBtns();
        CreateAllFacilityBtns();
    }

    #region 建筑物的指示UI
    //生成对应建筑物的指示UI
    void CreateBuildingUI()
    {
        GameObject building_point = GameObject.Find("model/building_point");
        Transform btn_parent = transform.Find("building_btn_parent").transform;
        GameObject btn_demo = btn_parent.Find("demo").gameObject;
        btn_demo.SetActive(false);
        foreach (Transform t in building_point.transform)
        {
            GameObject go = Instantiate(btn_demo);
            go.transform.SetParent(btn_parent);
            go.transform.Find("name").GetComponent<Text>().text = t.name;
            building_btn_dic.Add(go.transform, t);
            go.SetActive(true);
        }
    }

    //建筑物对应UI跟随相机移动
    public void RefreshAllBuildingUIPos()
    {
        foreach (KeyValuePair<Transform, Transform> pair in building_btn_dic)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(pair.Value.position);
            pair.Key.position = new Vector3(pos.x, pos.y, pos.z);
        }
    }
    #endregion

    #region 设备按钮

    void InitFacilityBtns() {
        facility_btn_parent = transform.Find("facility_btn_parent");
        hb_demo = facility_btn_parent.Find("hb_demo").gameObject;
        hb_demo.SetActive(false);
        pwjc_demo = facility_btn_parent.Find("pwjc_demo").gameObject;
        pwjc_demo.SetActive(false);
        hmjk_demo = facility_btn_parent.Find("hmjk_demo").gameObject;
        hmjk_demo.SetActive(false);
        kmzt_demo = facility_btn_parent.Find("kmzt_demo").gameObject;
        kmzt_demo.SetActive(false);
    }

    //生成全部设备运行状态标签
    void CreateAllFacilityBtns()
    {
        string[] type_names = MainController.Instance.type_names;
        for (int i = 1; i < type_names.Length; i++)
        {
            string type_name = type_names[i];
            facility_info[] type_facility_data_arr = MainController.Instance.GetTypeAllFacilityData(type_name);

            //生成设备类型对应按钮的父物体，把不同类型的按钮放置在父对象下
            GameObject parent = new GameObject();
            parent.name = type_name;
            parent.transform.SetParent(facility_btn_parent);
            facility_btn_type_parents.Add(type_name, parent);

            for (int j = 0; j < type_facility_data_arr.Length; j++)
            {
                string facility_id = MainController.Instance.GetSingleFacilityData(type_facility_data_arr[j].find_index).id; //根据id命名设备模型
                //查找并获取设备物体
                string path = string.Format("model/facility_parent/{0}", facility_id);
                GameObject obj = GameObject.Find(path);
                type_facility_data_arr[j].obj = obj;
                type_facility_data_arr[j].index = j;
                CreateSingleFacilityBtn(type_facility_data_arr[j], parent);
                RefreshSingleFacilityBtnInfo(type_facility_data_arr[j]);
            }
            facility_type_dic.Add(type_name, type_facility_data_arr);
        }
    }

    //生成单个设备按钮UI
    void CreateSingleFacilityBtn(facility_info f, GameObject parent)
    {
        FacilityData data = MainController.Instance.GetSingleFacilityData(f.find_index);
        string type_name = data.type;
        GameObject btn_demo = hb_demo;
        switch (type_name)
        {
            case "环保设备":
                btn_demo = hb_demo;
                break;
            case "排污监测设备":
                btn_demo = pwjc_demo;
                break;
            case "开门状态采集设备":
                btn_demo = kmzt_demo;
                break;
            case "画面监控设备":
                btn_demo = hmjk_demo;
                break;
        }
        GameObject go = Instantiate(btn_demo);
        go.transform.SetParent(parent.transform);
        go.name = data.id;
        f.obj_btn = go.transform;
        f.icon = go.GetComponent<Image>();
        //f.icon.sprite = Resources.Load(CommonData.Instance.type_pic_path_dic[type_name]) as Sprite;
        f.name = go.transform.Find("num").GetComponent<Image>();
        //f.name.text = data.deviceName;
        go.GetComponent<Button>().onClick.AddListener(delegate {
            OnFacilityBtnClick(f);
        });
        go.SetActive(true);
    }

    //刷新单个设备按钮的状态信息
    public void RefreshSingleFacilityBtnInfo(facility_info f)
    {
        FacilityData data = MainController.Instance.GetSingleFacilityData(f.find_index);

        //监控视频不刷新
        if (data.refresh_rate < 0f)
            return;

        //检查到刷新时间了没
        float refresh_rate = data.refresh_rate / 1000f; // 文档是毫秒
        //Debug.Log("-----------刷新   "  + "   " + f.last_get_data_time + "  " + Time.realtimeSinceStartup + "  " + (f.last_get_data_time + refresh_rate));
        if (f.last_get_data_time > 0 && Time.realtimeSinceStartup < (f.last_get_data_time + refresh_rate))
        { return; } //没到刷新设备数据的时间
        //Debug.Log("到了刷新时间");
        f.last_get_data_time = Time.realtimeSinceStartup;

        /*string back_topic = string.Format("{0}/{1}", data.facility_code, data.point_code);
        string result = GetFacilityRunningState(back_topic);
        if (f.current_state == result)
        { return; }
        f.current_state = result;*/
        MainController.Instance.NetworkGetFacilityRunningState(f);
    }

    //刷新单个设备按钮的图片 TODO
    void RefreshSingleFacilityBtnSprite(facility_info f) {
        string path = CommonData.Instance.resources_folder_name + "/" + CommonData.Instance.hb_sprite_dic[1];
        Sprite s = MainController.Instance.GetObjectFromResources(path) as Sprite;
        f.icon.sprite = s;
        path = CommonData.Instance.resources_folder_name + "/" + CommonData.Instance.yellow_num_dic[1];
        f.name.sprite = MainController.Instance.GetObjectFromResources(path) as Sprite; ;
    }

    //单个设备按钮的状态信息获取后的回调
    public void OnGetSingleFacilityBtnInfo(string event_name, object udata)
    {
        DataFormat.NewFacilityInfoData facility_data = JsonMapper.ToObject<DataFormat.NewFacilityInfoData>((string)udata);

        string message = facility_data.message;
        Debug.Log(message);
        FacilityData data = MainController.Instance.GetSingleFacilityData(int.Parse(facility_data.find_index));
        facility_info f = facility_type_dic[data.type][int.Parse(facility_data.index)];

        message = "1"; //  TODO
        
        bool is_different = false;//数据是否有变动
        //Debug.Log("单个设备按钮的状态信息获取后的回调111 " + data.name + "   " + f.nong_du + "    " + f.current_state);
        if (data.type == "排污监测设备")
        {
            if (!string.Equals(f.nong_du, message))
            {
                is_different = true;
                f.nong_du = message;
            }
        }
        else
        {
            if (!string.Equals(f.current_state, message))
            {
                is_different = true;
                f.current_state = message;
            }
        }
        if (!is_different) return;
        RefreshSingleFacilityBtnSprite(f);
        //Debug.Log("单个设备按钮的状态信息获取后的回调222 " + data.name + "   " + f.nong_du +  "    "  +  f.current_state);
        /*if (info.activeInHierarchy && last_click_facility_index == f.index)
        { //当前打开了设备小面板,刷新
            t2_content_txt.text = message;
        }*/
        return;
    }

    //点击设备运行状态标签，显示设备信息
    void OnFacilityBtnClick(facility_info f)
    {
        FacilityData data = MainController.Instance.GetSingleFacilityData(f.find_index);
        last_click_facility_index = f.index;
        last_click_facility_type = data.type;
        MainController.Instance.SetCurrentClickFacilityType(last_click_facility_type);
        string name = data.deviceName;
        string run_state = "正常";
        string event_des = "异常";
        string error_time = "2022/04/15";
        string facility_pos = "C路"; 
        string person_name = "杨先生";
        string phone_num = "12345678901";
        string video_path = data.rtsp_addr.Replace("main", "sub");
        string json = CreateJsonFormat.GetFacilityInfoPanelJson(name, run_state, event_des, error_time, facility_pos,
                                                            person_name, phone_num, video_path);
        EventManager.Instance.DispatchEvent("打开设备信息面板", json);

        /*
        if (data.type == "画面监控设备")
        {
            //rtsp视频流
            string video_path = data.rtsp_addr.Replace("main", "sub");
            Debug.Log("频频地址：" + video_path);
            string json = CreateJsonFormat.GetRtspInfoJson(data.deviceName, video_path);
            EventManager.Instance.DispatchEvent("打开监控面板", json);
            //OpenCamPanel(data.deviceName, video_path);
        }
        else
        {
            string json = CreateJsonFormat.GetFacilityInfoPanelJson();
            EventManager.Instance.DispatchEvent("打开设备信息面板", json);

            t1_content_txt.text = data.deviceName;
            if (data.type == "排污监测设备")
            {
                t2_txt.text = "当前浓度：";
                t2_content_txt.text = f.nong_du;
            }
            else
            {
                t2_txt.text = "运行状态：";
                t2_content_txt.text = f.current_state;
            }
            info_bg_btn.SetActive(true);
            info.SetActive(true);
            MoveFacilityInfo();
        }*/
    }


    //显示或者隐藏全部类型的设备按钮
    public void ShowOrHideAllFacilityTypeBtns(string type_name, bool is_show)
    {
        //facility_info[] data_arr = facility_type_dic[type_name];
        if (facility_btn_type_parents[type_name].activeInHierarchy != is_show)
            facility_btn_type_parents[type_name].SetActive(is_show);
    }
    #endregion

    void OnCloseInfoPanel(string event_name, object udata) {
        last_click_facility_index = -1;
        last_click_facility_type = null;
        MainController.Instance.SetCurrentClickFacilityType(last_click_facility_type);
    }

    public Dictionary<string, facility_info[]> GetFacilityTypeDic() {
        return facility_type_dic;
    }


    //移动某个设备类型的按钮(根据设备物体的位置)
    public void MoveSingleFacilityTypeBtns(string type_name)
    {
        facility_info[] data_arr = facility_type_dic[type_name];
        foreach (facility_info fa in data_arr)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(fa.obj.transform.position);
            fa.obj_btn.transform.position = new Vector3(pos.x, pos.y, pos.z);
        }
    }
}

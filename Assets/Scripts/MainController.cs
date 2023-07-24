using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Midea.DigitalTwin;
using BestHTTP.JSON.LitJson;
using System;
using System.Text;
using System.IO;

public class MainController : UnitySingleton<MainController>
{
    private bool is_org_1201 = true;//是否是大良厂区（1201 美芝制冷）

    //private string ip = "10.18.62.24"; // mqtt用
    //private int port = 1883; //mqtt用

    //设备信息
    public List<FacilityData> facility_data_list = new List<FacilityData>();//从csv文件读取的全部设备信息
    public string[] type_names;//全部的设备名字 
    public Dictionary<string, List<int>> facility_type_index_dic = new Dictionary<string, List<int>>();//<设备类型名字，该类型设备在facility_data_list的id>

    //摄像机
    private Camera cam;//摄像机
    private CameraControl cam_control;//摄像机控制脚本

    //刷新设备运行状态的数据
    private float time_interval = -1f; //  每隔多长时间检查每个设备是否需要刷新数据
    private float previous_time = 0f; //上次的时间
    //private int time_interval = 60;//刷新 的时间间隔( 每个设备刷新时间一样的话，用 invokerepeat)

    //private UIController ui_controller;//控制Canvas的脚本

    private string current_click_facility_type;//当前点击的设备类型名字

    private void Start()
    {
        InitConfig();

        InitCam();
        //MqttController.Instance.Connect(ip, port);
        GetFacilityDatas();

        InitRefreshTime();

        InitUIController();
    }

    void InitConfig() {
        string scene_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        is_org_1201 = scene_name == CommonData.Instance.org_1201_scene_name;
        CommonData.Instance.resources_folder_name = is_org_1201 ? CommonData.Instance.org_1201_scene_name : "1202";
    }

    void InitUIController() {
        /*if (ui_controller == null)
        {
            ui_controller = gameObject.AddComponent<UIController>();
            ui_controller.Init(this);
        }*/
        UIController.Instance.Init();
    }

    #region 获取设备信息部分
    //初始化获取设备信息
    void GetFacilityDatas() {
        facility_data_list = GetFacilityDatasByFactory(); // FacilityData.GetConfigDatas<FacilityData>();
        Dictionary<string, int> type_name_dic = new Dictionary<string, int>();
        for (int i = 0; i < facility_data_list.Count; i++) {
            string s = facility_data_list[i].type;
            if (!type_name_dic.ContainsKey(s)) //获取全部设备类型
            {
                type_name_dic.Add(s, i);
            }

            //获取对应类型的设备id
            if (!facility_type_index_dic.ContainsKey(s))
                facility_type_index_dic[s] = new List<int>();
            facility_type_index_dic[s].Add(i);
        }
        type_names = new string[type_name_dic.Count]; // type_name_dic.Count + 1
        //type_names[0] = CommonData.Instance.all_type_name;
        int temp_index = 0;  // 1
        foreach (KeyValuePair<string, int> pair in type_name_dic) { //加入设备类型的名字
            type_names[temp_index] = pair.Key;
            temp_index++;
        }
    }

    //获取某个厂区的全部设备信息
    List<FacilityData> GetFacilityDatasByFactory() {
        List<FacilityData> result_list = new List<FacilityData>();
        List<FacilityData> all_data_list = FacilityData.GetConfigDatas<FacilityData>();
        for (int i = 0; i < all_data_list.Count; i++)
        {
            string orgId = all_data_list[i].orgId;
            if (is_org_1201 && orgId == CommonData.Instance.org_1201_scene_name)
            {
                result_list.Add(all_data_list[i]);
            }
            else if (!is_org_1201 && orgId == CommonData.Instance.org_1201_scene_name)
            {
                result_list.Add(all_data_list[i]);
            }
        }
        return result_list;
    }

    //根据index获取设备数据 
    public FacilityData GetSingleFacilityData(int index) {
        return facility_data_list[index];
    }

    //获取某个类型的全部设备index(可以从facility_data_list直接用来查找数据)
    public facility_info[] GetTypeAllFacilityData(string type_name)
    {
        List<int> index_list = facility_type_index_dic[type_name];
        facility_info[] result = new facility_info[index_list.Count];
        for (int i = 0; i < index_list.Count; i++)
        {
            result[i] = new facility_info
            {
                find_index = index_list[i],
            };
        }
        return result;
    }

    //获取单个设备的运行信息(MQTT)
    string GetFacilityRunningState(string back_topic)
    {
        //string topic = CommonData.Instance.front_topic + back_topic;
        //return MqttController.Instance.Subscribe(topic).ToString();
        return "";
    }

    //获取单个设备的运行信息(HTTP)
    public void NetworkGetFacilityRunningState(facility_info f)
    {
        FacilityData data = GetSingleFacilityData(f.find_index);
        string url = string.Format("{0}{1}", CommonData.Instance.get_facility_info_address, CommonData.Instance.get_facility_info_interface);
        Debug.Log(url);
        string json = CreateJsonFormat.GetFacilityInfoJson(null, null, null, null, null, null, data.orgId);
        //(data.deviceId, data.deviceName, data.tagCode, data.tagName, "1", "1", data.orgId);
        Debug.Log(json);
        Byte[] bytes = Encoding.UTF8.GetBytes(json);
        BestHttp_Unity http_Unity = new BestHttp_Unity(url, bytes, message => {
            Debug.Log(message);

            //for test!!记得删 TODO
            /*string path = Application.dataPath + "/FacilityData.txt";
            if (!File.Exists(path))
            {
                //文本不存在创建文本
                FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);
                sw.Write(message);
                sw.Close();
                fileStream.Close();
            }*/

            //UIController.Instance.OnGetSingleFacilityBtnInfo(message, f);
            string json1 = CreateJsonFormat.GetNewFacilityInfoJson(message, f.index.ToString(), f.find_index.ToString());
            EventManager.Instance.DispatchEvent("更新设备信息", json1);
        });
        http_Unity._Post();

        /*NetworkDownload.Instance.GetMessageWithCallback(url,
            message =>{
                OnGetSingleFacilityBtnInfo(message, f);
            });*/
    }

    #endregion

    #region 摄像机部分（UI按钮跟随移动 ）

    //初始化获取摄像机
    void InitCam() {
        cam = Camera.main.GetComponent<Camera>();
        cam_control = cam.GetComponent<CameraControl>();
        cam_control.SetMotion(true, true, false);
    }

    public void CamControlSetMotion(bool can_move, bool can_zoom, bool can_rotate) {
        cam_control.SetMotion(can_move, can_zoom, can_rotate);
    }

    #endregion

    #region 设备信息的刷新
    private void Update()
    {
        if (!string.IsNullOrEmpty(current_click_facility_type)) //有点击设备类型
        {
            if (cam_control != null && cam_control.CheckIsCamOperating()) // 相机移动
            {
                //EventManager.Instance.DispatchEvent("摄像机移动", null);
                UIController.Instance.MoveFacilityInfo();
                UIController.Instance.MoveAllFacilityBtns();//当前显示的设备按钮的移动 
                UIController.Instance.RefreshAllBuildingUIPos();
            }
        }

        //检查设备数据更新
        if (time_interval > 0 && Time.realtimeSinceStartup >= previous_time + time_interval) {
            previous_time = Time.realtimeSinceStartup;
            RefreshAllFacilityBtnInfos();
        }
    }

    //初始化刷新设备运行状态的 时间 信息
    void InitRefreshTime() {
        /*float min_refresh_time = float.PositiveInfinity;
        for (int i = 0; i < facility_data_list.Count; i++)
        {
            int refresh_rate = facility_data_list[i].refresh_rate;
            if (refresh_rate >= 0f)
            {
                float rate = refresh_rate / 1000f;
                if (rate < min_refresh_time)
                {
                    min_refresh_time = rate;
                }
            }
        }*/
        time_interval = 1f; // min_refresh_time;
        previous_time = Time.realtimeSinceStartup - time_interval; //一开始先刷新一次数据
        Debug.Log("刷新的最小时间：" + time_interval + "；上次刷新的时间：" + previous_time);
    }

    //刷新设备按钮运行信息(全部设备信息统一刷新时间, invokereapting) 
    void RefreshAllFacilityBtnInfos()
    {
        UIController.Instance.RefreshAllFacilityBtnInfos();
    }

    #endregion

    public void SetCurrentClickFacilityType(string type_name) {
        current_click_facility_type = type_name;
    }

    //从Resources文件夹加载资源
    public UnityEngine.Object GetObjectFromResources(string path) {
        return Resources.Load(path);
    }

    private void OnDestroy()
    {
        //MqttController.Instance.Disconnect();
    }
}


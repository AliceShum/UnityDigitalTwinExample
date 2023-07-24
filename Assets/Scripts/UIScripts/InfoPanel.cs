using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.JSON.LitJson;

public class InfoPanel : MonoBehaviour
{
    /*private UIController ui_controller;

    public InfoPanel(UIController ui_con)
    {
        this.ui_controller = ui_con;
    }*/

    private GameObject info_bg_btn;//具体设备信息背景
    public info_panel_ui info;//具体设备信息UI
    public info_panel_ui info_cam;//具体摄像机设备信息UI

    private bool open_panel_is_cam = false;//当前打开的信息面板是设备信息

    private void Start()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        transform.localPosition = Vector3.zero;
        UIController.Instance.InfoPanel = this;
        EventManager.Instance.AddListener("打开设备信息面板", OpenInfoPanel);
        EventManager.Instance.AddListener("更新设备信息", RefreshInfoPanel);

        InitComponent();
    }

    void InitComponent() {
        info_bg_btn = transform.Find("info_bg_btn").gameObject;
        info_bg_btn.GetComponent<Button>().onClick.AddListener(delegate {
            HideInfo();
        });
        info_bg_btn.SetActive(false);

        info = new info_panel_ui();
        info.obj = transform.Find("info").gameObject;
        info.facility_name = transform.Find("info/name/t1_content").GetComponent<Text>();
        info.run_state = transform.Find("info/run_state/t1_content").GetComponent<Text>();
        info.event_des = transform.Find("info/event_des/t1_content").GetComponent<Text>();
        info.error_time = transform.Find("info/error_time/t1_content").GetComponent<Text>();
        info.facility_pos = transform.Find("info/facility_pos/t1_content").GetComponent<Text>();
        info.person_name = transform.Find("info/person_info/name").GetComponent<Text>();
        info.phone_num = transform.Find("info/person_info/phone_num").GetComponent<Text>();
        info.person_head = transform.Find("info/person_info/head").GetComponent<Image>();
        info.obj.SetActive(false);

        info_cam = new info_panel_ui();
        info_cam.obj = transform.Find("info_cam").gameObject;

        info_cam.facility_name = transform.Find("info_cam/name/t1_content").GetComponent<Text>();
        info_cam.run_state = transform.Find("info_cam/run_state/t1_content").GetComponent<Text>();
        info_cam.event_des = transform.Find("info_cam/event_des/t1_content").GetComponent<Text>();
        info_cam.error_time = transform.Find("info_cam/error_time/t1_content").GetComponent<Text>();
        info_cam.facility_pos = transform.Find("info_cam/facility_pos/t1_content").GetComponent<Text>();
        info_cam.person_name = transform.Find("info_cam/person_info/name").GetComponent<Text>();
        info_cam.phone_num = transform.Find("info_cam/person_info/phone_num").GetComponent<Text>();
        info_cam.person_head = transform.Find("info_cam/person_info/head").GetComponent<Image>();
        info_cam.cam_facility_name = transform.Find("info_cam/facility_name").GetComponent<Text>();
        info_cam.cam_time = transform.Find("info_cam/time").GetComponent<Text>();
        info_cam.cam_video_image = transform.Find("info_cam/VideoImage").gameObject;
        info_cam.obj.SetActive(false);
    }

    //隐藏设备信息小面板
    void HideInfo()
    {
        EventManager.Instance.DispatchEvent("关闭设备信息面板", null);
        info.obj.SetActive(false);
        info_cam.obj.SetActive(false);
        info_bg_btn.SetActive(false);
    }

    //打开设备信息小面板
    void OpenInfoPanel(string event_name, object udata) {
        DataFormat.InfoPanelData facility_data = JsonMapper.ToObject<DataFormat.InfoPanelData>((string)udata);
        Debug.Log(facility_data.video_path);
        open_panel_is_cam = !string.IsNullOrEmpty(facility_data.video_path);
        info_panel_ui current_info_obj = open_panel_is_cam ? info_cam : info;
        RefreshInfoPanelTextShow(current_info_obj, facility_data);
        current_info_obj.obj.SetActive(true);
    }

    //刷新设备信息小面板
    void RefreshInfoPanel(string event_name, object udata) {
        if (!info.obj.activeSelf && !info_cam.obj.activeSelf) return;
        info_panel_ui current_info_obj = info;
        //if (info.obj.activeSelf) current_info_obj = info;
        if (info_cam.obj.activeSelf) current_info_obj = info_cam;
        
        DataFormat.NewFacilityInfoData facility_data = JsonMapper.ToObject<DataFormat.NewFacilityInfoData>((string)udata);

        string message = facility_data.message;
        Debug.Log(message);
        FacilityData data = MainController.Instance.GetSingleFacilityData(int.Parse(facility_data.find_index));
        facility_info f = UIController.Instance.GetFacilityTypeDic()[data.type][int.Parse(facility_data.index)];

        //RefreshInfoPanelTextShow(current_info_obj, facility_data); TODO 记得回复 !!!
    }

    //刷新设备信息小面板的信息显示
    void RefreshInfoPanelTextShow(info_panel_ui current_info_obj, DataFormat.InfoPanelData facility_data) {
        current_info_obj.facility_name.text = facility_data.name;
        current_info_obj.run_state.text = facility_data.run_state;
        current_info_obj.event_des.text = facility_data.event_des;
        current_info_obj.error_time.text = facility_data.error_time;
        current_info_obj.facility_pos.text = facility_data.facility_pos;
        current_info_obj.person_name.text = facility_data.person_name;
        current_info_obj.phone_num.text = facility_data.phone_num;
        //current_info_obj.person_head.sprite = 
        if (open_panel_is_cam)
        {
            current_info_obj.cam_facility_name.text = facility_data.name;
            //current_info_obj.cam_time.text = System.DateTime.Now.ToString();
            //current_info_obj.cam_video_image
        }
    }
}

public class info_panel_ui {
    public GameObject obj;
    public Text facility_name;
    public Text run_state;
    public Text event_des;
    public Text error_time;
    public Text facility_pos;
    public Text person_name;
    public Text phone_num;
    public Image person_head;
    public Text cam_facility_name;
    public Text cam_time;
    public GameObject cam_video_image;
}

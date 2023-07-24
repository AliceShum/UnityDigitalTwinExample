using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UIController : UnitySingleton<UIController>
{
    //public MainController MainController;
    public GameObject canvas;

    public MainPanel MainPanel;
    public CamPanel CamPanel;
    public BtnsPanel BtnsPanel;
    public InfoPanel InfoPanel;
    public WarnPanel WarnPanel;

    public void Init() //(MainController controller)
    {
        //this.MainController = controller;
        InitUI();
    }

    void InitUI()
    {
        canvas = GameObject.Find("Canvas");

        string path = string.Format("{0}/Prefabs/", CommonData.Instance.resources_folder_name);
        foreach (KeyValuePair<string, Type> pair in CommonData.Instance.panel_map) {
            GameObject panel_obj = MainController.Instance.GetObjectFromResources(path + pair.Key) as GameObject;
            panel_obj = Instantiate(panel_obj);
            panel_obj.transform.SetParent(canvas.transform);
            Component c = panel_obj.AddComponent(pair.Value);
            //MonoBehaviour mono = Activator.CreateInstance(pair.Value) as MonoBehaviour;
        }
 
    }

    //单个设备按钮的状态信息获取后的回调
    /*public void OnGetSingleFacilityBtnInfo(string message, facility_info f) {
        BtnsPanel.OnGetSingleFacilityBtnInfo(message, f);
    }*/

    public Dictionary<string, facility_info[]> GetFacilityTypeDic()
    {
        return BtnsPanel.GetFacilityTypeDic();
    }

    //刷新设备按钮运行信息(全部设备信息统一刷新时间, invokereapting) 
    public void RefreshAllFacilityBtnInfos()
    {
        //Debug.Log("刷新全部设备信息：" + Time.realtimeSinceStartup);
        foreach (KeyValuePair<string, facility_info[]> pair in GetFacilityTypeDic())
        {
            foreach (facility_info fa in pair.Value)
            {
                /*FacilityData data = GetSingleFacilityData(fa.find_index);
                if (data.refresh_rate >= 0f)
                {  //监控视频不刷新
                    RefreshSingleFacilityBtnInfo(fa);
                }*/
                BtnsPanel.RefreshSingleFacilityBtnInfo(fa);
            }
        }
    }

    public void RefreshAllBuildingUIPos() {
        BtnsPanel.RefreshAllBuildingUIPos();
    }

    public void MoveAllFacilityBtns()
    {
        if (MainPanel.last_click_bottom_index == 0)
        {
            foreach (KeyValuePair<string, facility_info[]> pair in GetFacilityTypeDic())
            {
                 BtnsPanel.MoveSingleFacilityTypeBtns(pair.Key);
            }
        }
        else
        {
            string type_name = MainPanel.last_click_type_name;
            BtnsPanel.MoveSingleFacilityTypeBtns(type_name);
        }
    }

    //info的位置对准设备按钮（根据设备物体的位置）
    public void MoveFacilityInfo()
    {
        if (!InfoPanel.info.obj.activeInHierarchy && !InfoPanel.info_cam.obj.activeInHierarchy)
            return;
        if (BtnsPanel.last_click_facility_index < 0)
            return;
        string type_name = BtnsPanel.last_click_facility_type;// type_names[last_click_type_index];
        GameObject obj = GetFacilityTypeDic()[type_name][BtnsPanel.last_click_facility_index].obj;
        Vector3 pos = Camera.main.WorldToScreenPoint(obj.transform.position);
        if(InfoPanel.info.obj.activeInHierarchy)
            InfoPanel.info.obj.transform.position = new Vector3(pos.x, pos.y, pos.z);
        if (InfoPanel.info_cam.obj.activeInHierarchy)
            InfoPanel.info_cam.obj.transform.position = new Vector3(pos.x, pos.y, pos.z);
    }
}

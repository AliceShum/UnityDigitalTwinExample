using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    /*private UIController ui_controller;

    public MainPanel(UIController ui_con)
    {
        this.ui_controller = ui_con;
    }*/

    //设备总数部分
    private Text total_num;  //总数
    private Text running_num; //顺行设备数
    private Text fixing_num;  //修剪设备数
    private Text ratio_num; //设备运行率

    //底部按钮
    private GameObject[] bottom_btn_on_objs;//底部的三个大类按钮的on Gameobject
    private GameObject type_btn_parent;//底部的四个类型按钮的父物体
    private Dictionary<string, GameObject> type_btn_dic = new Dictionary<string, GameObject>();//底部的四个类型按钮的on Gameobject

    //记录点击过的 
    public int last_click_bottom_index = -1;//上次点击过的底部三大按钮index
    public string last_click_type_name;//上次点击过的底部类型按钮名字

    private void Start()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        transform.localPosition = Vector3.zero;
        UIController.Instance.MainPanel = this;
        EventManager.Instance.AddListener("更新设备信息", RefreshFacilityAmount);

        InitFacilityAmount();
        RefreshFacilityAmount();

        InitBottomBtns();
        OnClickBottomBtn(0);
    }

    #region 设备数量
    void InitFacilityAmount() {
        total_num = transform.Find("facility_count/content/total/num").GetComponent<Text>();
        running_num = transform.Find("facility_count/content/running/num").GetComponent<Text>();
        fixing_num = transform.Find("facility_count/content/fixing/num").GetComponent<Text>();
        ratio_num = transform.Find("facility_count/content/ratio/num").GetComponent<Text>();
    }

    //刷新
    void RefreshFacilityAmount(string event_name = null, object udata = null) {
        total_num.text = "5731";
        running_num.text = "5695";
        fixing_num.text = "36";
        ratio_num.text = "99.37%";
    }
    #endregion

    #region 底部按钮

    //生成底部按钮
    void InitBottomBtns()
    {
        //三个大类
        Transform bottom_btn_parent = transform.Find("bottom_btns");
        bottom_btn_on_objs = new GameObject[bottom_btn_parent.childCount];
        for (int i = 0; i < bottom_btn_parent.childCount; i++) {
            Transform trans = bottom_btn_parent.GetChild(0);
            bottom_btn_on_objs[i] = trans.Find("on").gameObject;
            int temp_index = i;
            trans.GetComponent<Button>().onClick.AddListener(delegate {
                OnClickBottomBtn(temp_index);
            });
        }

        //四个设备类型
        string[] type_names = MainController.Instance.type_names;
        type_btn_parent = transform.Find("facility_type/facility_type_bar").gameObject;
        for (int i = 0; i < type_names.Length; i++)
        {
            GameObject go = type_btn_parent.transform.Find(type_names[i]).gameObject;
            GameObject on = go.transform.Find("on").gameObject;
            type_btn_dic.Add(type_names[i], on);
            on.SetActive(false);

            string temp = type_names[i];
            go.GetComponentInChildren<Button>().onClick.AddListener(delegate {
                OnTypeBtnClick(temp);
            });
            go.SetActive(true);
        }
    }

    //点击了三个大类的按钮
    void OnClickBottomBtn(int index) {
        if (index == last_click_bottom_index) return;
        for (int i = 0; i < bottom_btn_on_objs.Length; i++) {
            bool is_active = i == index;
            bottom_btn_on_objs[i].SetActive(is_active);
            if (i == 0) {
                type_btn_parent.SetActive(is_active);
            }
        }

        if (index == 0) //设备安全
        {
            OnClickFacilitySafetyBtn();
        }
        else if (index == 1)  //人员安全
        { 
            
        }
        else if (index == 2) //环境安全
        {

        }
        last_click_bottom_index = index;
    }

    //点击了 设备安全 按钮
    void OnClickFacilitySafetyBtn() {
        //全部显示
        string[] type_names = MainController.Instance.type_names;
        foreach (KeyValuePair<string, GameObject> pair in type_btn_dic) {
            pair.Value.SetActive(false);
        }

        /*for (int i = 1; i < type_names.Length; i++)
        {
            //if (last_click_type_name != i)
            //{
                ShowOrHideAllFacilityTypeBtns(type_names[i], true);
            //}
        }*/
        last_click_type_name = null;
        DispatchTypeBtnClickEvent("全部");
    }

    //点击了底部按钮
    void OnTypeBtnClick(string type_name)
    {
        //全部显示
        if (type_name == last_click_type_name)
        {       
            OnClickFacilitySafetyBtn();
            return;
        }

        //显示当前选中设备
        /*string[] type_names = MainController.Instance.type_names;
        for (int i = 1; i < type_names.Length; i++)
        {
            if (type_name != type_names[i])
            {
                ShowOrHideAllFacilityTypeBtns(type_names[i], false);
            }
        }
        ShowOrHideAllFacilityTypeBtns(type_name, true);*/

        if (!string.IsNullOrEmpty(last_click_type_name))
            type_btn_dic[last_click_type_name].SetActive(false);
        type_btn_dic[type_name].SetActive(true);
        last_click_type_name = type_name;
        //MainController.Instance.MoveAllFacilityBtns(); //-->event
        DispatchTypeBtnClickEvent(type_name);
    }

    void DispatchTypeBtnClickEvent(string click_type_name) {
        EventManager.Instance.DispatchEvent("点击了设备类型", click_type_name);
    }
    #endregion
}

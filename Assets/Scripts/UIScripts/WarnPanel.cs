using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarnPanel : MonoBehaviour
{
    //private UIController ui_controller;


    //警告面板 
    private bool is_check_warn = false;//是否开始检查警告信息
    private GameObject promptCanvas;//警告面板物体
    private PromptPanel propt_panel_script; //警告面板的脚本


    /*public WarnPanel(UIController ui_con)
    {
        this.ui_controller = ui_con;
    }*/

    private void Start()
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        transform.localPosition = Vector3.zero;
        UIController.Instance.WarnPanel = this;

    }

    #region  警告信息
    //初始化
    void InitPromptCanvas()
    {
        if (promptCanvas == null)
        {
            GameObject obj = Resources.Load("Prefabs/PromptCanvas") as GameObject;
            promptCanvas = Instantiate(obj);
            propt_panel_script = promptCanvas.GetComponentInChildren<PromptPanel>();
            propt_panel_script.InitComponent();
            promptCanvas.SetActive(false);
            DontDestroyOnLoad(promptCanvas);
        }
        /*if (is_check_warn) {
            InvokeRepeating("CheckWarnMessage", 3, 10); //3 秒后调用方法，并且之后每隔 120 秒调用一次
        }*/
    }

    private int index;
    //通过网络获取警告信息 TODO
    public void CheckWarnMessage()  //  string url
    {
        //NetworkDownload.GetInstance().GetMessageWithCallback(url,  FinishDownloadWarnMessage);
        int warn_message_num = 3; // TODO 警告信息的数量
        string[] s = new string[warn_message_num];
        for (int i = 0; i < warn_message_num; i++)
        {
            index += 1;
            s[i] = "有新的警告信息" + index;
        }
        FinishDownloadWarnMessage(s);
    }
    //有警告数据之后的操作 
    void FinishDownloadWarnMessage(string[] message)
    {
        //判断是否有警告信息，获取或者生成PromptCanvas
        /*PromptPanel[] panel_scripts = promptCanvas.GetComponentsInChildren<PromptPanel>();
        foreach (PromptPanel p in panel_scripts){
            p.OnOpenPanelRefreshUI(message);
        }*/
        SetPromptCanvasActive(true);
        propt_panel_script.OnOpenPanelRefreshUI(message);
    }

    //开启或者关闭检测警告信息
    public void SetIsCheckWarn(bool is_check)
    {
        is_check_warn = is_check;
        if (is_check)
        {
            InvokeRepeating("CheckWarnMessage", 5, 180);
        }
        else
        {
            CancelInvoke("CheckWarnMessage");
        }
    }

    //设置promptCanvas显示或者隐藏
    public void SetPromptCanvasActive(bool is_active)
    {
        if (promptCanvas != null && promptCanvas.activeInHierarchy != is_active)
        {
            promptCanvas.SetActive(is_active);
        }
    }

    #endregion

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.JSON.LitJson;

public class CamPanel : MonoBehaviour
{
    //private UIController ui_controller;
    /*public CamPanel(UIController ui_con)
    {
        this.ui_controller = ui_con;
    }*/

    //UMP插件（播放rtsp）
    private UMP.UniversalMediaPlayer ump;
    private Text cam_name;//监控设备 名字

    private void Start() //UniversalMediaPlayer
    {
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
        transform.localPosition = Vector3.zero;
        UIController.Instance.CamPanel = this;
        EventManager.Instance.AddListener("打开监控面板", OpenCamPanel);

        Transform return_btn = transform.Find("return_btn");
        cam_name = return_btn.Find("Text").GetComponent<Text>();
        return_btn.GetComponent<Button>().onClick.AddListener(
            delegate {
                CloseCamPanel();
            });
        InitUMP();
        gameObject.SetActive(false);
    }

    //初始化获取UMP
    void InitUMP()
    {
        GameObject ump_obj = MainController.Instance.GetObjectFromResources("UniversalMediaPlayer") as GameObject;
        ump_obj = Instantiate(ump_obj);
        ump = ump_obj.GetComponent<UMP.UniversalMediaPlayer>();
        if (ump == null)
            Debug.LogError("找不到UMP");
        GameObject video_img = transform.Find("VideoImage").gameObject;
        ump.RenderingObjects = new GameObject[] { video_img };
        ump.AutoPlay = false;
        ump.Loop = false;
    }

    //打开监控设备面板 TODO
    void OpenCamPanel(string event_name, object udata)
    {
        DataFormat.RtspVideoData video_data = JsonMapper.ToObject<DataFormat.RtspVideoData>((string)udata);
        Debug.Log(video_data.cam_name + " 视频地址：" + video_data.video_path);
        this.cam_name.text = video_data.cam_name;
        ump.Path = video_data.video_path;
        ump.Play();
        MainController.Instance.CamControlSetMotion(false, false, false);
        gameObject.SetActive(true);
    }

    //关闭监控设备面板 TODO
    void CloseCamPanel()
    {
        ump.Path = null;
        ump.Stop();
        MainController.Instance.CamControlSetMotion(true, true, true);
        gameObject.SetActive(false);
    }
}

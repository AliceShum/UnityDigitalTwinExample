using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromptPanel : MonoBehaviour
{
    private float fly_message_time = 2f;//每次飞出来的新信息显示多久时间
    private float wait_next_fly_time = 1f;//下一个飞出来中间的间隔时间

    private List<string> warn_message_list = new List<string>();//全部未处理的警告信息
    private List<string> new_warn_message_list = new List<string>();//新接收到的警告信息(先飞出排最后的)

    private GameObject tip_btn;
    private Text tip_btn_txt;//tip_btn的Text
    private GameObject tip_btn_on;//on
    private bool is_open_scroll = false;//是否打开了scroll

    private GameObject warn_scroll;//scroll
    private Transform scroll_content;
    private GameObject scroll_demo;

    private GameObject choose_warn;
    private Text choose_warn_title;
    private Text choose_warn_content;
    private Button finish_btn;//完成按钮
    private Button close_btn;//关闭按钮

    private GameObject warn_fly;
    private Text warn_fly_content;
    private Button handle_btn;//处理按钮 
    private Button ignore_btn;//忽略按钮

    private List<WarnItem> total_item_list = new List<WarnItem>();//生成在scroll的item
    private List<WarnItem> new_item_list = new List<WarnItem>();//新的生成在scroll的item

    void Awake() {
        //DontDestroyOnLoad(this);
    }

    void Start()
    {
        //InitComponent();
    }

    //显示新接收到的警告信息
    public void OnOpenPanelRefreshUI(string[] message) {
        Debug.Log("当前接收到的信息数量："+message.Length);
        if (message == null || message.Length <= 0)
            return;
        AddNewWarnMessageToList(message);
        FlyNewMessage();
        OpenTipBtn();
        //RefreshNotFlyWarn();
    }

    //添加新的警告信息 
    void AddNewWarnMessageToList(string[] message)
    {
        for (int i = 0; i < message.Length; i++)
        {
            //warn_message_list.Add(message[i]);
            new_warn_message_list.Add(message[i]);
            InstantiateNewWarnItem(message[i]);
        }
    }

    public void InitComponent() {
        tip_btn = transform.Find("tip_btn").gameObject;
        tip_btn_txt = transform.Find("tip_btn/Text").GetComponent<Text>();
        tip_btn_on = transform.Find("tip_btn/on").gameObject;
        tip_btn.GetComponent<Button>().onClick.AddListener(delegate
        {
            OpenWarnScroll();
        });
        tip_btn.SetActive(false);

        warn_scroll = transform.Find("scroll").gameObject;
        scroll_content = transform.Find("scroll/Viewport/Content");
        scroll_demo = scroll_content.Find("item").gameObject;
        scroll_demo.SetActive(false);
        warn_scroll.SetActive(false);

        choose_warn = transform.Find("choose_warn").gameObject;
        choose_warn_title = transform.Find("choose_warn/title").GetComponent<Text>();
        choose_warn_content = transform.Find("choose_warn/content").GetComponent<Text>();
        finish_btn = transform.Find("choose_warn/finish_btn").GetComponent<Button>();
        close_btn = transform.Find("choose_warn/close_btn").GetComponent<Button>();
        choose_warn.SetActive(false);
        close_btn.onClick.AddListener(delegate
        {
            choose_warn.SetActive(false);
        });

        warn_fly = transform.Find("warn_fly").gameObject;
        warn_fly_content = transform.Find("warn_fly/content").GetComponent<Text>();
        handle_btn = transform.Find("warn_fly/handle_btn").GetComponent<Button>();
        ignore_btn = transform.Find("warn_fly/ignore_btn").GetComponent<Button>();
        warn_fly.SetActive(false);
        ignore_btn.onClick.AddListener(delegate
        {
            warn_fly.SetActive(false);
        });
    }

    #region warn_fly部分，新警告飞出一会
    //显示新的警告信息
    void FlyNewMessage() {
        StartCoroutine("ShowNewFlyNewMessage");
    }

    //点击 已处理 按钮 TODO
    void OnHandleBtnClick(int index, string message)
    {
        print("点击 已处理 按钮:" + message);
        StopCoroutine("ShowNewFlyNewMessage");
        OnFlyWarnClose(index, false);
        FlyNewMessage();
    }

    IEnumerator ShowNewFlyNewMessage()
    {
        while (new_warn_message_list.Count > 0)
        {
            int index = new_warn_message_list.Count - 1;
            string m = new_warn_message_list[index];
            GetNewFlyNewMessage(index, m);
            yield return new WaitForSeconds(fly_message_time);
            OnFlyWarnClose(index, true);
            yield return new WaitForSeconds(wait_next_fly_time);
        }
    }

    void GetNewFlyNewMessage(int index, string message) {
        warn_fly_content.text = message;
        handle_btn.onClick.RemoveAllListeners();
        handle_btn.onClick.AddListener(delegate
        {
            OnHandleBtnClick(index, message);
        });
        warn_fly.SetActive(true);
    }

    //飞完一个警告信息
    void OnFlyWarnClose(int index, bool is_add_to_list) {
        if (is_add_to_list)
        {
            string m = new_warn_message_list[index];
            warn_message_list.Add(m);
            new_warn_message_list.RemoveAt(index);
            RemoveToTotalItemList(index);
        }
        else {
            //tip_btn数量减一；scroll生成的减去；
            ReduceFinishedWarnItem(index, null);
            new_warn_message_list.RemoveAt(index);
            RefreshNotFlyWarn();
        }
        warn_fly.SetActive(false);
    }
    #endregion

    #region 常驻警告部分
    //刷新UI
    void RefreshNotFlyWarn() {
        OpenTipBtn();
        OpenWarnScroll();
    }

    //刷新tip_btn
    void OpenTipBtn() {
        /*for (int i = 0; i < warn_message_list.Count; i++) {
            Debug.Log(warn_message_list[i]);
        }
        for (int i = 0; i < new_warn_message_list.Count; i++)
        {
            Debug.Log(new_warn_message_list[i]);
        }*/
        int total_count = warn_message_list.Count + new_warn_message_list.Count;
        tip_btn.SetActive(total_count > 0);
        warn_scroll.SetActive(total_count > 0);
        tip_btn_txt.text = string.Format("有新的警告{0}个", total_count);
    }

    //打开/隐藏scroll
    void OpenWarnScroll() {
        int total_count = warn_message_list.Count + new_warn_message_list.Count;
        if (total_count <= 0)
        {
            is_open_scroll = true;//把下面的warn_scroll tip_btn_on关闭
            return;
        }
        is_open_scroll = !is_open_scroll;
        warn_scroll.SetActive(is_open_scroll);
        tip_btn_on.SetActive(is_open_scroll);
    }

    //生成一个新的scroll下item子物体
    void InstantiateNewWarnItem(string message) {
        WarnItem item = new WarnItem();
        item.obj = Instantiate(scroll_demo);
        item.obj.transform.SetParent(scroll_content);
        item.message = message;
        Transform trans = item.obj.transform;
        Text name = trans.Find("name").GetComponent<Text>();
        name.text = item.message;
        Text content = trans.Find("content").GetComponent<Text>();
        content.text = item.message;
        trans.GetComponent<Button>().onClick.AddListener(delegate
        {
            OpenChooseWarn(item);
        });
        item.obj.SetActive(true);
        new_item_list.Add(item);
    }

    //把新的警告信息item放到item_list
    //index:在new_warn_message_list里面的index，同步对应new_item_list
    void RemoveToTotalItemList(int index) {
        WarnItem item = new_item_list[index];
        item.index = total_item_list.Count + 1;
        total_item_list.Add(item);
        new_item_list.RemoveAt(index);
    }

    //减去已生成的item
    void ReduceFinishedWarnItem(int index, WarnItem item)
    {
        if (item != null) //  从choose_warn减去
        {
            Destroy(item.obj);
            for (int i = 0; i < total_item_list.Count; i++) {
                if (total_item_list[i].message == item.message) {
                    total_item_list.RemoveAt(i);
                }
            }
            for (int i = 0; i < new_item_list.Count; i++)
            {
                if (new_item_list[i].message == item.message)
                {
                    new_item_list.RemoveAt(i);
                }
            }
        }
        else //warn_fly从减去
        {
            Destroy(new_item_list[index].obj);
            new_item_list.RemoveAt(index);
        }
    }

    //打开choose_warn
    void OpenChooseWarn(WarnItem item)
    {
        choose_warn.SetActive(true);
        choose_warn_title.text = "警告标题";
        choose_warn_content.text = item.message;
        finish_btn.onClick.RemoveAllListeners();
        finish_btn.onClick.AddListener(delegate 
        {
            choose_warn.SetActive(false);
            //可能还没放进来
            for (int i = 0; i < warn_message_list.Count; i++) {
                if (warn_message_list[i] == item.message){
                    warn_message_list.RemoveAt(i);
                }
            }
            ReduceFinishedWarnItem(0, item);
            OpenTipBtn();
        });
    }

    #endregion
}

public class WarnItem {
    public GameObject obj;
    public int index;
    public string message;//要是唯一的
}

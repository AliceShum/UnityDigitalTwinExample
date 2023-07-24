using BestHTTP.JSON.LitJson;
public class CreateJsonFormat 
{
    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <param name="deviceId">设备编码</param>
    /// <param name="deviceName">设备统称</param>
    /// <param name="tagCode">测点编码</param>
    /// <param name="tagName">测点名称</param>
    /// <param name="pageSize">每页条数</param>
    /// <param name="pageNo">页数</param>
    /// <param name="orgId">组织ID</param>
    /// <param name="parentOrgId">事业部ID(查询集团数据则与orgId都不传)</param>
    /// <returns></returns>
    public static string GetFacilityInfoJson(string deviceId = null, string deviceName = null, string tagCode = null,
                                  string tagName = null, string pageSize = null, string pageNo = null, string orgId = null, string parentOrgId = null)
    {
        JsonData jsonData = new JsonData();
        if (deviceId != null)
            jsonData["deviceId"] = deviceId;
        if (deviceName != null)
            jsonData["deviceName"] = deviceName;
        if (tagCode != null)
            jsonData["tagCode"] = tagCode;
        if (tagName != null)
            jsonData["tagName"] = tagName;
        if (pageSize != null)
            jsonData["pageSize"] = pageSize;
        if (pageNo != null)
            jsonData["pageNo"] = pageNo;
        if(orgId != null)
            jsonData["orgId"] = orgId;
        if (parentOrgId != null)
            jsonData["parentOrgId"] = parentOrgId;
        return jsonData.ToJson();
    }

    /// <summary>
    /// 打开监控面板CamPanel需要传递的信息
    /// </summary>
    /// <param name="cam_name"></param>
    /// <param name="video_path"></param>
    /// <returns></returns>
    public static string GetRtspInfoJson(string cam_name, string video_path) {
        JsonData jsonData = new JsonData();
        if (cam_name != null)
            jsonData["cam_name"] = cam_name;
        if (video_path != null)
            jsonData["video_path"] = video_path;
        return jsonData.ToJson();
    }

    /// <summary>
    /// 打开设备信息面板InfoPanel需要传递的信息
    /// </summary>
    /// <param name="facility_name"></param>
    /// <param name="running_state"></param>
    /// <param name="event_des"></param>
    /// <param name="error_time"></param>
    /// <param name="facility_pos"></param>
    /// <param name="person_name"></param>
    /// <param name="phone_num"></param>
    /// <param name="video_path"></param>
    /// <returns></returns>
    public static string GetFacilityInfoPanelJson(string name, string run_state, string event_des, 
                                       string error_time, string facility_pos, string person_name, string phone_num,
                                       string video_path) {
        JsonData jsonData = new JsonData();
        if (name != null)
            jsonData["name"] = name;
        if (run_state != null)
            jsonData["run_state"] = run_state;
        if (event_des != null)
            jsonData["event_des"] = event_des;
        if (error_time != null)
            jsonData["error_time"] = error_time;
        if (facility_pos != null)
            jsonData["facility_pos"] = facility_pos;
        if(person_name != null)
            jsonData["person_name"] = person_name;
        if (phone_num != null)
            jsonData["phone_num"] = phone_num;
        if (video_path != null)
            jsonData["video_path"] = video_path;
        return jsonData.ToJson();
    }

    /// <summary>
    /// 更新了设备信息时需要传递的信息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="index"></param>
    /// <param name="find_index"></param>
    /// <returns></returns>
    public static string GetNewFacilityInfoJson(string message, string index, string find_index)
    {
        JsonData jsonData = new JsonData();
        if (message != null)
            jsonData["message"] = message;
        if (index != null)
            jsonData["index"] = index;
        if (find_index != null)
            jsonData["find_index"] = find_index;
        return jsonData.ToJson();
    }
}

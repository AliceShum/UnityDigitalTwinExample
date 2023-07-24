using UnityEngine;
using System.Collections;

public partial class FacilityData : GameConfigDataBase
{
	public string id; //id(唯一)
	public string orgId; //组织ID
	public string parentOrgId; //事业部ID
	public string deviceId; //设备编码
	public string deviceName; //设备名称
	public string tagCode; //测点编码
	public string tagName; //测点名称 
	public int refresh_rate; //刷新频率（ms）（监控填-1）
	public string rtsp_addr; //rtsp地址（参考：rtsp://xxxx:xxxx@10.174.61.87:554/xxx/xxx/main/av_stream）
	public string type; //设备类型 
	public string note; //备注
	protected override string getFilePath ()
	{
		return "FacilityData";
	}
}

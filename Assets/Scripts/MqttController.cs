using UnityEngine;
using System.Collections;
using System.Net;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using System;

public class MqttController :Singleton<MqttController> {

	private MqttClient client;

	//连接
	public void Connect(string ip, int port) {
		if (string.IsNullOrEmpty(ip))
			return;

		// create client instance 
		client = new MqttClient(IPAddress.Parse(ip), port , false , null); 
		
		// register to message received 
		client.MqttMsgPublishReceived += MqttMsgPublishReceived; 
		
		string clientId = Guid.NewGuid().ToString();
		client.Connect(clientId);
		Debug.Log("mqtt connect finish");
	}

	//订阅
	public ushort Subscribe(string topic)
	{
		if (string.IsNullOrEmpty(topic))
			return 2;
		Debug.Log("mqtt start Subscribe");
		// subscribe to the topic "/home/temperature" with QoS 2 
		ushort result = client.Subscribe(new string[] { topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });

		Debug.Log("Subscribe Finish:  " + result);
		return result;
	}

	//topic 发布的主题 ；content 发布的内容
	public void Publish(string topic, string content) {
		if (string.IsNullOrEmpty(topic))
			return;
		if (string.IsNullOrEmpty(content))
			return;
		client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(content), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
	}

	//接收到发布的消息
	void MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
	{
		Debug.Log("Topic:" + e.Topic);
		string tmp = System.Text.Encoding.UTF8.GetString(e.Message);
		Debug.Log("Received Message:" + tmp);
	}

	//断开 
	public void Disconnect() {
		if (client != null) {
			client.Disconnect();
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using uPLibrary.Networking.M2Mqtt.Messages;

public class SelectObject : MonoBehaviour
{
    /// <summary>
    /// 服务端订阅的Topic
    /// </summary>
    private string m_Topic;

    void Start()
    {
        m_Topic = SGGameLanguageManager.Instance.GetDescWithKey("TopicSend");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !EventSystem.current.IsPointerOverGameObject())
        {
            CheckClickItem();
        }
    }

    /// <summary>
    /// 检测是否点击到物体
    /// </summary>
    private void CheckClickItem()
    {
        Ray ray;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform != null)
            {
                if (hit.collider.gameObject.tag == "Device")
                {
                    Transform go = hit.collider.transform.parent;
                    string id = go.GetComponent<ClickItemInfo>().m_UniqueID;
                    Debug.Log(go.GetComponent<ClickItemInfo>().m_UniqueID + "点击到的唯一ID" + go.name + m_Topic);
                    //给服务器发送消息
                    ProjectStart.m_Instance.MqttClient.Publish(m_Topic, System.Text.Encoding.UTF8.GetBytes(id), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
                }
            }
        }
    }
}

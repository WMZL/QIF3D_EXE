using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System;
using UnityEngine.SceneManagement;
using LitJson;
using System.Data;

public class ProjectStart : MonoBehaviour
{
    public static ProjectStart m_Instance;
    /// <summary>
    /// IP地址
    /// </summary>
    private string m_IP;
    /// <summary>
    /// 端口号
    /// </summary>
    private int m_Port;
    /// <summary>
    /// 订阅的Topic
    /// </summary>
    private string m_Topic;
    /// <summary>
    /// Mqtt客户端
    /// </summary>
    private MqttClient m_MqttClient;
    /// <summary>
    /// 服务器发送的消息
    /// </summary>
    private string m_RecvMsg;
    /// <summary>
    /// jsondata信息
    /// </summary>
    private JsonData m_RecvJsonData;
    /// <summary>
    /// 测试json类型，后面需要修改
    /// </summary>
    private one m_TestJsonClass;
    /// <summary>
    /// 表示东善桥的唯一ID
    /// </summary>
    private const string m_UINTID = "8177a787a28b4f86a103fac9a023db05";
    /// <summary>
    /// 从数据库读取的所有树节点信息
    /// </summary>
    public List<BllTreeNodeInfo> m_NeedInfo;

    public MqttClient MqttClient { get { return m_MqttClient; } }

    void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
        }
        LoadConfig();
        CreateTreeNodes();
        InitNetInfo();
        Application.runInBackground = true;
        GameObject.DontDestroyOnLoad(this);
    }

    #region MQTT相关代码
    void Update()
    {
        //因为除了C#的东西，mono相关的东西不能在主线程里面处理，所以，设置文字现实的需要这样放在Update中处理
        if (m_RecvMsg != null && m_RecvMsg != "" && m_RecvMsg != string.Empty)
        {
            AnysiceMsg();
            m_RecvMsg = string.Empty;
        }
    }

    /// <summary>
    /// 接收订阅消息
    /// </summary>
    private void OnReceiveMsg(object sender, MqttMsgPublishEventArgs e)
    {
        Debug.Log("接收消息成功：" + Encoding.UTF8.GetString(e.Message));
        m_RecvMsg = Encoding.UTF8.GetString(e.Message);
    }

    /// <summary>
    /// 成功订阅
    /// </summary>
    private void OnSuccess(object sender, MqttMsgSubscribedEventArgs e)
    {
        Debug.Log("订阅成功");
    }

    /// <summary>
    /// 读取相关配置
    /// </summary>
    private void LoadConfig()
    {
        string netconfigpath = Application.streamingAssetsPath + "/NetConfig.txt";
        using (StreamReader fs = new StreamReader(netconfigpath, Encoding.Default))
        {
            string line;
            while ((line = fs.ReadLine()) != null)
            {
                if (line == "" ||
                    line == "\n" ||
                    line == "")
                {
                    continue;
                }

                string[] str = line.Split(' ');
                if (str.Length != 2)
                {
                    continue;
                }

                SGGameLanguageManager.Instance.AddKeyDesc(str[0], str[1]);
            }
        }
    }

    /// <summary>
    /// 初始化MQTT相关信息
    /// </summary>
    private void InitNetInfo()
    {
        m_IP = SGGameLanguageManager.Instance.GetDescWithKey("IP");
        m_Port = int.Parse(SGGameLanguageManager.Instance.GetDescWithKey("Port"));
        m_Topic = SGGameLanguageManager.Instance.GetDescWithKey("TopicRecv");

        if (m_IP == null || m_Port == 0 || m_Topic == null)
        {
            Debug.LogError("未配置IP或端口！！！");
            return;
        }
        m_MqttClient = new MqttClient(m_IP, m_Port, false, null);
        m_MqttClient.MqttMsgPublishReceived += OnReceiveMsg;
        m_MqttClient.MqttMsgSubscribed += OnSuccess;
        string clientid = Guid.NewGuid().ToString();
        m_MqttClient.Connect(clientid);
        //订阅服务器消息
        m_MqttClient.Subscribe(new string[] { m_Topic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    /// <summary>
    /// 解析需要处理的消息
    /// </summary>
    private void AnysiceMsg()
    {
        if (m_RecvMsg == null)
        {
            Debug.LogError("消息错误");
            return;
        }
        try
        {
            m_RecvJsonData = JsonMapper.ToObject(m_RecvMsg);
            if (!m_RecvJsonData.ContainsKey("Type"))
            {
                Debug.LogWarning("发送数据格式有误！！！");
                m_RecvMsg = string.Empty;
                return;
            }
            string type = (string)m_RecvJsonData["Type"];

            switch (type)
            {
                case "one":
                    AnysicsOne();
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("数据解析格式错误" + e);
            m_RecvMsg = string.Empty;
            //throw;
        }
    }

    /// <summary>
    /// 解析Json类型1
    /// </summary>
    private void AnysicsOne()
    {
        m_TestJsonClass = JsonMapper.ToObject<one>(m_RecvMsg);
        //唯一ID
        string id = m_TestJsonClass.ID;

        if (ClickItemManager.Instance.AllClickItem.ContainsKey(id))
        {
            ClickItemInfo clickite = ClickItemManager.Instance.AllClickItem[id];
            clickite.MoveTo();
        }
        else
        {
            Debug.LogError("未配置");
        }
        //是否报警
        int isAlram = m_TestJsonClass.isAlram;
        if (AlramItemManager.Instance.AllAlramItem.ContainsKey(id))
        {
            AlarmEffect clickite = AlramItemManager.Instance.AllAlramItem[id];
            clickite.SetIsOn(isAlram);
        }
        else
        {
            Debug.LogError("未配置");
        }
    }

    //给服务器发送消息  m_MqttClient.Publish(m_Topic, Encoding.UTF8.GetBytes(sendmsg), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);
    #endregion

    #region 数据库相关代码

    /// <summary>
    /// 读取数据库数据
    /// </summary>
    private void CreateTreeNodes()
    {
        BllTreeNodeInfo topTreeNode = new BllTreeNodeInfo();

        List<BllTreeNodeInfo> bllTreeNodes = GetBllTreeNodes();
        List<DevNodeInfo> devNodes = GetDevNodes();

        ///topNodes即为读取的树信息
        m_NeedInfo = bllTreeNodes.FindAll((tn) =>
        {
            return tn.TreeParentID.Equals("0");
        });
        foreach (BllTreeNodeInfo tempNode in m_NeedInfo)
        {
            AddTreeNode(bllTreeNodes, devNodes, tempNode, topTreeNode);
        }
    }

    /// <summary>
    /// 添加节点
    /// </summary>
    /// <param name="bllNodes"></param>
    /// <param name="devNodes"></param>
    /// <param name="child"></param>
    /// <param name="parent"></param>
    private void AddTreeNode(List<BllTreeNodeInfo> bllNodes,
                                    List<DevNodeInfo> devNodes,
                                    BllTreeNodeInfo child,
                                    BllTreeNodeInfo parent)
    {
        parent.Children.Add(child);

        List<BllTreeNodeInfo> findNodes = bllNodes.FindAll((tn) =>
        {
            return tn.TreeParentID.Equals(child.TreeID);
        });

        foreach (BllTreeNodeInfo tempNode in findNodes)
        {
            AddTreeNode(bllNodes, devNodes, tempNode, child);
        }

        //Add DevNodes
        if (findNodes == null || findNodes.Count <= 0)
        {
            //Bind Dev
            if (child.BindType == 2)
            {
                // Add DevNodes
                List<DevNodeInfo> tempDevNodes = devNodes.FindAll((dev) =>
                { return dev.DevID.Equals(child.BindID); });

                child.DevNodes.AddRange(tempDevNodes);
                //devNodes.RemoveAll()
            }
        }

        bllNodes.Remove(child);
    }

    /// <summary>
    /// 读取数据库内容
    /// </summary>
    /// <returns></returns>
    private List<BllTreeNodeInfo> GetBllTreeNodes()
    {
        string strErr = "";
        string strSql = "SELECT treeid,parentid,vc_Name,vc_Code,i_BindType,BindID,i_Sort " +
                       "from m_blltree where unitid='" + m_UINTID + "'";
        DataTable dtBllTree = MysqlDB.Instance.GetDataTable(out strErr, strSql);
        List<BllTreeNodeInfo> lstNodes = new List<BllTreeNodeInfo>();
        if (dtBllTree != null && dtBllTree.Rows.Count > 0)
        {
            for (int i = 0; i < dtBllTree.Rows.Count; i++)
            {
                BllTreeNodeInfo node = new BllTreeNodeInfo()
                {
                    BindID = dtBllTree.Rows[i]["BindID"].ToString() + "",
                    NodeName = dtBllTree.Rows[i]["vc_Name"].ToString() + "",
                    TreeID = dtBllTree.Rows[i]["TreeID"].ToString() + "",
                    TreeParentID = dtBllTree.Rows[i]["parentid"].ToString() + "",
                    //	Sort = int.Parse(dtBllTree.Rows[i]["i_Sort"].ToString()),
                };
                lstNodes.Add(node);
            }
        }
        return lstNodes;
    }

    /// <summary>
    /// 读取数据库内容
    /// </summary>
    /// <returns></returns>
    private List<DevNodeInfo> GetDevNodes()
    {
        string strErr = "";
        string strSql = "select  n.devid,n.nodeid,n.vc_name,n.i_NodeType " +
                        "from m_devnodes n " +
                         "INNER JOIN m_devinfo d on n.devid = d.DevID " +
                         "inner JOIN m_blltree b on b.BindID = d.DevID " +
                         "where b.unitid = '" + m_UINTID + "' and b.i_BindType = 2 " +
                         "order by n.nodeid;";
        DataTable dtDevNodes = MysqlDB.Instance.GetDataTable(out strErr, strSql);
        List<DevNodeInfo> lstNodes = new List<DevNodeInfo>();
        if (dtDevNodes != null && dtDevNodes.Rows.Count > 0)
        {
            for (int i = 0; i < dtDevNodes.Rows.Count; i++)
            {
                DevNodeInfo node = new DevNodeInfo()
                {
                    DevID = dtDevNodes.Rows[i]["DevID"].ToString() + "",
                    Name = dtDevNodes.Rows[i]["vc_name"].ToString() + "",
                    NodeID = dtDevNodes.Rows[i]["NodeID"].ToString() + "",
                    NodeType = int.Parse(dtDevNodes.Rows[i]["i_NodeType"].ToString())
                };
                lstNodes.Add(node);
            }
        }
        return lstNodes;
    }

    #endregion
}

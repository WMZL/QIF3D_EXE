using System.Collections;
using System.Data;
using System;
using MySql.Data.MySqlClient;

public class MysqlDB : SingletonClass<MysqlDB>
{
    //public static string MyConnectionString = "Server=26.47.189.183"
    //  + ";Database=dsa5200"
    //  + ";Uid=root"
    //  + ";Pwd=qif123.,"
    //  + ";Port=3306" + ";charset=utf8";

    private string m_IP;
    private string m_DataBase;
    private string m_UID;
    private string m_PassWord;
    private string m_Port;
    private string m_ChatSet;
    public static string m_MyConnectionString;

    //    public static string MyConnectionString = "Server=172.26.1.128"
    //+ ";Database=dsa5200_0815"
    //+ ";Uid=root"
    //+ ";Pwd=qif123.,"
    //+ ";Port=3306"
    //+ ";charset=utf8";

    /// <summary>
    /// 读取数据库IP端口等配置
    /// </summary>
    private void ReadConfig()
    {
        m_IP = SGGameLanguageManager.Instance.GetDescWithKey("Server");
        m_DataBase = SGGameLanguageManager.Instance.GetDescWithKey("Database");
        m_UID = SGGameLanguageManager.Instance.GetDescWithKey("Uid");
        m_PassWord = SGGameLanguageManager.Instance.GetDescWithKey("Pwd");
        m_Port = SGGameLanguageManager.Instance.GetDescWithKey("DataPort");
        m_ChatSet = SGGameLanguageManager.Instance.GetDescWithKey("Charset");

        m_MyConnectionString = "Server=" + m_IP
+ ";Database=" + m_DataBase
+ ";Uid=" + m_UID
+ ";Pwd=" + m_PassWord
+ ";Port=" + m_Port
+ ";charset=" + m_ChatSet;
    }

    // 读取数据 datatable
    public DataTable GetDataTable(out string sError, string sSQL)
    {
        if (m_MyConnectionString == null || m_MyConnectionString == string.Empty)
        {
            ReadConfig();
        }
        DataTable dt = null;
        sError = string.Empty;
        DataSet ds = null;
        MySqlConnection myConn = null;
        try
        {
            myConn = new MySqlConnection(m_MyConnectionString);
            MySqlCommand myCommand = new MySqlCommand(sSQL, myConn);
            myConn.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(myCommand);
            dt = new DataTable();
            ds = new DataSet();
            adapter.Fill(ds);
            dt = ds.Tables[0];
            myConn.Close();
        }
        catch (Exception ex)
        {
            sError = ex.Message;
            UnityEngine.Debug.Log("错误信息：" + ex.Message);
            //UnityEngine.Debug.Log("SqlError:" + sSQL + ";ErrorMsg:" + sError);
        }
        return dt;
    }

    // 读取数据 dataset
    public static DataSet GetDataSet(out string sError, string sSQL)
    {
        DataSet ds = null;
        sError = string.Empty;

        MySqlConnection myConn = null;
        try
        {
            myConn = new MySqlConnection(m_MyConnectionString);
            MySqlCommand myCmd = new MySqlCommand(sSQL, myConn);
            myConn.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(myCmd);
            ds = new DataSet();
            adapter.Fill(ds);
            myConn.Close();
        }
        catch (Exception ex)
        {
            sError = ex.Message;
        }
        return ds;
    }

    // 取最大的ID
    public Int32 GetMaxID(out string sError, string sKeyField, string sTableName)
    {
        DataTable dt = GetDataTable(out sError, "select IFNULL(max(" + sKeyField + "),0) as MaxID from " + sTableName);
        if (dt != null && dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0][0].ToString());
        }

        return 0;
    }

    //获取某个值的id
    public Int32 GetIDByCondition(out string sError, string Condition, string sTableName)
    {
        string sql = "select id from " + sTableName + " where " + Condition;
        DataTable dt = GetDataTable(out sError, sql);
        if (dt != null && dt.Rows.Count > 0)
        {
            return Convert.ToInt32(dt.Rows[0][0].ToString());
        }

        return 0;
    }

    // 插入，修改，删除，是否使用事务
    public static bool UpdateData(out string sError, string sSQL, bool bUseTransaction = false)
    {
        int iResult = 0;
        sError = string.Empty;

        MySqlConnection myConn = null;

        UnityEngine.Debug.Log("SQLUpdate:" + sSQL);
        if (!bUseTransaction)
        {
            try
            {
                myConn = new MySqlConnection(m_MyConnectionString);
                MySqlCommand myCmd = new MySqlCommand(sSQL, myConn);
                myConn.Open();
                iResult = myCmd.ExecuteNonQuery();
                myConn.Close();
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                iResult = -1;
                UnityEngine.Debug.Log("SQLUpdateError:" + sSQL + ":" + sError);
            }
        }
        else // 使用事务
        {
            MySqlTransaction myTrans = null;
            try
            {
                myConn = new MySqlConnection(m_MyConnectionString);
                myConn.Open();
                myTrans = myConn.BeginTransaction();
                MySqlCommand myCmd = new MySqlCommand(sSQL, myConn);
                myCmd.Transaction = myTrans;
                iResult = myCmd.ExecuteNonQuery();
                myTrans.Commit();
                myConn.Close();
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                iResult = -1;
                UnityEngine.Debug.Log("SQLUpdateError:" + sSQL + ":" + sError);
                myTrans.Rollback();
            }
        }

        return iResult > 0;
    }

    //通过变电站名称获取变电站ID
    public int GetStationIdByStationNameEx(string StationName)
    {
        int nStationID = 0;
        string errMsg;
        string strSql;
        DataTable dt;
        strSql = string.Format("SELECT station_id from ob_d5000_station where station_name_videoplant='{0}'", StationName);
        dt = GetDataTable(out errMsg, strSql);
        if (dt.Rows.Count > 0)
        {
            nStationID = Convert.ToInt32(dt.Rows[0]["station_id"].ToString());
        }
        return nStationID;
    }

    /// <summary>
    /// 根据设备名称和告警状态更新告警状态
    /// </summary>
    /// <param name="strTableName">表名</param>
    /// <param name="strDevName">设备名称</param>
    /// <param name="nWillAlarmStatus">要更新成什么告警状态</param>
    /// <param name="nCurrentAlarmStatus">当前是什么告警状态</param>
    /// <returns></returns>
    public static int UpdateAlarmStatusByDevNameAndAlarmStatus(string strTableName, string strDevName, int nWillAlarmStatus, int nCurrentAlarmStatus)
    {
        string errMsg;
        string strSql = string.Format("update {0} set alarm_status={1} where (device_name='{2}' or baohu_vs_dev='{3}') and alarm_status={4}", strTableName, nWillAlarmStatus, strDevName, strDevName, nCurrentAlarmStatus);
        MysqlDB.UpdateData(out errMsg, strSql, true);
        return 0;
    }
}

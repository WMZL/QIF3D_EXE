using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlramItemManager : Singleton<AlramItemManager>
{
    /// <summary>
    /// 所有的需要报警的物体
    /// </summary>
    private AlarmEffect[] m_AllAlramItemArr;

    private Dictionary<string, AlarmEffect> m_AllAlramItem;

    public Dictionary<string, AlarmEffect> AllAlramItem { get { return m_AllAlramItem; } }

    private void Start()
    {
        m_AllAlramItem = new Dictionary<string, AlarmEffect>();
        GameObject go = GameObject.Find("DSQ");
        m_AllAlramItemArr = go.GetComponentsInChildren<AlarmEffect>();
        foreach (var item in m_AllAlramItemArr)
        {
            if (!m_AllAlramItem.ContainsKey(item.m_UniqueID))
            {
                m_AllAlramItem.Add(item.m_UniqueID, item);
            }
            else
            {
                Debug.LogError("存在相同key值，出现错误！！");
            }
        }
    }
}

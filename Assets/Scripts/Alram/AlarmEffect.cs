using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmEffect : MonoBehaviour
{
    /// <summary>
    /// 当前物体的唯一ID
    /// </summary>
    [HideInInspector]
    public string m_UniqueID;
    /// <summary>
    /// 是否报警
    /// </summary>
    private bool m_IsOn;
    public Material[] m_Material;
    /// <summary>
    /// 闪烁的频率,可以外部调整
    /// </summary>
    public float gapTime = 0.1f;
    private float temp;
    private bool IsDisplay = true;
    private MeshRenderer[] m_SelfMaterials;
    /// <summary>
    /// 原来的材质
    /// </summary>
    private Material m_SoureceMaterial;

    private void Awake()
    {
        m_UniqueID = this.GetComponent<ClickItemInfo>().m_UniqueID;
        m_SelfMaterials = transform.GetComponentsInChildren<MeshRenderer>();
        m_SoureceMaterial = m_SelfMaterials[0].material;
    }

    void Update()
    {
        if (m_IsOn)
        {
            Effect();
        }
    }

    private void Effect()
    {
        temp += Time.deltaTime;
        if (temp >= gapTime)
        {
            if (IsDisplay)
            {
                for (int i = 0; i < m_SelfMaterials.Length; i++)
                {
                    m_SelfMaterials[i].material = m_Material[0];
                }
                //this.GetComponent<MeshRenderer>().material = m_Material[0];
                IsDisplay = false;
                temp = 0;
            }
            else
            {
                //this.GetComponent<MeshRenderer>().material = m_Material[1];
                for (int i = 0; i < m_SelfMaterials.Length; i++)
                {
                    m_SelfMaterials[i].material = m_Material[1];
                }
                IsDisplay = true;
                temp = 0;
            }
        }
    }

    /// <summary>
    /// 设置报警
    /// </summary>
    public void SetIsOn(int ID)
    {
        if (ID == 1)
        {
            m_IsOn = true;
        }
        else
        {
            m_IsOn = false;
            SetSelfMateria();
        }
    }

    /// <summary>
    /// 设置初始材质
    /// </summary>
    private void SetSelfMateria()
    {
        for (int i = 0; i < m_SelfMaterials.Length; i++)
        {
            m_SelfMaterials[i].material = m_SoureceMaterial;
        }
    }
}

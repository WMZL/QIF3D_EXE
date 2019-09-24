using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ClickItemInfo : MonoBehaviour
{
    /// <summary>
    /// 摄像机位置
    /// </summary>
    public Vector3 m_CamPos;
    /// <summary>
    /// 摄像机的旋转
    /// </summary>
    public Vector3 m_Rotation;
    /// <summary>
    /// 唯一的ID
    /// </summary>0
    public string m_UniqueID;

    /// <summary>
    /// 设置摄像机的位置
    /// </summary>
    public void MoveTo()
    {
        GameObject m_ThirdCam = GameObject.Find("Camera_Auto");
        if (m_ThirdCam == null)
        {
            Debug.LogWarning("当前不是自由视角");
            return;
        }
        Quaternion m_InitCamRotate = m_ThirdCam.transform.rotation;
        m_ThirdCam.transform.localEulerAngles = m_InitCamRotate.eulerAngles;
        m_ThirdCam.transform.DOMove(m_CamPos, 2.5f).OnComplete(() =>
        {

            m_ThirdCam.transform.DORotate(m_Rotation, 2f);
        });
    }
}
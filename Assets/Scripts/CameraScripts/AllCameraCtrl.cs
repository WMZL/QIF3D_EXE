using UnityEngine;
using System.Collections;

public class AllCameraCtrl : MonoBehaviour
{
    public static AllCameraCtrl instance;
    public EnumCameraStatus m_CameraStatus;
    private GameObject goAutoCamera;//第三人称视角
    private GameObject goFirstCamera;//第一人称视角
    public GameObject CurrentCamera;
    private Vector3 m_FirstInitPos;

    public GameObject GetAutoCamera()
    {
        return goAutoCamera;
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        m_FirstInitPos = new Vector3(15, 6, -5);
        goAutoCamera = GameObject.Find("Camera_Auto");
        goFirstCamera = GameObject.Find("Camera_First");
        StartAutoCamera();
    }

    void StopAllCamera()
    {
        goAutoCamera.SetActive(false);
        goFirstCamera.SetActive(false);
        m_CameraStatus = EnumCameraStatus.NONE;
    }

    public void StartAutoCamera()
    {
        StopAllCamera();
        goAutoCamera.SetActive(true);
        m_CameraStatus = EnumCameraStatus.AUTO;
        SetCurrentCamera(goAutoCamera);
    }

    void SetCurrentCamera(GameObject obj)
    {
        CurrentCamera = obj;
    }

    public void StartFirstCamera()
    {
        StopAllCamera();
        goFirstCamera.SetActive(true);
        m_CameraStatus = EnumCameraStatus.FIRST;
        SetCurrentCamera(goFirstCamera);
        goFirstCamera.transform.position = m_FirstInitPos;
    }

    /// <summary>
    /// 退出程序
    /// </summary>
    public void CloseApplication()
    {
        Application.Quit();
    }
}

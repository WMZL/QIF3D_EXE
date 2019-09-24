using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class WSADControl : MonoBehaviour
{
    public static WSADControl instance;
    private float rotationX;
    private float rotationY;
    private float minAngle = -80;
    private float maxAngle = 80;
    private float moveSpeed = 20f;
    private Vector3 targetPos;
    private Vector3 targetRotatePos;
    private float timeDown;//鼠标右键单击计时器
    private float timeUp;//鼠标右键单击计时器
    private Vector3 downPos;//按下的位置
    private Vector3 upPos;//抬起的位置

    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        //得到摄像机的初始角度
        rotationX = transform.localEulerAngles.y;
        rotationY = transform.localEulerAngles.x;
    }

    void Update()
    {
        ControlCamera();
    }

    void ControlCamera()
    {
        if (Input.GetMouseButton(1))
        {
            //获取x和y移动的坐标
            rotationX += Input.GetAxis("Mouse X") * Time.deltaTime * 100;
            rotationY -= Input.GetAxis("Mouse Y") * Time.deltaTime * 100;

            //Debug.Log(rotationX + ":" + rotationY);

            rotationY = ClampAngle(rotationY, minAngle, maxAngle);

            transform.rotation = Quaternion.Euler(new Vector3(rotationY, rotationX, 0));//设置绕Z轴旋转为0，保证了垂直方向的不倾斜
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Translate(new Vector3(0, 0, 1) * -moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, 0, 1) * moveSpeed * Time.deltaTime);//deltaTime是距离上一帧的时间，使得不同帧数运行的程序速度相同，与速度有关的都要乘以这个系数.
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, 1) * -moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(1, 0, 0) * -moveSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(1, 0, 0) * moveSpeed * Time.deltaTime);
        }

        //上箭头抬升高度  
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(new Vector3(0, 1, 0) * moveSpeed * Time.deltaTime);
        }

        //上箭头抬升高度  
        if (Input.GetKey(KeyCode.Space))
        {
            //transform.Translate(new Vector3(0, 1, 0) * moveSpeed * Time.deltaTime);
        }

        //下箭头下降高度  
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(new Vector3(0, 1, 0) * -moveSpeed * Time.deltaTime);
        }

        //左箭头向左旋转方向  
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0, 1, 0) * -moveSpeed * Time.deltaTime);
        }

        //右箭头向右旋转方向
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(new Vector3(0, 1, 0) * moveSpeed * Time.deltaTime);
        }
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}

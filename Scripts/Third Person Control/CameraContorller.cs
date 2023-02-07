using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContorller : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;
    [Header("转动速度")]
    public float rotationSpeed = 1;
    [Header("对上下旋转的角度限制")]
    public float min = 4, max = 45;

    private float rotationY,rotationX;

    private void Start()
    {
        //使光标不可见 按下esc可见但不可消除
        Cursor.visible = false;
        //按下esc后再次点击游戏可再次消除光标
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //左右旋转、上下旋转
        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationX += Input.GetAxis("Mouse Y") * rotationSpeed;
        //对上下旋转进行限制
        rotationX = Mathf.Clamp(rotationX, min, max);

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        transform.position = target.position - targetRotation * new Vector3(0, 0, 5);
        transform.rotation = targetRotation;
    }

    //返回x方向的旋转向量到PlayerControl脚本中
    public Quaternion planarRotation => Quaternion.Euler(0, rotationY, 0);
}

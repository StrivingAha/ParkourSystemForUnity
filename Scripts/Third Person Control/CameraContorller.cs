using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContorller : MonoBehaviour
{
    [Header("����Ŀ��")]
    public Transform target;
    [Header("ת���ٶ�")]
    public float rotationSpeed = 1;
    [Header("��������ת�ĽǶ�����")]
    public float min = 4, max = 45;

    private float rotationY,rotationX;

    private void Start()
    {
        //ʹ��겻�ɼ� ����esc�ɼ�����������
        Cursor.visible = false;
        //����esc���ٴε����Ϸ���ٴ��������
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        //������ת��������ת
        rotationY += Input.GetAxis("Mouse X") * rotationSpeed;
        rotationX += Input.GetAxis("Mouse Y") * rotationSpeed;
        //��������ת��������
        rotationX = Mathf.Clamp(rotationX, min, max);

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        transform.position = target.position - targetRotation * new Vector3(0, 0, 5);
        transform.rotation = targetRotation;
    }

    //����x�������ת������PlayerControl�ű���
    public Quaternion planarRotation => Quaternion.Euler(0, rotationY, 0);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("�ƶ��ٶ�")]
    public float moveSpeed = 5;
    [Header("����ת���ٶ�")]
    public float rotationSpeed = 500; //���ٶ�
    [Header("���player�Ƿ��뿪����")]
    public float groundCheckRadius = 0.2f; //�뾶����Ӧ��characterController����еİ뾶һ��
    public Vector3 groundCheckOffest;
    public LayerMask groundLayer;

    private CameraContorller cameraControl;
    private Quaternion targetRotation;
    private Animator animator;
    private CharacterController characterController;
    private bool isGrounded;
    private float ySpeed;
    private bool hasControl;

    private void Awake()
    {
        cameraControl = Camera.main.GetComponent<CameraContorller>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");
        
        float moveAmount = Mathf.Abs(hor) + Mathf.Abs(ver);

        var moveInput = (new Vector3(hor,0,ver)).normalized;
        //ʹ�����ˮƽ�ƶ�ʱҲ�ܿ��������ƶ�:�ƶ�����*��������
        var moveDir = cameraControl.planarRotation * moveInput;

        //��������
        GroundCheck();
        if(isGrounded)
        {
            ySpeed = -0.5f;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDir * moveSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0)
        {
            //���ʹ��CharacterController�������ʹ����move���������ƶ��Դ�ȷ����ײ����ʹ��
            //transform.position += moveDir * moveSpeed * Time.deltaTime;
            //characterController.Move(moveDir * moveSpeed * Time.deltaTime);

            targetRotation = Quaternion.LookRotation(moveDir);
        }

        //���ݲ�ͬ��ֵ�����Ų�ͬ����;ֹͣ����ʱ��������ֹͣ����������ƽ��ֹͣ
        animator.SetFloat("moveAmount",Mathf.Clamp01(moveAmount),0.2f,Time.deltaTime);
        
        //ʹ��תƽ��
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
            rotationSpeed * Time.deltaTime);
    }

    //�������Ƿ��ڵ���
    //ֱ��ʹ��characterController.isGrounded��ʱ����ִ��󣬲���������ɿ�
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffest), 
            groundCheckRadius, groundLayer);
    }

    //���������Ƿ��ڵ����sphere���ӻ�
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffest), groundCheckRadius);
    }

    //ʹ�������ȥ����Ҫ��ʱ�رս�ɫ������
    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if(!hasControl)
        {
            animator.SetFloat("moveAmount",0f);
            targetRotation = transform.rotation;
        }
    }
}

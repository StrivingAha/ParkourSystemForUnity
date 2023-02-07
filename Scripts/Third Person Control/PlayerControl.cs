using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("移动速度")]
    public float moveSpeed = 5;
    [Header("人物转向速度")]
    public float rotationSpeed = 500; //角速度
    [Header("检查player是否离开地面")]
    public float groundCheckRadius = 0.2f; //半径设置应和characterController组件中的半径一致
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
        //使照相机水平移动时也能控制人物移动:移动向量*方向向量
        var moveDir = cameraControl.planarRotation * moveInput;

        //设置重力
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
            //如果使用CharacterController组件，则使用其move方法进行移动以此确保碰撞可以使用
            //transform.position += moveDir * moveSpeed * Time.deltaTime;
            //characterController.Move(moveDir * moveSpeed * Time.deltaTime);

            targetRotation = Quaternion.LookRotation(moveDir);
        }

        //根据不同的值来播放不同动画;停止输入时不会立即停止动画，而会平滑停止
        animator.SetFloat("moveAmount",Mathf.Clamp01(moveAmount),0.2f,Time.deltaTime);
        
        //使旋转平滑
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
            rotationSpeed * Time.deltaTime);
    }

    //检查玩家是否在地面
    //直接使用characterController.isGrounded有时会出现错误，不如物理检测可靠
    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffest), 
            groundCheckRadius, groundLayer);
    }

    //将检查玩家是否在地面的sphere可视化
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffest), groundCheckRadius);
    }

    //使人物踩上去，需要暂时关闭角色控制器
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

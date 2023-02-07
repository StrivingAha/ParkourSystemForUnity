# ParkourSystemForUnity
## 跑酷系统制作学习心得:  
## 学习视频出处[【中文字幕】Unity第三人称跑酷系统实例制作视频教程 RRCG](https://www.bilibili.com/video/BV1Uf4y1f78J/?spm_id_from=333.999.0.0&vd_source=9e072443dd15f74d5ea3f0302db00607)  
## 此教程适合有一定c#和unity基础的人观看  
## 上传了脚本文件和动画文件  
## 下面是全部脚本代码展示： 
### 照相机跟随代码  
```
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
```  
### 第三人称控制  
```
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
```
### 环境检测
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScan : MonoBehaviour
{
    [Header("障碍检测")]
    public Vector3 forwardRayOffest = new Vector3(0, 0.25f, 0);
    public float forwardRayLength = 0.8f;
    public LayerMask ObstacleLayer;
    public float heightRayLength = 5;

    public ObstacleHitData ObstacleCheck()
    {
        //实例化结构体
        var hitData = new ObstacleHitData();

        //进行水平障碍检测 第三个变量为射线碰撞到物体的信息
        var forwardOrigin = transform.position + forwardRayOffest;
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward,
            out hitData.forwardHit, forwardRayLength, ObstacleLayer);

        //打印水平检测射线
        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength,
            (hitData.forwardHitFound) ? Color.red:Color.white);

        //水平检测到之后开始进行障碍高度检测
        if(hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            //需要注意射线要从无碰撞器的地方射到有碰撞器的地方才能有正确的bool值
            //若将第二个变量改为Vector3.up且射线原点为水平与垂直的交点时，bool值为false
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightdHit,
                heightRayLength, ObstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength,
            (hitData.heightHitFound) ? Color.red : Color.white);
        }

        return hitData;
    }
}

//使用结构体进行数据存储
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightdHit;
}


```
### 跑酷动画选择
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourControl : MonoBehaviour
{
    public List<ParkourAction> actionList;
    public float turnSpeed = 1000;

    EnvironmentScan environmentScan;
    Animator animator;
    bool inAction;
    PlayerControl playerControl;

    private void Awake()
    {
        environmentScan = GetComponent<EnvironmentScan>();
        animator = GetComponent<Animator>();
        playerControl = GetComponent<PlayerControl>();
    }

    private void Update()
    {
        if(Input.GetButton("Jump") && !inAction)
        {
            var hitData = environmentScan.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                //遍历跑酷动作列表
                foreach(var action in actionList)
                {
                    //寻找合适的动作
                    if(action.CheckIfPossible(hitData,transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }
        
    }

    //为了在播放stepUp动画之后返回原先动画
    IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;
        playerControl.SetControl(false);

        animator.CrossFade(action.AniName, 0.2f);
        yield return null; //因为crossfade需要时间过渡防止直接跳过过渡时间

        //获取下一个动画的时间长度
        var aniState = animator.GetNextAnimatorStateInfo(0);
        //检测动画名称是否有错
        if (!aniState.IsName(action.AniName))
            Debug.LogError("The Parkour Animation Wrong!");

        //yield return new WaitForSeconds(aniState.length);

        float timer = 0f;
        while(timer <= aniState.length)
        {
            timer += Time.deltaTime;

            //将玩家转向障碍物，要在unity将RotateToObstacle勾选
            if (action.RotateToObstacle)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                    action.TargetRotation, turnSpeed * Time.deltaTime);
            }

            if(action.EnableTargetMatching)
            {
                MatchTarget(action);
            }
                
            yield return null;
        }

        playerControl.SetControl(true);
        inAction = false;
    }

    //使玩家动画与障碍物高度匹配
    void MatchTarget(ParkourAction action)
    {
        //只用匹配一次若以前匹配过则退出
        if(animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, 
            action.MatchBodyPart, new MatchTargetWeightMask(action.MatchDir, 0), 
            action.MatchStartTime, action.MatchTargetTime);
    }
}

```
### 跑酷动作资源创建ScriptableObject类的使用
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Parkour System/New parkour action")]
public class ParkourAction : ScriptableObject
{
    public string aniName;
    public string ObstacleTag;

    public float minHeight;
    public float maxHeight;

    //让玩家转向障碍物
    public bool rotateToObstacle;

    [Header("目标匹配")]
    //进行目标匹配，使动画在不同高度都可使用
    public bool enableTargetMatching = true;
    public AvatarTarget matchBodyPart;
    public float matchStartTime;
    public float matchTargetTime;
    public Vector3 matchDir = new Vector3(0,1,0);

    public Quaternion TargetRotation { get; set; }
    public Vector3 MatchPos { get; set; }

    //筛选使用哪个动作
    public bool CheckIfPossible(ObstacleHitData hitData, Transform Player)
    {
        //check Tag
        //在unity中要将有tag的动画放在没有tag动画之上
        if(!string.IsNullOrEmpty(ObstacleTag) && hitData.forwardHit.transform.tag != ObstacleTag)
            return false;

        //height Tag
        float height = hitData.heightdHit.point.y - Player.position.y;
        if(height < minHeight || height > maxHeight)
            return false;

        //判断是否需要将玩家转向面朝障碍物
        if(rotateToObstacle)
        {
            //旋转到障碍物法线的负方向
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
        }

        //将匹配的位置保存
        if(enableTargetMatching)
        {
            MatchPos = hitData.heightdHit.point;
        }

        return true;
    }

    public string AniName => aniName;
    public bool RotateToObstacle => rotateToObstacle;
    public bool EnableTargetMatching => enableTargetMatching;
    public AvatarTarget MatchBodyPart => matchBodyPart;
    public float MatchStartTime => matchStartTime;
    public float MatchTargetTime => matchTargetTime;
    public Vector3 MatchDir => matchDir;
}

```
### 整体学习下来用了两天，但消化了许久，让我对c#养成了一定的使用规范，同时也让我对unity的控件进行了回顾，是一个非常不错的教学视频

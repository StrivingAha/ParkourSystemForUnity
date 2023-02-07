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

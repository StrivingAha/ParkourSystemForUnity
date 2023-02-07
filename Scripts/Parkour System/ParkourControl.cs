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
                //�����ܿᶯ���б�
                foreach(var action in actionList)
                {
                    //Ѱ�Һ��ʵĶ���
                    if(action.CheckIfPossible(hitData,transform))
                    {
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }
        
    }

    //Ϊ���ڲ���stepUp����֮�󷵻�ԭ�ȶ���
    IEnumerator DoParkourAction(ParkourAction action)
    {
        inAction = true;
        playerControl.SetControl(false);

        animator.CrossFade(action.AniName, 0.2f);
        yield return null; //��Ϊcrossfade��Ҫʱ����ɷ�ֱֹ����������ʱ��

        //��ȡ��һ��������ʱ�䳤��
        var aniState = animator.GetNextAnimatorStateInfo(0);
        //��⶯�������Ƿ��д�
        if (!aniState.IsName(action.AniName))
            Debug.LogError("The Parkour Animation Wrong!");

        //yield return new WaitForSeconds(aniState.length);

        float timer = 0f;
        while(timer <= aniState.length)
        {
            timer += Time.deltaTime;

            //�����ת���ϰ��Ҫ��unity��RotateToObstacle��ѡ
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

    //ʹ��Ҷ������ϰ���߶�ƥ��
    void MatchTarget(ParkourAction action)
    {
        //ֻ��ƥ��һ������ǰƥ������˳�
        if(animator.isMatchingTarget) return;

        animator.MatchTarget(action.MatchPos, transform.rotation, 
            action.MatchBodyPart, new MatchTargetWeightMask(action.MatchDir, 0), 
            action.MatchStartTime, action.MatchTargetTime);
    }
}

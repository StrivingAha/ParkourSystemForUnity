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

    //�����ת���ϰ���
    public bool rotateToObstacle;

    [Header("Ŀ��ƥ��")]
    //����Ŀ��ƥ�䣬ʹ�����ڲ�ͬ�߶ȶ���ʹ��
    public bool enableTargetMatching = true;
    public AvatarTarget matchBodyPart;
    public float matchStartTime;
    public float matchTargetTime;
    public Vector3 matchDir = new Vector3(0,1,0);

    public Quaternion TargetRotation { get; set; }
    public Vector3 MatchPos { get; set; }

    //ɸѡʹ���ĸ�����
    public bool CheckIfPossible(ObstacleHitData hitData, Transform Player)
    {
        //check Tag
        //��unity��Ҫ����tag�Ķ�������û��tag����֮��
        if(!string.IsNullOrEmpty(ObstacleTag) && hitData.forwardHit.transform.tag != ObstacleTag)
            return false;

        //height Tag
        float height = hitData.heightdHit.point.y - Player.position.y;
        if(height < minHeight || height > maxHeight)
            return false;

        //�ж��Ƿ���Ҫ�����ת���泯�ϰ���
        if(rotateToObstacle)
        {
            //��ת���ϰ��﷨�ߵĸ�����
            TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);
        }

        //��ƥ���λ�ñ���
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

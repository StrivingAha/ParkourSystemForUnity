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

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


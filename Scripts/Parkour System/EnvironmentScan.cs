using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentScan : MonoBehaviour
{
    [Header("�ϰ����")]
    public Vector3 forwardRayOffest = new Vector3(0, 0.25f, 0);
    public float forwardRayLength = 0.8f;
    public LayerMask ObstacleLayer;
    public float heightRayLength = 5;

    public ObstacleHitData ObstacleCheck()
    {
        //ʵ�����ṹ��
        var hitData = new ObstacleHitData();

        //����ˮƽ�ϰ���� ����������Ϊ������ײ���������Ϣ
        var forwardOrigin = transform.position + forwardRayOffest;
        hitData.forwardHitFound = Physics.Raycast(forwardOrigin, transform.forward,
            out hitData.forwardHit, forwardRayLength, ObstacleLayer);

        //��ӡˮƽ�������
        Debug.DrawRay(forwardOrigin, transform.forward * forwardRayLength,
            (hitData.forwardHitFound) ? Color.red:Color.white);

        //ˮƽ��⵽֮��ʼ�����ϰ��߶ȼ��
        if(hitData.forwardHitFound)
        {
            var heightOrigin = hitData.forwardHit.point + Vector3.up * heightRayLength;
            //��Ҫע������Ҫ������ײ���ĵط��䵽����ײ���ĵط���������ȷ��boolֵ
            //�����ڶ���������ΪVector3.up������ԭ��Ϊˮƽ�봹ֱ�Ľ���ʱ��boolֵΪfalse
            hitData.heightHitFound = Physics.Raycast(heightOrigin, Vector3.down, out hitData.heightdHit,
                heightRayLength, ObstacleLayer);

            Debug.DrawRay(heightOrigin, Vector3.down * heightRayLength,
            (hitData.heightHitFound) ? Color.red : Color.white);
        }

        return hitData;
    }
}

//ʹ�ýṹ��������ݴ洢
public struct ObstacleHitData
{
    public bool forwardHitFound;
    public bool heightHitFound;
    public RaycastHit forwardHit;
    public RaycastHit heightdHit;
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    Vector3 shakeCameraOrignPos;    //최처 카메라 위치
    bool isShake = false;   // 쉐이크 여부

    void Start()
    {
        shakeCameraOrignPos = this.transform.position;   //쉐이크 카메라 최초 위치 저장
    }

    void LateUpdate()
    {
        
    }

    //카메라 흔들기
    void Shake(float x)
    {
        if (isShake)
        {
            //this.transform.Rotate(Vector3.left, y, Space.Self);
        }
    }
}

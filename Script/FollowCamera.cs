using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Camera mainCamera;       // 시작하자마자 손이 사라지고 씬뷰로 플레이어를 포착하거나 카메라 거리를 늘렸다 줄여야 손이 보이는 버그가 있어 대응책으로 한 코딩.

    private void Awake()
    {
        mainCamera = Camera.main;
    }


    private void Start()
    {
        mainCamera.transform.localPosition = new Vector3(0,0,-1.21f);
        Invoke("Test",0.5f);
    }

    private void Test()
    {
        mainCamera.transform.localPosition = new Vector3(0,0,0);
    }
}

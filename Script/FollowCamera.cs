using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    private Camera mainCamera;       // �������ڸ��� ���� ������� ����� �÷��̾ �����ϰų� ī�޶� �Ÿ��� �÷ȴ� �ٿ��� ���� ���̴� ���װ� �־� ����å���� �� �ڵ�.

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{

    //�ν��Ͻ�
    private static ShakeCamera instance;
    public static ShakeCamera Instance => instance;

    //��������
    private float shakeTime;
    private float shakeIntensity;
    
    private Vector3 offset;

    public ShakeCamera()
    {
        instance = this;
    }

    private void Start()
    {
        offset = Vector3.zero;
    }


    public void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f)  //�ǰݽ� ȭ�� ��鸲 ����
    {
        this.shakeTime = shakeTime;
        this.shakeIntensity = shakeIntensity;

        StopCoroutine("ShakeByPosition");
        StartCoroutine("ShakeByPosition");
    }


    private IEnumerator ShakeByPosition()
    {
        //Vector3 startPosition = transform.localPosition;
        

        while (shakeTime > 0.0f)
        {
            transform.localPosition = transform.localPosition + Random.insideUnitSphere * shakeIntensity;

            shakeTime -= Time.deltaTime;

            yield return null;
        }

        transform.localPosition = offset;
    }       // ȭ�� ��鸲 �ڷ�ƾ


 
}

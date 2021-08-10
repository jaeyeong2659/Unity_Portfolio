using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{

    //인스턴스
    private static ShakeCamera instance;
    public static ShakeCamera Instance => instance;

    //전역변수
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


    public void OnShakeCamera(float shakeTime = 1.0f, float shakeIntensity = 0.1f)  //피격시 화면 흔들림 구현
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
    }       // 화면 흔들림 코루틴


 
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void Start()        // 총알 발사 후 2초 지나면 삭제
    {
        Destroy(gameObject,2f);
    }
    
    private void OnCollisionEnter(Collision collision)          // 바닥 또는 벽에 충돌시 바로 삭제
    {
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}

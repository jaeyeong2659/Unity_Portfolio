using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void Start()        // �Ѿ� �߻� �� 2�� ������ ����
    {
        Destroy(gameObject,2f);
    }
    
    private void OnCollisionEnter(Collision collision)          // �ٴ� �Ǵ� ���� �浹�� �ٷ� ����
    {
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}

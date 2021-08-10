using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventSystem : MonoBehaviour
{
    // ������Ʈ �Ҵ�
    [SerializeField] GameObject player;                     // �÷��̾� ����
    [SerializeField] GameObject startPos;                   // ���� �Ǵ� �ʹ����� ���������� �̵��Ǵ� ����
    [SerializeField] GameObject one_Door;                   // ù��° ��
    [SerializeField] GameObject two_Door;                   // �ι�° ��
    [SerializeField] private AudioClip one_Door_Sound;      // ������ ���� ����
    [SerializeField] private AudioClip two_Door_Sound;      // ö�� ���� ����
    [SerializeField] GameObject[] gate;                     // ����Ʈ Array ����
    [SerializeField] Text misstionText;                     // �̼� ���� ��
    [SerializeField] private GameObject[] Enemy;            // ���� �Ҵ� ����
    [SerializeField] private GameObject zombiePrefab;       // ���� Prefab
    [SerializeField] private GameObject gameClearScreen;    // ���� Ŭ����� ȣ��Ǵ� ȭ��
    [SerializeField] private Text gameClearScreenScore;     // ���� Ŭ����� ȣ��Ǵ� ȭ�鿡 �Ҵ�Ǵ� Text
    [SerializeField] private Text gamePlayTime;             // �� ���� �÷��� �ð�

    // ����� ���� ����
    private AudioSource oneDoorSource;      // ������ ���� ���� �ҽ�
    private AudioSource twoDoorSource;      // ö�� ���� ���� �ҽ�

    // ���� ����
    private bool one_Door_open;          // ù��° �� ���� ����.         * ������ ���� ��� ����
    private bool two_Door_open;          // �ι�° �� ���� ����
    private bool two_Door_key;           // �ι�° �� ���� ȹ�� ���� üũ��
    private bool blue_Potion;            // ����° �� ���� ȹ�� ���� üũ��
    private bool Red_Potion;             // ������ ���� ȹ�� üũ��
    private bool two_Stage_spawn_start;  // Ʈ���� �ߵ��� �ι�° �������� ���� ���
    private bool one_Stage_spawn_start;  // Ʈ���� �ߵ��� ù��° �������� ���� ���

    public float playTime;          // �� �÷��� �ð�
    private float[] spawnDelay;     // ���� ���� ������   * ������������ ���� ������ �ο��� ���� Array�� ���� ����

    private void Start()
    {
        player.transform.position = startPos.transform.position;  // ���۽� �÷��̾ ���������� �ڷ���Ʈ
        oneDoorSource = one_Door.GetComponent<AudioSource>();     // ������ ������Ʈ �ִ� ����ҽ� �Ҵ�
        twoDoorSource = two_Door.GetComponent<AudioSource>();     // ö�� ������Ʈ�� �ִ� ���� �ҽ� �Ҵ�
        spawnDelay = new float[Enemy.Length];                     // ���� ���� ����


    }


    private void Update()
    {
        RestorePlayer();             // �ʹ����� �������� �� ó���������� �ڷ���Ʈ
        DoorDistanceCheack();        // ���� �÷��̾� �Ÿ� üũ 
        DoorChange();                // �� ȸ�� ������ 
        MisstoinTextContent();       // �̼� üũ �� ����
        EnemySpawn();                // ���� ����
        Timmer();                    // ���� ���� ������ üũ
        GameClear();                 // ��� �̼� Ŭ����� Ŭ���� ȭ�� ȣ��
        TotalPlayTime();             // �� �÷��̽ð� üũ
    }

    private void TotalPlayTime()    // �� �÷��̽ð� üũ
    {
        playTime += Time.deltaTime;
    }

    private void GameClear()    // ��� �̼� Ŭ����� Ŭ���� ȭ�� ȣ��
    {
        if(two_Door_key && blue_Potion && Red_Potion && player.GetComponent<PlayerMovement>().AliveZombieCount == 0)
        {
            gameClearScreen.SetActive(true);
            gameClearScreenScore.text = "Zombie Kill Count : "+player.GetComponent<PlayerMovement>().zombieKillCount;
            gamePlayTime.text = "Play Time : "+((int)playTime/60%60 >= 1 ? (int)playTime/60%60 : 00).ToString() + ":" +((int)playTime%60).ToString();
            Destroy(player.gameObject);
        }
    }

    private void Timmer()   // ���� ���� ������ üũ
    {
        for(int i = 0; i < Enemy.Length; i++)
        {
            if(spawnDelay[i] >= 0)
            {
                spawnDelay[i] -= Time.deltaTime;
            }
            else
            {
                spawnDelay[i] = 0f;
            }
        }
    }

    private void EnemySpawn()       // ���� ����
    {
        if (!two_Door_key && !blue_Potion && !Red_Potion && one_Stage_spawn_start)
        {
            for (int i = 0; i < 6; i++)
            {
                if (Vector3.Distance(player.transform.position, Enemy[i].transform.position) <= 50f && spawnDelay[i] == 0f)
                {
                    GameObject zombie = Instantiate(zombiePrefab, Enemy[i].transform.position, Quaternion.identity);
                    spawnDelay[i] = 12f;
                }
            }
        }

        if (two_Door_key && !blue_Potion && !Red_Potion && two_Door_open && two_Stage_spawn_start)
        {
            for (int i = 6; i < 12; i++)
            {
                if (Vector3.Distance(player.transform.position, Enemy[i].transform.position) <= 50f && spawnDelay[i] == 0f)
                {
                    GameObject zombie = Instantiate(zombiePrefab, Enemy[i].transform.position, Quaternion.identity);
                    spawnDelay[i] = 12f;
                }
            }
        }

        if (two_Door_key && blue_Potion && !Red_Potion)
        {
            for (int i = 12; i < 32; i++)
            {
                if (Vector3.Distance(player.transform.position, Enemy[i].transform.position) <= 30f && spawnDelay[i] == 0f)
                {
                    GameObject zombie = Instantiate(zombiePrefab, Enemy[i].transform.position, Quaternion.identity);
                    spawnDelay[i] = 12f;
                }
            }
        }

    }

    private void MisstoinTextContent()  // �̼� üũ �� ����
    {
        two_Door_key = player.GetComponent<PlayerMovement>().one_Potion;
        blue_Potion = player.GetComponent<PlayerMovement>().two_Potion;
        Red_Potion = player.GetComponent<PlayerMovement>().three_Potion;

        if (!two_Door_key && !blue_Potion && !Red_Potion)
        {
            misstionText.text = "���Ͽ� �ִ� '�׸�����'�� ȹ���϶�.";
        }
        if (two_Door_key && !blue_Potion && !Red_Potion)
        {
            misstionText.text = "1���� �ִ� ö���� �����ġ�� �����Ǿ����ϴ�. \nö���� �Ѿ� '�������'�� ȹ�� �Ͻʽÿ�.";
        }
        if (two_Door_key && blue_Potion && !Red_Potion)
        {
            misstionText.text = "�����ߴ� ö�� �ݴ����� '��'�� ��������ϴ�. \n���� �Ѿ� �ֽɺο� �ִ� '��������'�� ȹ�� �Ͻʽÿ�.";
            DestroyGate();
        }
        if (two_Door_key && blue_Potion && Red_Potion)
        {
            misstionText.text = "�������� ���� ��� ����մϴ�. \n ���� ���� ���� �Ͻʽÿ�. \n���� �� : " + player.GetComponent<PlayerMovement>().AliveZombieCount;
        }
    }

    private void DoorDistanceCheack() // ���� �÷��̾� �Ÿ� üũ 
    {
        
                                                                
        if(Vector3.Distance(one_Door.transform.position, player.transform.position) <= 2f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                one_Door_open = !one_Door_open;
                one_Stage_spawn_start = true;
                if(one_Door_open)
                {
                    oneDoorSource.PlayOneShot(one_Door_Sound);
                }
            }
        }

        if(Vector3.Distance(two_Door.transform.position, player.transform.position) <= 3f)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                two_Door_open = !two_Door_open;
                two_Stage_spawn_start = true;
                if(two_Door_open)
                {
                    twoDoorSource.PlayOneShot(two_Door_Sound);
                }
            }
        }
    }

    private void RestorePlayer()    // �ʹ����� �������� �� ó���������� �ڷ���Ʈ
    {
        if (player.transform.position.y <= -35f)
        {
            player.transform.position = startPos.transform.position;
        }
    }

     private void DoorChange()  // �� ȸ�� ������ 
    {
        if (one_Door_open)
        {
            Quaternion targetRotation = Quaternion.Euler(0f,-120f,0f);
            one_Door.transform.localRotation = Quaternion.Slerp(one_Door.transform.localRotation, targetRotation, 2f*Time.deltaTime);
        }
        else if (!one_Door_open)
        {
            Quaternion targetRotation = Quaternion.Euler(0f,0f,0f);
            one_Door.transform.localRotation = Quaternion.Slerp(one_Door.transform.localRotation, targetRotation, 2f*Time.deltaTime);
        }

        
        if (two_Door_open && two_Door_key)
        {
            Quaternion targetRotation = Quaternion.Euler(0f,-60f,0f);
            two_Door.transform.localRotation = Quaternion.Slerp(two_Door.transform.localRotation, targetRotation, 2f*Time.deltaTime);
        }
        else if (!two_Door_open && two_Door_key)
        {
            Quaternion targetRotation = Quaternion.Euler(0f,-180f,0f);
            two_Door.transform.localRotation = Quaternion.Slerp(two_Door.transform.localRotation, targetRotation, 2f*Time.deltaTime);
        }
    }


    public void DestroyGate()   // �ι�° �̼� Ŭ����� ���� ���� �κ� ����
    {
            Destroy(gate[0].gameObject);
            Destroy(gate[1].gameObject);
    }


    public void ReStart()       // �׾��� �� ����� ��ư ������ ȣ��
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }

}

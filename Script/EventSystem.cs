using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EventSystem : MonoBehaviour
{
    // 오브젝트 할당
    [SerializeField] GameObject player;                     // 플레이어 설정
    [SerializeField] GameObject startPos;                   // 시작 또는 맵밖으로 떨어졌을시 이동되는 지점
    [SerializeField] GameObject one_Door;                   // 첫번째 문
    [SerializeField] GameObject two_Door;                   // 두번째 문
    [SerializeField] private AudioClip one_Door_Sound;      // 나무문 오픈 사운드
    [SerializeField] private AudioClip two_Door_Sound;      // 철문 오픈 사운드
    [SerializeField] GameObject[] gate;                     // 게이트 Array 변수
    [SerializeField] Text misstionText;                     // 미션 설명 란
    [SerializeField] private GameObject[] Enemy;            // 좀비 할당 변수
    [SerializeField] private GameObject zombiePrefab;       // 좀비 Prefab
    [SerializeField] private GameObject gameClearScreen;    // 게임 클리어시 호출되는 화면
    [SerializeField] private Text gameClearScreenScore;     // 게임 클리어시 호출되는 화면에 할당되는 Text
    [SerializeField] private Text gamePlayTime;             // 총 게임 플레이 시간

    // 오디오 사운드 선언
    private AudioSource oneDoorSource;      // 나무문 오픈 사운드 소스
    private AudioSource twoDoorSource;      // 철문 오픈 사운드 소스

    // 전역 변수
    private bool one_Door_open;          // 첫번째 문 오픈 변수.         * 열리면 좀비 출몰 시작
    private bool two_Door_open;          // 두번째 문 오픈 변수
    private bool two_Door_key;           // 두번째 문 열쇠 획득 유무 체크용
    private bool blue_Potion;            // 세번째 문 열쇠 획득 유무 체크용
    private bool Red_Potion;             // 마지막 열쇠 획득 체크용
    private bool two_Stage_spawn_start;  // 트리거 발동시 두번째 스테이지 좀비 출몰
    private bool one_Stage_spawn_start;  // 트리거 발동시 첫번째 스테이지 좀비 출몰

    public float playTime;          // 총 플레이 시간
    private float[] spawnDelay;     // 좀비 스폰 딜레이   * 스폰지점마다 개별 딜레이 부여를 위해 Array형 변수 선언

    private void Start()
    {
        player.transform.position = startPos.transform.position;  // 시작시 플레이어를 지정지점에 텔레포트
        oneDoorSource = one_Door.GetComponent<AudioSource>();     // 나무문 오프젝트 있는 사운드소스 할당
        twoDoorSource = two_Door.GetComponent<AudioSource>();     // 철문 오브젝트에 있는 사운드 소스 할당
        spawnDelay = new float[Enemy.Length];                     // 변수 형식 설정


    }


    private void Update()
    {
        RestorePlayer();             // 맵밖으로 떨어졌을 시 처음지점으로 텔레포트
        DoorDistanceCheack();        // 문과 플레이어 거리 체크 
        DoorChange();                // 문 회전 움직임 
        MisstoinTextContent();       // 미션 체크 및 변경
        EnemySpawn();                // 좀비 스폰
        Timmer();                    // 좀비 스폰 딜레이 체크
        GameClear();                 // 모든 미션 클리어시 클리어 화면 호출
        TotalPlayTime();             // 총 플레이시간 체크
    }

    private void TotalPlayTime()    // 총 플레이시간 체크
    {
        playTime += Time.deltaTime;
    }

    private void GameClear()    // 모든 미션 클리어시 클리어 화면 호출
    {
        if(two_Door_key && blue_Potion && Red_Potion && player.GetComponent<PlayerMovement>().AliveZombieCount == 0)
        {
            gameClearScreen.SetActive(true);
            gameClearScreenScore.text = "Zombie Kill Count : "+player.GetComponent<PlayerMovement>().zombieKillCount;
            gamePlayTime.text = "Play Time : "+((int)playTime/60%60 >= 1 ? (int)playTime/60%60 : 00).ToString() + ":" +((int)playTime%60).ToString();
            Destroy(player.gameObject);
        }
    }

    private void Timmer()   // 좀비 스폰 딜레이 체크
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

    private void EnemySpawn()       // 좀비 스폰
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

    private void MisstoinTextContent()  // 미션 체크 및 변경
    {
        two_Door_key = player.GetComponent<PlayerMovement>().one_Potion;
        blue_Potion = player.GetComponent<PlayerMovement>().two_Potion;
        Red_Potion = player.GetComponent<PlayerMovement>().three_Potion;

        if (!two_Door_key && !blue_Potion && !Red_Potion)
        {
            misstionText.text = "지하에 있는 '그린포션'을 획득하라.";
        }
        if (two_Door_key && !blue_Potion && !Red_Potion)
        {
            misstionText.text = "1층에 있는 철문의 잠금장치가 해제되었습니다. \n철문을 넘어 '블루포션'을 획득 하십시오.";
        }
        if (two_Door_key && blue_Potion && !Red_Potion)
        {
            misstionText.text = "개봉했던 철문 반대편의 '벽'이 사라졌습니다. \n벽을 넘어 최심부에 있는 '레드포션'을 획득 하십시오.";
            DestroyGate();
        }
        if (two_Door_key && blue_Potion && Red_Potion)
        {
            misstionText.text = "모든곳에서 좀비가 대거 출몰합니다. \n 좀비를 전부 제거 하십시오. \n좀비 수 : " + player.GetComponent<PlayerMovement>().AliveZombieCount;
        }
    }

    private void DoorDistanceCheack() // 문과 플레이어 거리 체크 
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

    private void RestorePlayer()    // 맵밖으로 떨어졌을 시 처음지점으로 텔레포트
    {
        if (player.transform.position.y <= -35f)
        {
            player.transform.position = startPos.transform.position;
        }
    }

     private void DoorChange()  // 문 회전 움직임 
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


    public void DestroyGate()   // 두번째 미션 클리어시 돌로 막힌 부분 제거
    {
            Destroy(gate[0].gameObject);
            Destroy(gate[1].gameObject);
    }


    public void ReStart()       // 죽었을 시 재시작 버튼 누르면 호출
    {
        SceneManager.LoadScene(gameObject.scene.name);
    }

}

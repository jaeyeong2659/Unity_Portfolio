using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]

public class PlayerMovement : MonoBehaviour
{
    // 오브젝트 할당
    [SerializeField] private float speed = 5f;      // 이동속도 설정
    [SerializeField] private Transform cameraArm;   // 화면 회전 설정
    [SerializeField] private float jumpPower = 6f;  // 점프 파워 설정
    [SerializeField] private Text deathKillCount;   // 좀비 킬 카운트 체크
    [SerializeField] private Transform Carector;    // 움직임 설정
    [SerializeField] private GameObject player;     // 캐릭터 설정
    [SerializeField] private GameObject crosshair;  // 조준점 설정
    [SerializeField] private AudioClip walkSound1;  // 걷는 소리
    [SerializeField] private AudioClip walkSound2;  // 걷는 소리 2
    [SerializeField] private Image bloodScreen;     // 피격시 화면 붉은 색 처리
    [SerializeField] GameObject gameUI;             // 체력 및 탄창 이미지 UI
    [SerializeField] GameObject gameOverScreen;     // 게임 죽음 시 출력 되는 UI
    [SerializeField] private Text gamePlayTime;     // 총 플레이 시간
    private AudioSource source = null;              // 오디오 소스 선언
    private Rigidbody rigid;                        // 리지드 선언
    private Animator anim;                          // 애니메이터 선언
    
    // 변수 선언
    public int currentHealth;       // 현재 체력 설정
    private bool isMove;            // 움직임 체크
    private bool isRun;             // 뛰는 체크
    private bool isJump;            // 점프 체크
    public bool isGround;           // 땅 체크
    private bool isReloding;        // 장전 체크
    private bool stopRun;           // 벽에 충돌시 런 해제
    private float stepDelay;        // 걷는 사운드 딜레이
    private bool stepChk;           // 걷는 사운드 On-off 체크
    public bool one_Potion;         // 1스테이지 열쇠 유무 체크
    public bool two_Potion;         // 2스테이지 열쇠 유무 체크
    public bool three_Potion;       // 3스테이지 열쇠 유무 체크
    public int zombieKillCount =0;  // 좀비 킬 카운트 선언 및 초기화
    public int AliveZombieCount;    // 현재 맵에 존재하는 좀비 수 체크
    private bool isBorder;          // 플레이어 벽 충돌 체크
    Vector3 moveDir;                // 플레이어 움직이는 방향 변수
    //private static int maxHealth = 100;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();          // 리지드 할당
        anim =  GetComponent<Animator>();           // 에니메이터 할당
        source = GetComponent<AudioSource>();       // 오디오 소스 할당
    }

    private void Start()
    {
        currentHealth = 100;                                // 현재 체력 초기화
        stepDelay = (isRun && isGround ? 0.22f : 0.5f);     // 걷는 딜레이 체크
        gameOverScreen.SetActive(false);                    // 게임 오버 UI 초기화
        gameUI.SetActive(true);                             // 플레이 화면 UI 초기화
        Cursor.lockState = CursorLockMode.Locked;           // 플레이 시작 시 마우스 포인터 숨김

    }
    private void FixedUpdate()
    {
        Move();     // 플레이어 움직임 이벤트
    }

    private void Update()
    {
        LookAround();                 // 플레이어 마우스 회전 이벤트
        Jump();                       // 플레이어 점프 이벤트
        test();                       // 'M' 누를시 플레이억 권총 퍼포먼스 이벤트
        #region 조준시 크로스헤어 비활성화      // 크로스 헤어 이벤트
        if (Input.GetMouseButton(1))
        {
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(true);
        }
        #endregion  
        Death();                      // 죽음 이벤트
        stepDelay -= Time.deltaTime;  // 스텝 딜레이 이벤트
    }


    private void Death()    // 죽음 이벤트
    {
        if(currentHealth <= 0)
        {
            gameUI.SetActive(false);
            gameOverScreen.SetActive(true);
            deathKillCount.text = "Zombie Kill Count : "+zombieKillCount;
            Cursor.lockState = CursorLockMode.None;
            zombieKillCount = 0;
            AliveZombieCount = 0;
            float playtime = GameObject.Find("EventSystem").GetComponent<EventSystem>().playTime;
            gamePlayTime.text = "Play Time : "+((int)playtime/60%60 == 0 ? 00 :(int)playtime/60%60).ToString() + ":" +((int)playtime%60).ToString();
            Destroy(gameObject);
        }
    }

    private void test() // 'M' 누를시 플레이억 권총 퍼포먼스 이벤트
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            anim.SetTrigger("test");
        }
    }

    private void Jump() // 플레이어 점프 이벤트
    {
        isJump = Input.GetButtonDown("Jump");

        if (isJump && isGround)
        {
            isGround = false;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Move() // 플레이어 움직임 이벤트
    {
        Vector2 MoveVec = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        isMove = MoveVec.magnitude != 0;
        anim.SetBool("isMove", isMove);
        isRun = Input.GetKey(KeyCode.LeftShift) == true && isMove && !stopRun;
        anim.SetBool("isRun", isRun && isGround);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            moveDir = lookForward * MoveVec.y + lookRight * MoveVec.x;
            if(!isBorder)
            rigid.MovePosition(transform.position + (moveDir * (isRun ? speed + 4f : speed) * Time.deltaTime));
            if (stepDelay <= 0 && isGround)
            {
                isReloding = anim.GetCurrentAnimatorStateInfo(0).IsName("Recharge");
                stepDelay = (isRun && isGround ? 0.22f : 0.5f);
                if (stepChk!)
                {
                    if (!isReloding) source.volume = 1f;
                    stepChk = !stepChk;
                    source.PlayOneShot(walkSound1);
                }
                else
                {
                    if (!isReloding) source.volume = 1f;
                    stepChk = !stepChk;
                    source.PlayOneShot(walkSound2);
                }

            }
        }
    }

    private void LookAround()   // 플레이어 마우스 회전 이벤트
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = player.transform.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z);
        //cameraArm.rotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)  // 바닥 또는 벽 충돌 체크
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGround = true;
        }
        if(collision.gameObject.tag == "Wall")
        {
            stopRun = true;
        }
    }

    private void OnCollisionExit(Collision collision)   // 벽 충돌 체크
    {
        if(collision.gameObject.tag == "Wall")
        {
            stopRun = false;
        }
    }


    public void Beaten(int damege)      // 좀비 피격 당했을 시 이벤트
    {
        StartCoroutine(ShowBloodScreen());
        ShakeCamera.Instance.OnShakeCamera(0.1f, 0.1f);
        currentHealth -= damege;
    }

    IEnumerator ShowBloodScreen()       // 피격 당했을 시 화면 붉은색 처리
    {
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;
    }



    private void OnTriggerEnter(Collider other)     // 각 스테이지 열쇠 획득 이벤트
    {
        if(other.gameObject.tag == "Green_Potion")
        {
            one_Potion = true;
            Destroy(other.transform.parent.parent.gameObject);
        }
        if(other.gameObject.tag == "Blue_Potion")
        {
            two_Potion = true;
            Destroy(other.transform.parent.parent.gameObject);
            GameObject.Find("EventSystem").GetComponent<EventSystem>().DestroyGate();
        }
        if(other.gameObject.tag == "Red_Potion")
        {
            three_Potion = true;
            Destroy(other.transform.parent.parent.gameObject);
        }
    }
  

    
}

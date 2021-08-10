using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AudioSource))]

public class PlayerMovement : MonoBehaviour
{
    // ������Ʈ �Ҵ�
    [SerializeField] private float speed = 5f;      // �̵��ӵ� ����
    [SerializeField] private Transform cameraArm;   // ȭ�� ȸ�� ����
    [SerializeField] private float jumpPower = 6f;  // ���� �Ŀ� ����
    [SerializeField] private Text deathKillCount;   // ���� ų ī��Ʈ üũ
    [SerializeField] private Transform Carector;    // ������ ����
    [SerializeField] private GameObject player;     // ĳ���� ����
    [SerializeField] private GameObject crosshair;  // ������ ����
    [SerializeField] private AudioClip walkSound1;  // �ȴ� �Ҹ�
    [SerializeField] private AudioClip walkSound2;  // �ȴ� �Ҹ� 2
    [SerializeField] private Image bloodScreen;     // �ǰݽ� ȭ�� ���� �� ó��
    [SerializeField] GameObject gameUI;             // ü�� �� źâ �̹��� UI
    [SerializeField] GameObject gameOverScreen;     // ���� ���� �� ��� �Ǵ� UI
    [SerializeField] private Text gamePlayTime;     // �� �÷��� �ð�
    private AudioSource source = null;              // ����� �ҽ� ����
    private Rigidbody rigid;                        // ������ ����
    private Animator anim;                          // �ִϸ����� ����
    
    // ���� ����
    public int currentHealth;       // ���� ü�� ����
    private bool isMove;            // ������ üũ
    private bool isRun;             // �ٴ� üũ
    private bool isJump;            // ���� üũ
    public bool isGround;           // �� üũ
    private bool isReloding;        // ���� üũ
    private bool stopRun;           // ���� �浹�� �� ����
    private float stepDelay;        // �ȴ� ���� ������
    private bool stepChk;           // �ȴ� ���� On-off üũ
    public bool one_Potion;         // 1�������� ���� ���� üũ
    public bool two_Potion;         // 2�������� ���� ���� üũ
    public bool three_Potion;       // 3�������� ���� ���� üũ
    public int zombieKillCount =0;  // ���� ų ī��Ʈ ���� �� �ʱ�ȭ
    public int AliveZombieCount;    // ���� �ʿ� �����ϴ� ���� �� üũ
    private bool isBorder;          // �÷��̾� �� �浹 üũ
    Vector3 moveDir;                // �÷��̾� �����̴� ���� ����
    //private static int maxHealth = 100;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();          // ������ �Ҵ�
        anim =  GetComponent<Animator>();           // ���ϸ����� �Ҵ�
        source = GetComponent<AudioSource>();       // ����� �ҽ� �Ҵ�
    }

    private void Start()
    {
        currentHealth = 100;                                // ���� ü�� �ʱ�ȭ
        stepDelay = (isRun && isGround ? 0.22f : 0.5f);     // �ȴ� ������ üũ
        gameOverScreen.SetActive(false);                    // ���� ���� UI �ʱ�ȭ
        gameUI.SetActive(true);                             // �÷��� ȭ�� UI �ʱ�ȭ
        Cursor.lockState = CursorLockMode.Locked;           // �÷��� ���� �� ���콺 ������ ����

    }
    private void FixedUpdate()
    {
        Move();     // �÷��̾� ������ �̺�Ʈ
    }

    private void Update()
    {
        LookAround();                 // �÷��̾� ���콺 ȸ�� �̺�Ʈ
        Jump();                       // �÷��̾� ���� �̺�Ʈ
        test();                       // 'M' ������ �÷��̾� ���� �����ս� �̺�Ʈ
        #region ���ؽ� ũ�ν���� ��Ȱ��ȭ      // ũ�ν� ��� �̺�Ʈ
        if (Input.GetMouseButton(1))
        {
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(true);
        }
        #endregion  
        Death();                      // ���� �̺�Ʈ
        stepDelay -= Time.deltaTime;  // ���� ������ �̺�Ʈ
    }


    private void Death()    // ���� �̺�Ʈ
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

    private void test() // 'M' ������ �÷��̾� ���� �����ս� �̺�Ʈ
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            anim.SetTrigger("test");
        }
    }

    private void Jump() // �÷��̾� ���� �̺�Ʈ
    {
        isJump = Input.GetButtonDown("Jump");

        if (isJump && isGround)
        {
            isGround = false;
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Move() // �÷��̾� ������ �̺�Ʈ
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

    private void LookAround()   // �÷��̾� ���콺 ȸ�� �̺�Ʈ
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = player.transform.rotation.eulerAngles;

        transform.rotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z);
        //cameraArm.rotation = transform.rotation;
    }

    private void OnCollisionEnter(Collision collision)  // �ٴ� �Ǵ� �� �浹 üũ
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

    private void OnCollisionExit(Collision collision)   // �� �浹 üũ
    {
        if(collision.gameObject.tag == "Wall")
        {
            stopRun = false;
        }
    }


    public void Beaten(int damege)      // ���� �ǰ� ������ �� �̺�Ʈ
    {
        StartCoroutine(ShowBloodScreen());
        ShakeCamera.Instance.OnShakeCamera(0.1f, 0.1f);
        currentHealth -= damege;
    }

    IEnumerator ShowBloodScreen()       // �ǰ� ������ �� ȭ�� ������ ó��
    {
        bloodScreen.color = new Color(1, 0, 0, UnityEngine.Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        bloodScreen.color = Color.clear;
    }



    private void OnTriggerEnter(Collider other)     // �� �������� ���� ȹ�� �̺�Ʈ
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

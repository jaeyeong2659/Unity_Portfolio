using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class ZombieController : MonoBehaviour
{
    // 오프젝트 할당
    [SerializeField] private Collider die_collider;          // 몸통 피격 체크용 콜라이더 
    [SerializeField] private Collider die_head_collider;     // 머리 피격 체크용 콜라이더 
    [SerializeField] private float speed = 4f;               // 좀비 이동 속도 체크
    [SerializeField] float m_angle;                          // 좀비가 바라보는 방향 체크
    [SerializeField] float m_distance;                       // 좀비와 플레이어 거리 체크
    [SerializeField] LayerMask m_layerMask;                  // 플레이어가 시야에 들어왔는지 체크용
    [SerializeField] private AudioClip zombieAttack;         // 좀비 공격 사운드


    // 오브젝트 할당
    private GameObject target;              // 타겟 체크용
    private NavMeshAgent agent;             // 네비게이션 할당
    private Rigidbody rigid;                // 리지드 할당
    private Animator anim;                  // 에니메이터 할당
    private AudioSource source = null;      // 오디오 소스 할당

    // 전역 변수
    private float hp = 100f;            // 좀비 최대 체력
    private int damage = 5;             // 좀비 데미지
    private float attackDelay = 1f;     // 좀비 공격 딜레이
    private bool isAttack;              // 좀비 공격중인지 체크
    private float attckRange;           // 좀비 공격 범위
    public bool detected;               // 좀비 플레이어 발견 체크

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();           // 네비게이션 할당
        rigid = GetComponent<Rigidbody>();              // 리지드 할당
        anim = GetComponent<Animator>();                // 에니메이터 할당
        target = GameObject.FindWithTag("Player");      // 타겟 할당
        source = GetComponent<AudioSource>();           // 오디오 소스 할당

    }

    private void Start()
    {    
        attckRange = agent.stoppingDistance;                        // 좀비 공격 범위 설정
        detected = true;                                            // 타겟 접근 설정
        target.GetComponent<PlayerMovement>().AliveZombieCount++;   // 현재 살아있는 좀비 수 체크
    }

    private void Update()
    {
        Die();                              // 좀비 죽음 이벤트
        Attack();                           // 좀비 공격 이벤트
        attackDelay -= Time.deltaTime;      // 좀비 공격 딜레이 체크
        Sight();                            // 좀비 시야에 타겟 확보 이벤트
        Detected();                         // 좀비가 타겟 발견시 호출 이벤트

    }

    private void Detected()     // 좀비가 타겟 발견시 호출 이벤트
    {
        if (detected)
        {
            transform.LookAt(agent.steeringTarget);
            agent.speed = speed * 1.5f;
            agent.SetDestination(target.transform.position);
            if (!isAttack) { anim.SetBool("isRun", true); }
        }
    }



    private void Sight()    // 좀비 시야에 타겟 확보 이벤트  * 구현 후 쓸모없어진 기능.   좀비 스폰 후 접근시 따라가는게 아닌 스폰하자마자 타겟으로 달려가는걸로 변경
    {
        Collider[] t_cols = Physics.OverlapSphere(transform.position, m_distance, m_layerMask);
        if (t_cols.Length > 0)
        {
            Transform t_tfPlayer = t_cols[0].transform;

            Vector3 t_direction = (t_tfPlayer.position - transform.position).normalized;
            float t_angle = Vector3.Angle(t_direction, transform.forward);
            if (t_angle < m_angle * 0.5f)
            {
                if (Physics.Raycast(transform.position + new Vector3(0f, 1f, 0f), t_direction, out RaycastHit t_hit, m_distance, m_layerMask))
                {

                    if (t_hit.collider.gameObject.tag == "Player")
                    {
                        detected = true;
                    }
                }
            }
        }
    }



    private void DamagedOnHead()        // 머리에 피격시 바로 즉사 
    {
        hp = 0f;
    }

    public void Damaged(int damage)     // 몸통에 피격시 맞은 총알 데미지 만큼 체력 감소
    {
        hp -= damage;
    }

    private void Attack()       // 좀비 공격 이벤트
    {
        isAttack = Vector3.Distance(transform.position, target.transform.position) <= attckRange;
        if (attackDelay <= 0f && isAttack)
        {
            Vector3 TargetAmingPos = (target.transform.position - transform.position).normalized;
            Debug.Log(target.name);

            if (Physics.Raycast(transform.position, TargetAmingPos, out RaycastHit Hitinfo, attckRange, 1<<6))
            {
                Debug.Log(Hitinfo.collider.gameObject.name);
                if (Hitinfo.collider.gameObject.tag == "Player")
                {
                    attackDelay = 1f;
                    anim.SetTrigger("Attack");
                    source.volume = 1;
                    source.PlayOneShot(zombieAttack);
                    PlayerMovement hitPlayer = Hitinfo.collider.gameObject.GetComponent<PlayerMovement>();
                    hitPlayer.Beaten(damage);
                }
            }
        }





    }

    private void Die()  // 좀비 죽음 이벤트
    {
        if (hp <= 0)
        {
            agent.enabled = false;
            attackDelay = 999f;
            if (detected)
            {
                anim.SetTrigger("die");
            }
            else if (!detected)
            {
                anim.SetTrigger("frontdie");
            }
            die_collider.enabled = false;
            die_head_collider.enabled = false;
            Destroy(gameObject, 2f);
        }
    }

    private void OnDestroy()        // 좀비 오브젝트 삭제시  현재 좀비 수 감소와 죽인 좀비 수 증가
    {
        target.GetComponent<PlayerMovement>().AliveZombieCount--;
        target.GetComponent<PlayerMovement>().zombieKillCount++;
    }

}

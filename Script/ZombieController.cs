using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class ZombieController : MonoBehaviour
{
    // ������Ʈ �Ҵ�
    [SerializeField] private Collider die_collider;          // ���� �ǰ� üũ�� �ݶ��̴� 
    [SerializeField] private Collider die_head_collider;     // �Ӹ� �ǰ� üũ�� �ݶ��̴� 
    [SerializeField] private float speed = 4f;               // ���� �̵� �ӵ� üũ
    [SerializeField] float m_angle;                          // ���� �ٶ󺸴� ���� üũ
    [SerializeField] float m_distance;                       // ����� �÷��̾� �Ÿ� üũ
    [SerializeField] LayerMask m_layerMask;                  // �÷��̾ �þ߿� ���Դ��� üũ��
    [SerializeField] private AudioClip zombieAttack;         // ���� ���� ����


    // ������Ʈ �Ҵ�
    private GameObject target;              // Ÿ�� üũ��
    private NavMeshAgent agent;             // �׺���̼� �Ҵ�
    private Rigidbody rigid;                // ������ �Ҵ�
    private Animator anim;                  // ���ϸ����� �Ҵ�
    private AudioSource source = null;      // ����� �ҽ� �Ҵ�

    // ���� ����
    private float hp = 100f;            // ���� �ִ� ü��
    private int damage = 5;             // ���� ������
    private float attackDelay = 1f;     // ���� ���� ������
    private bool isAttack;              // ���� ���������� üũ
    private float attckRange;           // ���� ���� ����
    public bool detected;               // ���� �÷��̾� �߰� üũ

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();           // �׺���̼� �Ҵ�
        rigid = GetComponent<Rigidbody>();              // ������ �Ҵ�
        anim = GetComponent<Animator>();                // ���ϸ����� �Ҵ�
        target = GameObject.FindWithTag("Player");      // Ÿ�� �Ҵ�
        source = GetComponent<AudioSource>();           // ����� �ҽ� �Ҵ�

    }

    private void Start()
    {    
        attckRange = agent.stoppingDistance;                        // ���� ���� ���� ����
        detected = true;                                            // Ÿ�� ���� ����
        target.GetComponent<PlayerMovement>().AliveZombieCount++;   // ���� ����ִ� ���� �� üũ
    }

    private void Update()
    {
        Die();                              // ���� ���� �̺�Ʈ
        Attack();                           // ���� ���� �̺�Ʈ
        attackDelay -= Time.deltaTime;      // ���� ���� ������ üũ
        Sight();                            // ���� �þ߿� Ÿ�� Ȯ�� �̺�Ʈ
        Detected();                         // ���� Ÿ�� �߽߰� ȣ�� �̺�Ʈ

    }

    private void Detected()     // ���� Ÿ�� �߽߰� ȣ�� �̺�Ʈ
    {
        if (detected)
        {
            transform.LookAt(agent.steeringTarget);
            agent.speed = speed * 1.5f;
            agent.SetDestination(target.transform.position);
            if (!isAttack) { anim.SetBool("isRun", true); }
        }
    }



    private void Sight()    // ���� �þ߿� Ÿ�� Ȯ�� �̺�Ʈ  * ���� �� ��������� ���.   ���� ���� �� ���ٽ� ���󰡴°� �ƴ� �������ڸ��� Ÿ������ �޷����°ɷ� ����
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



    private void DamagedOnHead()        // �Ӹ��� �ǰݽ� �ٷ� ��� 
    {
        hp = 0f;
    }

    public void Damaged(int damage)     // ���뿡 �ǰݽ� ���� �Ѿ� ������ ��ŭ ü�� ����
    {
        hp -= damage;
    }

    private void Attack()       // ���� ���� �̺�Ʈ
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

    private void Die()  // ���� ���� �̺�Ʈ
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

    private void OnDestroy()        // ���� ������Ʈ ������  ���� ���� �� ���ҿ� ���� ���� �� ����
    {
        target.GetComponent<PlayerMovement>().AliveZombieCount--;
        target.GetComponent<PlayerMovement>().zombieKillCount++;
    }

}

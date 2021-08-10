using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class BulletController : MonoBehaviour
{
    [SerializeField] private GameObject bullet;                 // �Ѿ� Prefab
    [SerializeField] private Transform bulletPos;               // �Ѿ� �߻� ����
    [SerializeField] private AudioClip handGunSound;            // �Ѿ� �߻� ����
    [SerializeField] private AudioClip reloadSound;             // ���� ����
    [SerializeField] private AudioClip emptyFire;               // ��źâ �߻� ����
    [SerializeField] private GameObject hit_Effect_Prefab;      // �Ѿ� ź������ ����Ʈ
    [SerializeField] private int damage = 25;                   // �Ѿ� ������
    [SerializeField] GameObject idleLightPos;                   // Non-Aming �Ѿ� �߻� ����
    [SerializeField] GameObject amingLightPos;                  // Aming �Ѿ� �߻� ����

    private Animator anim;                  // ���ϸ����� ����
    private AudioSource source = null;      // ����� �ҽ� ����
    private Light idleLight;                // Non-Aming FlashLight ���� ����
    private Light amingLight;               // Aming FlashLight ���� ����

    RaycastHit hit;                 //  �Ѿ˿� ���� ��ü���� ���� ����


    public int currentAmmo;             // ���� �Ѿ� ���� üũ
    public int totalAmmo;               // �� �Ѿ� ���� üũ
    private bool isFire;                // ���� �߻� �̺�Ʈ ������ üũ
    public bool aming;                  // ���������� üũ
    private float timmer;               // �Ѿ� �߻� ������ üũ
    private float reloadTimmer;         // ���� ������ üũ
    private float fireDelay = 0.2f;     // �Ѿ� �߻� ������ ����
    private bool fire;                  // �Ѿ� �߻� üũ
    public bool isReloading;            // ������ üũ
    private bool flashLight;            // �÷��ö���Ʈ On-Off üũ








    private void Awake()
    {
        anim = GetComponent<Animator>();                                // ���ϸ����� �Ҵ�
        source = GetComponent<AudioSource>();                           // ����� �ҽ� �Ҵ�
        idleLight = idleLightPos.gameObject.GetComponent<Light>();      // Non-FlashLifgt üũ
        amingLight = amingLightPos.gameObject.GetComponent<Light>();    // FlashLight üũ
    }

    private void Start()
    {
        currentAmmo = 12;               // ���� �� ���� �Ѿ� ���� �ʱ�ȭ
        totalAmmo = 999;                // ��ü �Ѿ� ���� �ʱ�ȭ
        idleLight.enabled = false;      // �÷��� ����Ʈ �ʱ�ȭ
        amingLight.enabled = false;     // �÷��� ����Ʈ �ʱ�ȭ
    }

    private void Update()
    {
        Fire();                         // �Ѿ� �߻� �̺�Ʈ
        Reload();                       // ���� �̺�Ʈ
        timmer += Time.deltaTime;       // �Ѿ� �߻� ������ �̺�Ʈ
        FlashLight();                   // �÷��� ����Ʈ On-Off �̺�Ʈ


    }

    private void FlashLight()       // �÷��� ����Ʈ On-Off �̺�Ʈ
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashLight = !flashLight;
        }


        if (flashLight)
        {
            if (aming)
            {
                idleLight.enabled = false;
                amingLight.enabled = true;
            }
            else
            {
                idleLight.enabled = true;
                amingLight.enabled = false;

            }
        }

        if (!flashLight)
        {
            idleLight.enabled = false;
            amingLight.enabled = false;
        }

    }

    private void Reload()       // ���� �̺�Ʈ
    {
        bool reload = Input.GetKeyDown(KeyCode.R);

        isReloading = anim.GetCurrentAnimatorStateInfo(0).IsName("Recharge");

        if (reload && !isReloading && currentAmmo != 12)
        {
            isReloading = true;
            anim.SetTrigger("isReload");
            source.volume = 0.15f;
            source.PlayOneShot(reloadSound);
            totalAmmo -= 12 - currentAmmo;
            currentAmmo = 12;
            anim.SetBool("EmptyAmmo",false);
        }
        else if (!isReloading)
        {
            source.volume = 1f;
        }



    }

    private void Fire()     // �Ѿ� �߻� �̺�Ʈ
    {
        isFire = Input.GetButtonDown("Fire1");
        aming = Input.GetButton("Fire2");




        if (isFire && fireDelay < timmer && currentAmmo > 0 && !isReloading)
        {
            timmer = 0f;
            fire = true;
            if (!aming)
            {
                GameObject instantBullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
                Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
                bulletRigid.useGravity = false;
                bulletRigid.velocity = transform.forward * 500;
            }
            source.volume = 1f;
            source.PlayOneShot(handGunSound, 0.33f);

            if (currentAmmo > 0)
            {
                currentAmmo--;
            }

            anim.SetBool("isFire", isFire && fire && !aming);
            anim.SetBool("isAmingFire", aming && isFire && fire);

            if (Physics.Raycast(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), Camera.main.transform.forward, out hit, 100f))
            {
                Debug.Log(hit.collider.gameObject.tag);
                var clone = Instantiate(hit_Effect_Prefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(clone, 0.5f);

                if (hit.collider.gameObject.tag == "Zombie")
                {
                    hit.collider.GetComponent<ZombieController>().detected = true;
                    hit.collider.gameObject.GetComponent<ZombieController>().Damaged(damage);
                }
                if (hit.collider.gameObject.tag == "ZombieHead")
                {
                    hit.collider.gameObject.GetComponentInParent<ZombieController>().gameObject.SendMessage("DamagedOnHead");
                }

            }
        }
        else if(isFire && fireDelay < timmer && currentAmmo <= 0)
        {
            fire = false;
            source.PlayOneShot(emptyFire);
        }

        if(currentAmmo <= 0)
        {
            anim.SetBool("EmptyAmmo",true);
        }


        anim.SetBool("isAming", aming);





    }



}

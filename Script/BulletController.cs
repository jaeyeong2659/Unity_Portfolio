using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class BulletController : MonoBehaviour
{
    [SerializeField] private GameObject bullet;                 // 총알 Prefab
    [SerializeField] private Transform bulletPos;               // 총알 발사 지점
    [SerializeField] private AudioClip handGunSound;            // 총알 발사 사운드
    [SerializeField] private AudioClip reloadSound;             // 장전 사운드
    [SerializeField] private AudioClip emptyFire;               // 빈탄창 발사 사운드
    [SerializeField] private GameObject hit_Effect_Prefab;      // 총알 탄착지점 이펙트
    [SerializeField] private int damage = 25;                   // 총알 데미지
    [SerializeField] GameObject idleLightPos;                   // Non-Aming 총알 발사 지점
    [SerializeField] GameObject amingLightPos;                  // Aming 총알 발사 지점

    private Animator anim;                  // 에니메이터 선언
    private AudioSource source = null;      // 오디오 소스 선언
    private Light idleLight;                // Non-Aming FlashLight 지점 선언
    private Light amingLight;               // Aming FlashLight 지점 선언

    RaycastHit hit;                 //  총알에 맞은 물체정보 변수 선언


    public int currentAmmo;             // 현재 총알 개수 체크
    public int totalAmmo;               // 총 총알 개수 체크
    private bool isFire;                // 현재 발사 이벤트 중인지 체크
    public bool aming;                  // 조준중인지 체크
    private float timmer;               // 총알 발사 딜레이 체크
    private float reloadTimmer;         // 정전 딜레이 체크
    private float fireDelay = 0.2f;     // 총알 발사 딜레이 변수
    private bool fire;                  // 총알 발사 체크
    public bool isReloading;            // 장전중 체크
    private bool flashLight;            // 플레시라이트 On-Off 체크








    private void Awake()
    {
        anim = GetComponent<Animator>();                                // 에니메이터 할당
        source = GetComponent<AudioSource>();                           // 오디오 소스 할당
        idleLight = idleLightPos.gameObject.GetComponent<Light>();      // Non-FlashLifgt 체크
        amingLight = amingLightPos.gameObject.GetComponent<Light>();    // FlashLight 체크
    }

    private void Start()
    {
        currentAmmo = 12;               // 시작 시 현재 총알 개수 초기화
        totalAmmo = 999;                // 전체 총알 개수 초기화
        idleLight.enabled = false;      // 플레시 라이트 초기화
        amingLight.enabled = false;     // 플레시 라이트 초기화
    }

    private void Update()
    {
        Fire();                         // 총알 발사 이벤트
        Reload();                       // 장전 이벤트
        timmer += Time.deltaTime;       // 총알 발사 딜레이 이벤트
        FlashLight();                   // 플레시 라이트 On-Off 이벤트


    }

    private void FlashLight()       // 플레시 라이트 On-Off 이벤트
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

    private void Reload()       // 장전 이벤트
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

    private void Fire()     // 총알 발사 이벤트
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

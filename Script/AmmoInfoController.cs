using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoInfoController : MonoBehaviour
{
    // 오프젝트 할당   
    [SerializeField] private Text currentHealth;        // 현재 체력 
    [SerializeField] private Text ammoCount;            // 현재 총알 갯수

    
    // 스크림트 할당
    BulletController player;            // 플레이어 총알 스크립트 호출
    PlayerMovement playerMovement;      // 플레이어 움직임 스크립트 호출

    // 전역 변수 선언
    public Transform aming;             // 플레이어 조준시 총알이 발사되는 지점 체크

    private bool isReloding;            // 장전중 체크
    private int currentAmmo;            // 현재 총알 개수 체크
    private int MaxAmmo;                // 보유중인 총알 개수 체크


    private void Awake()
    {
        player = GetComponentInParent<BulletController>();          // 플레이어 총알 스크립트 할당
        playerMovement = GetComponentInParent<PlayerMovement>();    // 플레이어 움직임 스크립트 할당
    }

    private void Start()
    {
    }

    private void Update()
    {
        StateAmmo();        // 화면 하단에 현재 총알 개수 와 전체 총알 개수 표시
        StateHealth();      // 화면 하단에 현재 체력 표시

    }

    private void StateHealth()      // 화면 하단에 현재 체력 표시
    {
        currentHealth.text = playerMovement.currentHealth.ToString();
    }

    private void StateAmmo()        // 화면 하단에 현재 총알 개수 와 전체 총알 개수 표시
    {
        currentAmmo = player.currentAmmo;
        MaxAmmo = player.totalAmmo;

        ammoCount.text = currentAmmo +" / " + MaxAmmo;

    }



}

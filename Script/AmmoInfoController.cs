using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoInfoController : MonoBehaviour
{
    // ������Ʈ �Ҵ�   
    [SerializeField] private Text currentHealth;        // ���� ü�� 
    [SerializeField] private Text ammoCount;            // ���� �Ѿ� ����

    
    // ��ũ��Ʈ �Ҵ�
    BulletController player;            // �÷��̾� �Ѿ� ��ũ��Ʈ ȣ��
    PlayerMovement playerMovement;      // �÷��̾� ������ ��ũ��Ʈ ȣ��

    // ���� ���� ����
    public Transform aming;             // �÷��̾� ���ؽ� �Ѿ��� �߻�Ǵ� ���� üũ

    private bool isReloding;            // ������ üũ
    private int currentAmmo;            // ���� �Ѿ� ���� üũ
    private int MaxAmmo;                // �������� �Ѿ� ���� üũ


    private void Awake()
    {
        player = GetComponentInParent<BulletController>();          // �÷��̾� �Ѿ� ��ũ��Ʈ �Ҵ�
        playerMovement = GetComponentInParent<PlayerMovement>();    // �÷��̾� ������ ��ũ��Ʈ �Ҵ�
    }

    private void Start()
    {
    }

    private void Update()
    {
        StateAmmo();        // ȭ�� �ϴܿ� ���� �Ѿ� ���� �� ��ü �Ѿ� ���� ǥ��
        StateHealth();      // ȭ�� �ϴܿ� ���� ü�� ǥ��

    }

    private void StateHealth()      // ȭ�� �ϴܿ� ���� ü�� ǥ��
    {
        currentHealth.text = playerMovement.currentHealth.ToString();
    }

    private void StateAmmo()        // ȭ�� �ϴܿ� ���� �Ѿ� ���� �� ��ü �Ѿ� ���� ǥ��
    {
        currentAmmo = player.currentAmmo;
        MaxAmmo = player.totalAmmo;

        ammoCount.text = currentAmmo +" / " + MaxAmmo;

    }



}

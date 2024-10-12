using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject fireballPrefab; // �߻��� fireball ������
    public Transform firePoint; // fireball �߻� ����
    public float cooltime;
    private float curtime;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public AudioClip shootSound; // �߻� ���� Ŭ��
    public GameManager gameManager; // GameManager ����

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (curtime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
            {
                ShootFireball();
                curtime = cooltime;
            }
        }
        curtime -= Time.deltaTime;
    }

    void ShootFireball()
    {
        GameObject newFireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        Fireball fireballScript = newFireball.GetComponent<Fireball>();
        
        Vector2 direction = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        fireballScript.Initialize(direction);

        // Fireball�� GameManager ����
        fireballScript.gameManager = gameManager;

        // �߻� ���� ���
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}

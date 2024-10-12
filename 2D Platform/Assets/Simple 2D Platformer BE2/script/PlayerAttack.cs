using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject fireballPrefab; // 발사할 fireball 프리팹
    public Transform firePoint; // fireball 발사 지점
    public float cooltime;
    private float curtime;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    public AudioClip shootSound; // 발사 사운드 클립
    public GameManager gameManager; // GameManager 참조

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

        // Fireball에 GameManager 설정
        fireballScript.gameManager = gameManager;

        // 발사 사운드 재생
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
}

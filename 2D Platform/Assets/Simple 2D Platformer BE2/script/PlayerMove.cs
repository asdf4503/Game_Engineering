using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    public float maxSpeed;
    public float jumpPower;
    private int jumpCount = 0; // ���� Ƚ���� �����ϴ� ����
    private bool isGrounded = false; // �÷��̾ ���� �ִ��� ���θ� �����ϴ� ����
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    Animator anim;
    AudioSource audioSource;

    private bool isInvincible = false; // ���� �ð� ���� �浹�� �����ϱ� ���� ����

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < 2)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, 0); // ���� y�� �ӵ��� �ʱ�ȭ�մϴ�.
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetBool("isJumping", true);
            PlaySound("JUMP");
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;

        if (Mathf.Abs(rigid.velocity.x) < 0.3)
            anim.SetBool("isWalking", false);
        else
            anim.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        else if (rigid.velocity.x < maxSpeed * (-1))
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

        if (rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    isGrounded = true;
                    jumpCount = 0;
                    anim.SetBool("isJumping", false);
                }
            }
        }
        else
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Spike") && !isInvincible)
        {
            OnDamaged(collision.transform.position);
            return;
        }

        if (collision.gameObject.CompareTag("Boss") && !isInvincible)
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                //OnBossAttack(collision.transform);
                OnDamaged(collision.transform.position);
            }
            else if (transform.position.y <= collision.transform.position.y)
            {
                OnDamaged(collision.transform.position);
            }
        }

        if (collision.gameObject.tag == "Enemy" && !isInvincible)
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }
            else if (transform.position.y <= collision.transform.position.y)
            {
                OnDamaged(collision.transform.position);
            }
        }
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f); // �÷��̾ �����ϰ� ����ϴ�.
        spriteRenderer.flipY = true; // �÷��̾ �������ϴ�.
        capsuleCollider.enabled = false; // �ݶ��̴��� ��Ȱ��ȭ�մϴ�.

        rigid.velocity = new Vector2(0, 0); // ���� �ӵ��� �ʱ�ȭ�մϴ�.
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse); // �߶��ϴ� ���� ���մϴ�.                         
        Time.timeScale = 0; // ������ ������ŵ�ϴ�.
        PlaySound("DIE");
        spriteRenderer.flipY = false; // �÷��̾ �������ϴ�.
    }

    void OnDamaged(Vector2 targetPos)
    {
        if (!isInvincible)
        {
            StartCoroutine(InvincibilityCoroutine(1.0f)); // 1�� ���� ���� ����
            gameManager.HealthDown();
            gameObject.layer = 11;
            spriteRenderer.color = new Color(1, 1, 1, 0.4f);

            int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

            anim.SetTrigger("doDamaged");
            PlaySound("DAMAGED");
        }
    }

    private IEnumerator InvincibilityCoroutine(float duration)
    {
        isInvincible = true;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        OffDamaged();
    }

    void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnAttack(Transform enemy)
    {
        gameManager.stagePoint += 100;
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        MonsterMove monsterMove = enemy.GetComponent<MonsterMove>();
        monsterMove.OnDamaged();
        PlaySound("ATTACK");
    }

    public void OnBossAttack(Transform enemy)
    {
        BossMove bossMove = enemy.GetComponent<BossMove>();
        bossMove.BossAttack(); // ������ ü���� ���ҽ�ŵ�ϴ�.
        PlaySound("ATTACK");
        rigid.AddForce(Vector2.up * 100, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            bool isBronze = collision.gameObject.name.Contains("BronzeCoin");
            bool isSilver = collision.gameObject.name.Contains("SilverCoin");
            bool isGold = collision.gameObject.name.Contains("GoldCoin");

            if (isBronze)
                gameManager.stagePoint += 50;
            else if (isSilver)
                gameManager.stagePoint += 100;
            else if (isGold)
                gameManager.stagePoint += 300;

            collision.gameObject.SetActive(false);
            PlaySound("ITEM");

            // �������� ���� �� �ݶ��̴��� ��� ��Ȱ��ȭ�Ͽ� �ߺ� ����
            StartCoroutine(DisableCollider(collision));
        }
        else if (collision.gameObject.tag == "Finish")
        {
            if (!collision.gameObject.GetComponent<BoxCollider2D>().enabled) return; 
            gameManager.NextStage();
            PlaySound("FINISH");

            // Finish �������� �ݶ��̴��� ��� ��Ȱ��ȭ�Ͽ� �ߺ� ����
            collision.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    void PlaySound(string action)
    {
        switch (action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    private IEnumerator DisableCollider(Collider2D collider)
    {
        collider.enabled = false;
        yield return new WaitForSeconds(0.1f);
        collider.enabled = true;
    }
}

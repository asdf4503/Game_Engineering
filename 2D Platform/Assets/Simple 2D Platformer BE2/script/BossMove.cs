using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove : MonoBehaviour
{
    public GameManager gameManager;
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulCollider;

    public int nextMove;
    public int maxHealth = 100; // ������ �ִ� ü��
    private int currentHealth; // ������ ���� ü��
    public int CurrentHealth // ������ ���� ü���� �б� ���� ������Ƽ
    {
        get { return currentHealth; }
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulCollider = GetComponent<CapsuleCollider2D>();
        currentHealth = maxHealth; // ������ ü���� �ʱ�ȭ
        Think();

        Invoke("Think", 5);
    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y); // ����ĳ��Ʈ ���� ��ġ
        float rayLength = 2.0f; // ����ĳ��Ʈ ���� ����

        Debug.DrawRay(frontVec, Vector2.down * rayLength, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector2.down, rayLength, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    void Think()
    {
        // 0�� ������ ������ ���� ����
        nextMove = Random.Range(0, 2) == 0 ? -1 : 1;

        anim.SetInteger("Walk Speed", nextMove);
        spriteRenderer.flipX = nextMove == -1;

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        // ������ �ݴ�� ����
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == -1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        capsulCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("DeActive", 2);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }

    public void BossAttack()
    {
        currentHealth -= 10; // ������ ü���� 10 ���ҽ�ŵ�ϴ�.
        if (currentHealth <= 0)
        {
            OnDamaged();
            gameManager.stagePoint += 1000;
        }
    }
}

using UnityEngine;

public class Fireball : MonoBehaviour
{
    public GameManager gameManager;
    public float speed;
    private Vector2 direction;
    private Rigidbody2D rb;
    public AudioClip audioDamaged;
    private AudioSource audioSource;
    private bool hasCollided = false;

    public void Initialize(Vector2 direction)
    {
        this.direction = direction.normalized;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.isKinematic = true; // ������ ������ ���� �ʵ��� ����
        Invoke("DestroyFireball", 2);
    }

    void Update()
    {
        if (!hasCollided) // �浹���� ���� ��쿡�� �̵�
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    void DestroyFireball()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
        {
            Debug.LogError("collision ��ü�� null�Դϴ�.");
            return;
        }

        int platformLayer = LayerMask.NameToLayer("Platform");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int bossLayer = LayerMask.NameToLayer("Boss");

        if (collision.gameObject == null)
        {
            Debug.LogError("collision.gameObject�� null�Դϴ�.");
            return;
        }

        if (collision.gameObject.layer == platformLayer || collision.gameObject.layer == enemyLayer || collision.gameObject.layer == bossLayer)
        {
            if (collision.gameObject.layer == enemyLayer)
            {
                MonsterMove monsterMove = collision.GetComponent<MonsterMove>();
                if (monsterMove != null)
                {
                    monsterMove.OnDamaged(); // OnDamaged ȣ��
                    gameManager.stagePoint += 100; // ���� �߰�
                    PlaySoundAndDestroy();
                }
            }

            if(collision.gameObject.layer == bossLayer)
            {
                BossMove bossMove = collision.GetComponent<BossMove>();
                if (bossMove != null)
                {
                    bossMove.BossAttack();
                    PlaySoundAndDestroy();
                }
            }
            Destroy(gameObject); // fireball ��� �ı�
        }
    }

    void PlaySoundAndDestroy()
    {
        GameObject soundObject = new GameObject("TempAudio");
        AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
        tempAudioSource.clip = audioDamaged;
        tempAudioSource.Play();

        Destroy(soundObject, audioDamaged.length); // ���� ��� �� ������Ʈ �ı�
    }
}

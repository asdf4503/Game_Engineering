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
        rb.isKinematic = true; // 물리적 영향을 받지 않도록 설정
        Invoke("DestroyFireball", 2);
    }

    void Update()
    {
        if (!hasCollided) // 충돌하지 않은 경우에만 이동
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
            Debug.LogError("collision 객체가 null입니다.");
            return;
        }

        int platformLayer = LayerMask.NameToLayer("Platform");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        int bossLayer = LayerMask.NameToLayer("Boss");

        if (collision.gameObject == null)
        {
            Debug.LogError("collision.gameObject가 null입니다.");
            return;
        }

        if (collision.gameObject.layer == platformLayer || collision.gameObject.layer == enemyLayer || collision.gameObject.layer == bossLayer)
        {
            if (collision.gameObject.layer == enemyLayer)
            {
                MonsterMove monsterMove = collision.GetComponent<MonsterMove>();
                if (monsterMove != null)
                {
                    monsterMove.OnDamaged(); // OnDamaged 호출
                    gameManager.stagePoint += 100; // 점수 추가
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
            Destroy(gameObject); // fireball 즉시 파괴
        }
    }

    void PlaySoundAndDestroy()
    {
        GameObject soundObject = new GameObject("TempAudio");
        AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();
        tempAudioSource.clip = audioDamaged;
        tempAudioSource.Play();

        Destroy(soundObject, audioDamaged.length); // 사운드 재생 후 오브젝트 파괴
    }
}

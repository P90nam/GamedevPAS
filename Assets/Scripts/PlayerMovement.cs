using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float kecepatan = 5f;
    private Vector2 arahGerak;
    public int skor = 0;

    [Header("Combat Settings")]
    [SerializeField] private Transform attackPoint; // Titik ujung pedang
    [SerializeField] private float attackRange = 0.5f; // Jarak jangkauan tebasan
    [SerializeField] private LayerMask enemyLayers; // Layer khusus musuh
    [SerializeField] private int attackDamage = 1;

    // Referensi Komponen
    private Rigidbody2D rb; // <-- Variabel ini yang sebelumnya belum ada
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; // buat player ga muter pas kena tabrakan
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnMove(InputValue value)
    {
        arahGerak = value.Get<Vector2>();
    }

    void OnAttack()
    {
        MulaiSerang();
    }

    void Update()
    {
        // Parameter Animator
        if (anim != null)
        {
            anim.SetFloat("Speed", arahGerak.sqrMagnitude);
        }

        // Balik hadap karakter (kiri / kanan)
        if (arahGerak.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (arahGerak.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }

        // Cadangan input langsung (Spasi atau Klik Kiri)
        if ((Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            MulaiSerang();
        }
    }

    void FixedUpdate()
    {
        // Pergerakan murni fisika: otomatis mengatasi tabrakan liar & player mundur sendiri
        rb.linearVelocity = arahGerak.normalized * kecepatan;
    }

    void MulaiSerang()
    {
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // Deteksi musuh di depan Player
        if (attackPoint != null)
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);
            foreach (Collider2D enemy in hitEnemies)
            {
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(attackDamage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            SpriteRenderer coinSprite = other.GetComponent<SpriteRenderer>();
            Sprite icon = (coinSprite != null) ? coinSprite.sprite : null;

            Destroy(other.gameObject);
            
            skor++;
            Debug.Log("Skor Kamu Saat Ini: " + skor);

            GameManager gm = FindAnyObjectByType<GameManager>();
            if (gm != null)
            {
                gm.AmbilKoin(icon);
            }
        }
    }

    // Musuh mati saat menyentuh Player (Collision)
    void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(enemyHealth.GetCurrentHealth());
        }
    }
}
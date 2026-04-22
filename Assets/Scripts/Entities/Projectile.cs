using Player;
using UnityEngine;

public class Projectile : MonoBehaviour, Weapon
{
    [Header("Настройки урона")] [SerializeField]
    private int damageAmount = 10;

    private Rigidbody2D rb;
    private bool isReflected = false; // Был ли снаряд отбит игроком?

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Время жизни стрелы на случай, если она улетит в космос и ни обо что не ударится
        Destroy(gameObject, 10f);
    }

    void Update()
    {
        // Динамически разворачиваем стрелу по направлению её полета
        if (rb != null && rb.velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    // Логика столкновений снаряда
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Игнорируем триггеры самого Скелета-создателя при спавне
        if (collision.gameObject.name.Contains("Skeleton") && !isReflected)
            return;

        // Игнорируем другие стрелы
        if (collision.TryGetComponent<Projectile>(out _))
            return;

        // КРИТИЧЕСКИ ВАЖНО: Если стрела врезалась в ЛЮБОЙ триггер (например, в ProtAttackCollider игрока),
        // мы НЕМЕДЛЕННО выходим. Стрела не должна наносить урон через триггеры!
        // Это позволит твоему щиту в его собственном скрипте отбить стрелу.
        if (collision.isTrigger)
            return;

        // --- 1. ПРОВЕРКА НА ИГРОКА ---
        // Ищем здоровье игрока строго на том объекте, у которого есть Rigidbody2D (на корневом объекте игрока)
        PlayerStats playerStatsHealth = null;
        if (collision.attachedRigidbody != null)
        {
            playerStatsHealth = collision.attachedRigidbody.GetComponent<PlayerStats>();
        }

        if (playerStatsHealth != null)
        {
            if (!isReflected)
            {
                playerStatsHealth.Hurt(1, ThreatType.RangedCombat, DamageSourceContext.DirectHit);
                Debug.Log($"Стрела попала в ТЕЛО игрока! Объект столкновения: {collision.gameObject.name}");
                Destroy(gameObject);
            }

            return;
        }

        // --- 2. ПРОВЕРКА НА ВРАГА (NPC) ---
        Enemy npc = null;
        if (collision.attachedRigidbody != null)
        {
            npc = collision.attachedRigidbody.GetComponent<Enemy>();
        }

        if (npc != null)
        {
            if (isReflected)
            {
                // npc.TakeDamage(damageAmount); 
                Debug.Log($"Отбитая стрела попала обратно в ТЕЛО NPC: {npc.name}");
                Destroy(gameObject);
                Destroy(npc.gameObject);
            }

            return;
        }

        // --- 3. ПРОВЕРКА НА РАЗРУШАЕМЫЙ ОБЪЕКТ ---
        DestroyableEntity destroyable = null;
        if (collision.attachedRigidbody != null)
        {
            destroyable = collision.attachedRigidbody.GetComponent<DestroyableEntity>();
        }
        else
        {
            destroyable = collision.GetComponent<DestroyableEntity>();
        }

        if (destroyable != null)
        {
            if (isReflected)
            {
                Destroy(destroyable.gameObject);
                Destroy(gameObject);
            }

            return;
        }

        // --- 4. СТОЛКНОВЕНИЕ СО СТЕНАМИ / ПОЛОМ (ПРЕПЯТСТВИЯ) ---
        // Если объект не триггер, у него нет Rigidbody или он статический (земля, стены)
        if (collision.attachedRigidbody == null || collision.attachedRigidbody.bodyType == RigidbodyType2D.Static)
        {
            Debug.Log($"Стрела воткнулась в твердое препятствие: {collision.name}");
            Destroy(gameObject);
        }
    }

    // Этот метод автоматически вызовется, когда твой щит изменит скорость этой стрелы
    public void Reflect()
    {
        isReflected = true;
        gameObject.tag = "Untagged";
    }
}
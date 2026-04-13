using System.Collections.Generic;
using UnityEngine;

public class ProtAttackCollliderScript : MonoBehaviour
{
    [Header("Настройки атаки")]
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float attackCooldown = 1f; // Кулдаун для атаки (в секундах)

    [Header("Настройки защиты (Отбивания)")]
    [SerializeField] private float bounceForce = 15f; 
    [SerializeField] private float protectionCooldown = 1f; // Кулдаун для защиты (в секундах)

    // Ссылка на обработчик анимаций
    private AnimationScriptHandler animationHandler;

    // Словари для отслеживания времени последнего взаимодействия с объектами
    private Dictionary<Collider2D, float> lastAttackTimes = new Dictionary<Collider2D, float>();
    private Dictionary<Collider2D, float> lastProtectionTimes = new Dictionary<Collider2D, float>();

    void Start()
    {
        animationHandler = GetComponentInParent<AnimationScriptHandler>();
        
        if (animationHandler == null)
        {
            Debug.LogError("Не найден AnimationScriptHandler на родительских объектах!");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (animationHandler == null) return;

        // --- РЕЖИМ АТАКЫ ---
        if (animationHandler.IsAttackActive)
        {
            // Проверяем, прошел ли кулдаун для этого конкретного объекта при атаке
            if (CheckCooldown(collision, lastAttackTimes, attackCooldown))
            {
                bool actionTaken = false;

                // 1. Уничтожение DestroyableEntity
                if (collision.TryGetComponent<DestroyableEntity>(out var destroyable))
                {
                    Destroy(collision.gameObject); 
                    Debug.Log($"Уничтожен объект: {collision.name}");
                    actionTaken = true;
                }

                // 2. Нанесение урона NpcEntity
                if (collision.TryGetComponent<Enemy>(out var npc))
                {
                    // npc.TakeDamage(damageAmount); 
                    Debug.Log($"Нанесен урон NPC: {collision.name}");
                    actionTaken = true;
                }

                // Если действие совершено успешно, обновляем таймер кулдауна для этого объекта
                if (actionTaken)
                {
                    lastAttackTimes[collision] = Time.time;
                }
            }
        }

        // --- РЕЖИМ ЗАЩИТЫ (ПРОТЕКШН) ---
        if (animationHandler.IsProtActive)
        {
            // Проверяем кулдаун для защиты
            if (CheckCooldown(collision, lastProtectionTimes, protectionCooldown))
            {
                bool actionTaken = false;

// 3. Отбивание снарядов ProjectEntity в обратную сторону
                if (collision.TryGetComponent<Projectile>(out var projectile))
                {
                    if (collision.TryGetComponent<Rigidbody2D>(out var projRb))
                    {
                        projRb.velocity = -projRb.velocity * 1.2f; 
        
                        // ВОТ ЭТУ СТРОЧКУ ДОБАВЬ:
                        projectile.Reflect(); // Говорим стреле, что она отбита и теперь опасна для NPC
        
                        Debug.Log("Снаряд отбит обратно!");
                        actionTaken = true;
                    }
                }

                // 4. Отбивание NPC, атакующих игрока (расталкивание)
                if (collision.TryGetComponent<Enemy>(out var attackingNpc))
                {
                    if (collision.TryGetComponent<Rigidbody2D>(out var npcRb))
                    {
                        Vector2 pushDirection = (collision.transform.position - transform.parent.position).normalized;
                        
                        // Сбрасываем старую скорость, чтобы импульс всегда отбрасывал с одинаковой силой
                        npcRb.velocity = Vector2.zero; 
                        
                        npcRb.AddForce(pushDirection * bounceForce, ForceMode2D.Impulse);
                        Debug.Log($"Враг {collision.name} отбит защитой!");
                        actionTaken = true;
                    }
                }

                // Если отбили объект, обновляем таймер кулдауна для него
                if (actionTaken)
                {
                    lastProtectionTimes[collision] = Time.time;
                }
            }
        }
    }

    // Вспомогательный метод для проверки: можно ли уже взаимодействовать с объектом?
    private bool CheckCooldown(Collider2D col, Dictionary<Collider2D, float> cooldownDictionary, float cooldownDuration)
    {
        // Если объект еще ни разу не получал урон/не отбивался, разрешаем действие
        if (!cooldownDictionary.ContainsKey(col))
        {
            return true;
        }

        // Если прошло достаточно времени с момента последней записи — разрешаем действие
        if (Time.time - cooldownDictionary[col] >= cooldownDuration)
        {
            return true;
        }

        return false;
    }

    // Очистка словарей, когда объекты полностью покидают триггер, чтобы не забивать память
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (lastAttackTimes.ContainsKey(collision)) lastAttackTimes.Remove(collision);
        if (lastProtectionTimes.ContainsKey(collision)) lastProtectionTimes.Remove(collision);
    }
}
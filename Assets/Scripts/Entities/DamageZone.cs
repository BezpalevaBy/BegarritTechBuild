using UnityEngine;

namespace Player
{
    public class DamageZone : MonoBehaviour
    {
        [Header("Settings")]
        public float damageDelay = 1.0f; // Задержка между ударами (в секундах)
        
        private float lastDamageTime;

        // Метод срабатывает каждый кадр, пока игрок внутри триггера
        private void OnTriggerStay2D(Collider2D other)
        {
            // Проверяем, что в зону вошел именно объект с тегом "Player"
            if (other.CompareTag("Player"))
            {
                // Проверяем, прошло ли достаточно времени с последнего урона
                if (Time.time >= lastDamageTime + damageDelay)
                {
                    // Ищем компонент здоровья на игроке
                    PlayerStats playerStatsHealth = other.GetComponent<PlayerStats>();

                    if (playerStatsHealth != null)
                    {
                        playerStatsHealth.Hurt(1, ThreatType.Traps, DamageSourceContext.TrapTriggered); // Вызываем твой метод урона
                        lastDamageTime = Time.time; // Запоминаем время удара
                    }
                }
            }
        }
    }
}
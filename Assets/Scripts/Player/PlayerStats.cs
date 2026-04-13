using UnityEngine;
using System.Collections;
using DefaultNamespace;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        public float health = 3;
        public GameObject vinytte;   
        public GameObject blackout;  

        [Header("Spawn Settings")]
        public Vector3 spawnPosition = Vector3.zero; // Нулевые координаты по умолчанию
        
        private SpriteRenderer vignetteSR;
        private SpriteRenderer blackoutSR;

        public void Start()
        {
            if (vinytte != null)
                vignetteSR = vinytte.GetComponent<SpriteRenderer>();

            if (blackout != null)
                blackoutSR = blackout.GetComponent<SpriteRenderer>();

            // При запуске игры сразу вызываем спавн
            Spawn();
        }

        public void Spawn()
        {
            // 1. Устанавливаем здоровье
            health = 3;

            // 2. Перемещаем игрока (Аделара) в точку спавна
            transform.position = spawnPosition;

            // 3. Тёмный фон полностью (мгновенно)
            if (blackout != null)
            {
                blackout.SetActive(true);
                Color c = blackoutSR.color;
                c.a = 1f; 
                blackoutSR.color = c;

                // 4. Плавное снятие темного фона
                StartCoroutine(FadeFromBlack());
            }

            // Сбрасываем виньетку до начального состояния (3 HP)
            UpdateVignetteAlpha();
        }

        private IEnumerator FadeFromBlack()
        {
            float duration = 4.0f; // Длительность появления (в секундах)
            float elapsed = 0f;
            Color c = blackoutSR.color;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
                blackoutSR.color = c;
                yield return null;
            }

            blackout.SetActive(false); // Выключаем совсем, когда прозрачный
        }

        public void Hurt(int damage, ThreatType thType, DamageSourceContext cont)
        {
            health--;
            if (DifficultyAdapter.Instance != null)
            {
                DifficultyAdapter.Instance.TrackPlayerDamage(thType, cont, damage);
            }
            if (health <= 0) Death();
            else UpdateVignetteAlpha();
        }

        private void UpdateVignetteAlpha()
        {
            if (vignetteSR == null) return;
            float t = (health - 1) / 2f; 
            float alpha = Mathf.Lerp(1.0f, 0.92f, t);
            Color c = vignetteSR.color;
            c.a = Mathf.Clamp(alpha, 0f, 1f);
            vignetteSR.color = c;
        }

        public void Death()
        {
            health = 0;
            if (blackout != null)
            {
                blackout.SetActive(true);
                StartCoroutine(FadeToBlack());
            }
        }

        private IEnumerator FadeToBlack()
        {
            float duration = 1.5f; 
            float elapsed = 0f;
            Color c = blackoutSR.color;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                blackoutSR.color = c;
                yield return null;
            }
            
            if (SceneManager.GetActiveScene().name == "FirstDungeonLevel")
            {
                GameManager.ChangeScene("GameMenu");
                yield break;
            }
            
            Spawn(); 
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Базовые характеристики")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Боевые параметры")]
    public float blockChance = 0.2f;
    [SerializeField] private GameObject arrowPrefab; // Префаб стрелы (для лучников)
    [SerializeField] private Transform firePoint;     // Точка спавна снаряда

    // Скрытые компоненты, управляемые через тело
    private Rigidbody2D rb;
    private Animator anim;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    // --- ЛОГИКА ДВИЖЕНИЯ ---
    
    public void Move(Vector2 direction, float speed)
    {
        rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
        Flip(direction.x);
        UpdateAnimationSpeed();
    }

    public void StopMoving()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
        UpdateAnimationSpeed();
    }

    public void Flip(float x)
    {
        if (x > 0) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
        else if (x < 0) transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, 1);
    }

    private void UpdateAnimationSpeed()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
    }

    // --- ЛОГИКА ДЕЙСТВИЙ (Вызываются из AI) ---

    public void PlayAttackAnimation()
    {
        if (anim != null) anim.SetTrigger("Attack");
        else ShootArrow(); // Если аниматора нет, стреляем кодом напрямую
    }

    public void ActivateBlock()
    {
        StopMoving();
        if (anim != null) anim.SetTrigger("Block");
        Debug.Log($"{gameObject.name} блокирует удар!");
    }

    // Метод триггерится из AI или через Animation Event на кадре выстрела лука
    public void ShootArrow()
    {
        // Находим игрока для определения направления выстрела
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (arrowPrefab == null || firePoint == null || playerObj == null) return;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        Vector2 shootDirection = (playerObj.transform.position - firePoint.position).normalized;

        if (arrow.TryGetComponent<Rigidbody2D>(out var arrowRb))
        {
            float arrowSpeed = 5f;
            arrowRb.velocity = shootDirection * arrowSpeed;

            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    
    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} получил {damage} урона. Осталось HP: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} погиб.");
        Destroy(gameObject);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Simple, Smart }
    public EnemyType type;

    [Header("Detection")]
    public float detectRange = 8f;
    public float attackRange = 5f; 
    public LayerMask playerLayer;

    [Header("Movement & Patrol")]
    public float speed = 2f;
    [SerializeField] private float leftBoundary;  
    [SerializeField] private float rightBoundary; 
    private bool movingRight = true;
    
    [Header("Combat & Ranged")]
    public bool isRanged = true; 
    [SerializeField] private float attackCooldown = 2f; 
    private float nextAttackTime = 0f;
    private bool isRetreating = false;

    // Ссылки на подконтрольные объекты
    private Enemy enemyBody;
    private Transform player;

    void Start()
    {
        enemyBody = GetComponent<Enemy>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (leftBoundary == 0 && rightBoundary == 0)
        {
            leftBoundary = transform.position.x - 3f;
            rightBoundary = transform.position.x + 3f;
        }
    }

    void Update()
    {
        if (player == null || isRetreating) return;

        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= detectRange)
        {
            HandleBehavior(distance);
        }
        else
        {
            Patrol();
        }
    }

    void HandleBehavior(float distance)
    {
        if (type == EnemyType.Smart && IsLastInGroup())
        {
            StartCoroutine(RunAway());
            return;
        }

        // Если игрок ушел из зоны видимости — возвращаемся к патрулированию
        if (Vector2.Distance(transform.position, player.position) > detectRange)
        {
            Patrol();
            return;
        }

        // Если игрок в зоне видимости, но еще далеко для атаки — преследуем
        if (distance > attackRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            enemyBody.Move(direction, speed);
        }
        else
        {
            CombatLogic(distance);
        }
    }

    void Patrol()
    {
        if (Mathf.Approximately(leftBoundary, rightBoundary))
        {
            leftBoundary = transform.position.x - 5f;
            rightBoundary = transform.position.x + 5f;
        }

        if (movingRight)
        {
            enemyBody.Move(Vector2.right, speed);

            if (transform.position.x >= rightBoundary)
            {
                movingRight = false;
                Debug.Log($"{gameObject.name}: Достиг ПРАВОЙ границы. Поворот влево.");
            }
        }
        else
        {
            enemyBody.Move(Vector2.left, speed);

            if (transform.position.x <= leftBoundary)
            {
                movingRight = true;
                Debug.Log($"{gameObject.name}: Достиг ЛЕВОЙ границы. Поворот вправо.");
            }
        }
    }

    void CombatLogic(float distance)
    {
        Vector2 direction = (player.position - transform.position).normalized;
        enemyBody.Flip(direction.x); // Всегда держим фокус взгляда на игроке

        // Умный враг использует блок через вызов тела
        if (type == EnemyType.Smart && distance < 1.5f && Random.value < enemyBody.blockChance)
        {
            enemyBody.ActivateBlock();
            return;
        }

        // Если игрок подошел слишком близко, лучник отступает назад (кайтит)
        if (isRanged && distance < attackRange * 0.5f)
        {
            enemyBody.Move(-direction, speed);
        }
        else
        {
            enemyBody.StopMoving();
        }

        // Атака по таймеру
        if (Time.time >= nextAttackTime)
        {
            enemyBody.PlayAttackAnimation();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    IEnumerator RunAway()
    {
        isRetreating = true;
        Debug.Log(gameObject.name + " в ужасе убегает!");
        
        float retreatTime = 3f;
        while (retreatTime > 0)
        {
            Vector2 dir = (transform.position - player.position).normalized;
            enemyBody.Move(dir, speed * 1.5f);
            retreatTime -= Time.deltaTime;
            yield return null;
        }
        
        Destroy(gameObject);
    }

    bool IsLastInGroup()
    {
        GameObject[] teammates = GameObject.FindGameObjectsWithTag("Enemy");
        return teammates.Length <= 1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 leftPos = new Vector3(leftBoundary, transform.position.y, transform.position.z);
        Vector3 rightPos = new Vector3(rightBoundary, transform.position.y, transform.position.z);
        Gizmos.DrawLine(leftPos + Vector3.up, leftPos + Vector3.down);
        Gizmos.DrawLine(rightPos + Vector3.up, rightPos + Vector3.down);
        Gizmos.DrawLine(leftPos, rightPos);
    }
}
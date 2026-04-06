using UnityEngine;

public class AnimationScriptHandler : MonoBehaviour
{
    [SerializeField] private Collider2D Collider;

    private bool isProtActive = false;
    private bool isAttackActive = false;

    // Свойства («геттеры»), чтобы скрипт коллайдера мог узнать текущее состояние
    public bool IsProtActive => isProtActive;
    public bool IsAttackActive => isAttackActive;

    public void EnableAttackCollider()
    {
        if (Collider != null)
        {
            Collider.enabled = true;
            isAttackActive = true;
            isProtActive = false; // На всякий случай выключаем защиту
            Debug.Log("Атака включена");
        }
    }

    public void DisableAttackCollider()
    {
        if (Collider != null)
        {
            Collider.enabled = false;
            isAttackActive = false;
            Debug.Log("Атака выключена");
        }
    }
    
    public void EnableProtection()
    {
        if (Collider != null)
        {
            Collider.enabled = true;
            isProtActive = true;
            isAttackActive = false; // На всякий случай выключаем атаку
            Debug.Log("Защита включена");
        }
    }
    
    public void DisableProtection()
    {
        if (Collider != null)
        {
            Collider.enabled = false;
            isProtActive = false;
            Debug.Log("Защита выключена");
        }
    }
}
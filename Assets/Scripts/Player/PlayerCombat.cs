using System;
using UnityEngine;
public class PlayerCombat : MonoBehaviour
{
    private static readonly int Protect = Animator.StringToHash("Protect");
    public Animator animator;

    public Canvas menu;
    
    // Задержка между атаками (чтобы не спамить)
    public float attackRate = 2f;
    float nextAttackTime = 0f;
    private bool isPreparingAlready = false;
    
    void Update()
    {
        if (Input.GetButtonDown("Fire1")) // По умолчанию это ЛКМ или левый Ctrl
        { 
            PerformAttack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            PerformProtection();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.gameObject.SetActive(!menu.isActiveAndEnabled);
            StopGameManager.DoAction();
        }
        if (Input.GetKeyDown(KeyCode.F5))
        {
            FindObjectOfType<SaveManager>().SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.F9))
        {
            FindObjectOfType<SaveManager>().LoadGame();
        }
    }

    void PerformProtection()
    {
        if (animator.GetFloat("Speed") > 0.1f) return;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isProtect = stateInfo.IsName("Protect");

        if (isProtect) return;
        
        animator.SetTrigger(Protect);
    }

    void PerformAttack()
    {
        if (animator.GetFloat("Speed") > 0.1f) return;
        
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("SprintAttack") || stateInfo.IsName("AttackStandBase");

        if (isAttacking) return;
        
        if (isPreparingAlready)
        {
            isPreparingAlready = false;
            animator.SetBool("IsAttacking", true);
            return;
        }
        else
        {
            isPreparingAlready = true;
            animator.SetBool("IsAttacking", false);
            Invoke("RechargePreparing", 2f);
        }

        animator.SetBool("IsMovementBlocked", true);
        Invoke("UnlockMovement", 3f);

        animator.SetTrigger("Attack");

        Debug.Log("Аделар атакует!");
    }

    void RechargePreparing()
    {
        isPreparingAlready = false;
        animator.SetBool("IsAttacking", false);
    }
    
    void UnlockMovement()
    {
        isPreparingAlready = false;
        animator.SetBool("IsMovementBlocked", false);
    }
}
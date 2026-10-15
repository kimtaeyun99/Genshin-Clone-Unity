using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator Animator => CharacterManager.Instance != null ? CharacterManager.Instance.CurrentAnimator : null;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int AttackIndexHash = Animator.StringToHash("AttackIndex");
    private static readonly int SkillHash = Animator.StringToHash("Skill");
    private static readonly int SkillIndexHash = Animator.StringToHash("SkillIndex");
    private static readonly int BurstHash = Animator.StringToHash("Burst");
    private static readonly int DeadHash = Animator.StringToHash("Dead");


    private void Start()
    {
        CharacterManager.Instance.OnCurrentCharacterChanged += HandleCharacterChanged;
        ResetAnimatorToIdle();
    }
    private void OnDestroy()
    {
        CharacterManager.Instance.OnCurrentCharacterChanged -= HandleCharacterChanged;
    }

    private void HandleCharacterChanged(CharacterRunTime _)
    {
        ResetAnimatorToIdle();
    }

    public void ResetAnimatorToIdle()
    {
        if (CharacterManager.Instance == null)
            return;

        Animator animator = Animator;
        if (animator == null || animator.runtimeAnimatorController == null)
            return;

        animator.Rebind();

        animator.ResetTrigger(JumpHash);
        animator.ResetTrigger(DodgeHash);
        animator.ResetTrigger(AttackHash);
        animator.ResetTrigger(SkillHash);
        animator.ResetTrigger(BurstHash);
        animator.ResetTrigger(DeadHash);
        animator.SetInteger(AttackIndexHash, 0);
        animator.SetInteger(SkillIndexHash, 0);
        animator.SetInteger(StateHash, 0);
        animator.Update(0f);
    }

    public void PlayIdle()
    {
        Animator.SetInteger(StateHash, 0);
    }

    public void PlayMove()
    {
        Animator.SetInteger(StateHash, 1);
    }

    public void PlayJump()
    {
        Animator.SetTrigger(JumpHash);
    }

    public void PlayDodge()
    {
        Animator.SetTrigger(DodgeHash);
    }

    public void PlayAttack()
    {
        Animator.SetTrigger(AttackHash);
    }

    public void SetAttackIndex(int index)
    {
        Animator.SetInteger(AttackIndexHash, index);
    }

    public void PlaySkill()
    {
        Animator.SetTrigger(SkillHash);
    }

    public void SetSkillIndex(int index)
    {
        Animator.SetInteger(SkillIndexHash, index);
    }

    public void PlayBurst()
    {
        Animator.SetTrigger(BurstHash);
    }

    public void PlayDead()
    {
        Animator.SetTrigger(DeadHash);
    }

    public AnimatorStateInfo GetCurrentStateInfo()
    {
        return Animator.GetCurrentAnimatorStateInfo(0);
    }

    public bool IsTransition()
    {
        return Animator.IsInTransition(0);
    }
}

using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;

    private Animator Animator => characterManager.CurrentAnimator;

    public void PlayIdle()
    {
        Animator.SetInteger("State", 0);
    }

    public void PlayMove()
    {
        Animator.SetInteger("State", 1);
    }

    public void PlayJump()
    {
        Animator.SetTrigger("Jump");
    }

    public void PlayDodge()
    {
        Animator.SetTrigger("Dodge");
    }

    public void PlayNormalAttack()
    {
        Animator.SetInteger("State", 0);
        Animator.SetTrigger("NormalAttack");
    }

    public void SetNormalAttackIndex(int index)
    {
        Animator.SetInteger("NormalAttackIndex", index);
    }
    public void PlaySkill()
    {
        Animator.SetInteger("State", 0);
        Animator.SetTrigger("Skill");
    }
    public void SetSkillIndex(int index)
    {
        Animator.SetInteger("SkillIndex", index);
    }
    public void PlayBurst()
    {
        Animator.SetInteger("State", 0);
        Animator.SetTrigger("Burst");
    }
    public void PlayDead()
    {
        Animator.SetInteger("State", 0);
        Animator.SetTrigger("Dead");
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
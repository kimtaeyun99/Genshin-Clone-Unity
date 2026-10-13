using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private CharacterManager characterManager;

    private Animator Animator => characterManager.CurrentAnimator;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");
    private static readonly int NormalAttackHash = Animator.StringToHash("NormalAttack");
    private static readonly int NormalAttackIndexHash = Animator.StringToHash("NormalAttackIndex");
    private static readonly int SkillHash = Animator.StringToHash("Skill");
    private static readonly int SkillIndexHash = Animator.StringToHash("SkillIndex");
    private static readonly int BurstHash = Animator.StringToHash("Burst");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

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

    public void PlayNormalAttack()
    {
        Animator.SetTrigger(NormalAttackHash);
    }

    public void SetNormalAttackIndex(int index)
    {
        Animator.SetInteger(NormalAttackIndexHash, index);
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
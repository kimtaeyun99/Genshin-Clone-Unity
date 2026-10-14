using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private CharacterManager subscribedManager;

    private Animator Animator => CharacterManager.Instance != null ? CharacterManager.Instance.CurrentAnimator : null;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DodgeHash = Animator.StringToHash("Dodge");
    private static readonly int NormalAttackHash = Animator.StringToHash("NormalAttack");
    private static readonly int NormalAttackIndexHash = Animator.StringToHash("NormalAttackIndex");
    private static readonly int SkillHash = Animator.StringToHash("Skill");
    private static readonly int SkillIndexHash = Animator.StringToHash("SkillIndex");
    private static readonly int BurstHash = Animator.StringToHash("Burst");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void OnEnable()
    {
        BindManager();
    }

    private void Start()
    {
        BindManager();
    }

    private void OnDisable()
    {
        if (subscribedManager != null)
            subscribedManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        subscribedManager = null;
    }

    private void BindManager()
    {
        CharacterManager manager = CharacterManager.Instance;
        if (manager == null || manager == subscribedManager)
            return;

        if (subscribedManager != null)
            subscribedManager.OnCurrentCharacterChanged -= HandleCharacterChanged;

        subscribedManager = manager;
        subscribedManager.OnCurrentCharacterChanged += HandleCharacterChanged;

        if (manager.CurrentCharacter != null)
            ResetAnimatorToIdle();
    }

    private void HandleCharacterChanged(CharacterRunTime character)
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
        animator.ResetTrigger(NormalAttackHash);
        animator.ResetTrigger(SkillHash);
        animator.ResetTrigger(BurstHash);
        animator.ResetTrigger(DeadHash);
        animator.SetInteger(NormalAttackIndexHash, 0);
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

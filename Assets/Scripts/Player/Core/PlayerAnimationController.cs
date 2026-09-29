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
        Animator.SetTrigger("NormalAttack");
    }

    public void SetNormalAttackIndex(int index)
    {
        Animator.SetInteger("NormalAttackIndex", index);
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
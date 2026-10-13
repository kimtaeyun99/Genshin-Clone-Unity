using UnityEngine;

public class MonsterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int AttackIndexHash = Animator.StringToHash("AttackIndex");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DeadHash = Animator.StringToHash("Dead");

    public void PlayIdle()
    {
        animator.SetInteger(StateHash, 0);
    }

    public void PlayPatrol()
    {
        animator.SetInteger(StateHash, 1);
    }

    public void PlayChase()
    {
        animator.SetInteger(StateHash, 2);
    }

    public void PlayAttack(int attackIndex)
    {
        animator.SetInteger(StateHash, 3);
        animator.SetInteger(AttackIndexHash, attackIndex);
        animator.SetTrigger(AttackHash);
    }

    public void PlayDead()
    {
        animator.SetTrigger(DeadHash);
    }

    public AnimatorStateInfo GetCurrentStateInfo()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }
    public bool IsTransition()
    {
        return animator.IsInTransition(0);
    }
}
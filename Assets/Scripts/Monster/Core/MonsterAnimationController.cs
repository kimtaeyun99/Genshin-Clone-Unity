using UnityEngine;

public class MonsterAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void PlayIdle()
    {
        animator.SetInteger("State", 0);
    }

    public void PlayPatrol()
    {
        animator.SetInteger("State", 1);
    }

    public void PlayChase()
    {
        animator.SetInteger("State", 2);
    }

    public void PlayAttack()
    {
        int attackIndex = Random.Range(0, 2);

        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetTrigger("Attack");
    }

    public void PlayDead()
    {
        animator.SetTrigger("Dead");
    }

    public AnimatorStateInfo GetCurrentStateInfo()
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }
}
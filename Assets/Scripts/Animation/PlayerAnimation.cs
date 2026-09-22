using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator currentAnimator;

    public void SetAnimator(Animator animator)
    {
        currentAnimator = animator;
    }

    public void PlayIdle()
    {
        if (currentAnimator == null)
            return;

        currentAnimator.Play("Idle");
    }

    public void PlayMove()
    {
        if (currentAnimator == null)
            return;

        currentAnimator.Play("Move");
    }

    public void PlayJump()
    {
        if (currentAnimator == null)
            return;

        currentAnimator.Play("Jump");
    }

    public void PlayFall()
    {
        if (currentAnimator == null)
            return;

        currentAnimator.Play("Fall");
    }

    public void PlayDodge()
    {
        if (currentAnimator == null)
            return;

        currentAnimator.Play("Dodge");
    }
}
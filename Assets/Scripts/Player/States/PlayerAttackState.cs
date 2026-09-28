using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private int normalAttackIndex;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    private bool nextAttackInput;
    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {
        animator = player.CharacterManager.CurrentAnimator;

        normalAttackIndex = 1;
        nextAttackInput = false;

        player.Movement.ExitDashMode();

        player.CharacterManager.CurrentAnimator.SetTrigger("NormalAttack");
        player.CharacterManager.CurrentAnimator.SetInteger("NormalAttackIndex", normalAttackIndex);

        InputManager.Instance.OnAttack += OnAttack;

        Debug.Log("Attack 진입");
    }
    public override void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsTag("NormalAttack"))
        {
            return;
        }

        if (animator.IsInTransition(0))
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            if (nextAttackInput && normalAttackIndex < 3)
            {
                normalAttackIndex++;
                nextAttackInput = false;

                animator.SetInteger("NormalAttackIndex", normalAttackIndex);

                Debug.Log($"{normalAttackIndex}타 시작");
            }
            else
            {
                ChangeNextState();
            }
        }
    }
    public override void Exit()
    {
        InputManager.Instance.OnAttack -= OnAttack;

        Debug.Log("Attack 퇴장");
    }
    private void ChangeNextState()
    {
        if(InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    private void OnAttack()
    {
        if (normalAttackIndex >= 3)
        {
            return;
        }

        nextAttackInput = true;

        Debug.Log("다음 평타 입력 저장");
    }
}
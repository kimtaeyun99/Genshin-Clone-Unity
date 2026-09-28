using UnityEngine;

public class PlayerAttackState : PlayerStateBase
{
    private int normalAttackIndex;
    public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {
        normalAttackIndex = 1;

        player.Movement.ExitDashMode();

        player.CharacterManager.CurrentAnimator.SetTrigger("NormalAttack");
        player.CharacterManager.CurrentAnimator.SetInteger("NormalAttackIndex", normalAttackIndex);

        InputManager.Instance.OnAttack += OnAttack;

        Debug.Log("Attack 진입");
    }
    public override void Update()
    {
        Animator animator = player.CharacterManager.CurrentAnimator;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if(stateInfo.normalizedTime >= 1f)
        {
            ChangeNextState();
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
        if(normalAttackIndex >= 3)
        {
            return;
        }

        normalAttackIndex++;

        player.CharacterManager.CurrentAnimator.SetInteger("NormalAttackIndex", normalAttackIndex);

        Debug.Log($"{normalAttackIndex} 타입력");
    }
}
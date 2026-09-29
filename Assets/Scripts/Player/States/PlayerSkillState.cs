using UnityEngine;

public class PlayerSkillState : PlayerStateBase
{
    private int skillIndex;
    private AnimatorStateInfo stateInfo;
    private bool nextSkillInput;
    public PlayerSkillState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {

    }
    public override void Enter()
    {
        skillIndex = 1;
        nextSkillInput = false;

        player.Animation.SetSkillIndex(skillIndex);
        player.Animation.PlaySkill();


        InputManager.Instance.OnSkill += OnSkill;

        Debug.Log("Skill 진입");
    }
    public override void Update()
    {
        stateInfo = player.Animation.GetCurrentStateInfo();

        if (!stateInfo.IsTag("Skill"))
        {
            return;
        }

        if (player.Animation.IsTransition())
        {
            return;
        }

        if (stateInfo.normalizedTime >= 1f)
        {
            if (nextSkillInput && skillIndex < 3)
            {
                skillIndex++;
                nextSkillInput = false;

                player.Animation.SetSkillIndex(skillIndex);

                Debug.Log($"{skillIndex}타 시작");
            }
            else
            {
                ChangeNextState();
            }
        }
    }
    public override void Exit()
    {
        InputManager.Instance.OnSkill -= OnSkill;

        Debug.Log("Skill 퇴장");
    }
    private void ChangeNextState()
    {
        if (InputManager.Instance.MoveInput.sqrMagnitude > 0.01f)
        {
            stateMachine.ChangeState(PlayerStateType.Move);
        }
        else
        {
            stateMachine.ChangeState(PlayerStateType.Idle);
        }
    }
    private void OnSkill()
    {
        if (skillIndex >= 3)
        {
            return;
        }

        nextSkillInput = true;

        Debug.Log("다음 스킬 입력 저장");
    }
}
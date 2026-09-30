using UnityEngine;

public class DilucBurst : MonoBehaviour, ICharacterBurst
{
    private bool isFinished;

    private CharacterRunTime character;
    private PlayerAnimationController playerAnimationController;

    public bool IsFinished => isFinished;

    public void Enter(
        CharacterRunTime characterRunTime,
        PlayerAnimationController playerAnimationController)
    {
        character = characterRunTime;
        this.playerAnimationController = playerAnimationController;

        isFinished = false;

        playerAnimationController.PlayBurst();

        Debug.Log("Diluc Burst 시작");
    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (!stateInfo.IsTag("Burst"))
        {
            return;
        }

        if (stateInfo.normalizedTime >= 0.95f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {
        Debug.Log("Diluc Burst 종료");
    }
}
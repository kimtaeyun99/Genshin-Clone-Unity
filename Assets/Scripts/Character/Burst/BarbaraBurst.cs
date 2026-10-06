using UnityEngine;

public class BarbaraBurst : MonoBehaviour, ICharacterBurst
{
    private bool isFinished;
    private bool enteredBurstAnimation;

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
        enteredBurstAnimation = false;

        playerAnimationController.PlayBurst();

        Debug.Log("Barbara Burst 시작");
    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (stateInfo.IsTag("Burst"))
        {
            enteredBurstAnimation = true;
            return;
        }

        if (enteredBurstAnimation)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {
        Debug.Log("Barbara Burst 종료");
    }

    private void HealParty()
    {
        float healPercent =
            character.Data.CharacterCombatData
                .GetBurstMultipliers(0);

        int healAmount = Mathf.RoundToInt(
            character.MaxHP * (healPercent / 100f)
        );

        foreach (CharacterData partyMember in CharacterParty.Instance.PartyMembers)
        {
            CharacterRunTime runTime =
                CharacterManager.Instance.GetCharacter(partyMember);

            if (runTime == null)
            {
                continue;
            }

            runTime.Heal(healAmount);

            Debug.Log(
                $"{partyMember.name} 회복 : {healAmount} / " +
                $"현재 HP : {runTime.CurrentHP}"
            );
        }
    }
}
using UnityEngine;
using PixPlays.ElementalVFX;

public class BarbaraBurst : MonoBehaviour, ICharacterBurst
{
    [Header("Burst VFX")]
    [SerializeField] private LocationVfx burstVfxPrefab;
    [SerializeField] private float burstVfxDuration = 3f;
    [SerializeField] private float burstVfxRadius = 1f;

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
        enteredBurstAnimation = false;
        isFinished = false;

        playerAnimationController.PlayBurst();

        Debug.Log("Barbara Burst 시작");
    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (!enteredBurstAnimation)
        {
            if (stateInfo.IsTag("Burst"))
            {
                enteredBurstAnimation = true;
            }

            return;
        }

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
        Debug.Log("Barbara Burst 종료");
    }

    // Animation Event
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

        PlayBurstVfx();
    }

    private void PlayBurstVfx()
    {
        Transform characterTransform =
            CharacterManager.Instance.CharacterView.CurrentView.transform;

        Vector3 source = characterTransform.position;
        Vector3 target = source + characterTransform.forward;

        LocationVfx burstVfx =
            Instantiate(
                burstVfxPrefab,
                source,
                Quaternion.identity
            );

        VfxData data = new VfxData(
            source,
            target,
            burstVfxDuration,
            burstVfxRadius
        );

        burstVfx.Play(data);
    }
}
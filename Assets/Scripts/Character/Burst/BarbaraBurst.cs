using UnityEngine;
using PixPlays.ElementalVFX;

public class BarbaraBurst : MonoBehaviour, ICharacterBurst
{
    [Header("Burst VFX")]
    [SerializeField] private LocationVfx burstVfxPrefab;
    [SerializeField] private float burstVfxDuration = 3f;
    [SerializeField] private float burstVfxRadius = 1f;

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
    }

    public void BurstUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (stateInfo.normalizedTime >= 0.95f)
        {
            isFinished = true;
        }
    }

    public void Exit()
    {

    }

    private void HealParty()
    {
        float healPercent =
            character.Data.BurstMultipliers[0];

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
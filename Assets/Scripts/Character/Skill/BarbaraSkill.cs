using UnityEngine;
using PixPlays.ElementalVFX;

public class BarbaraSkill : MonoBehaviour, ICharacterSkill
{
    [Header("Water Aura")]
    [SerializeField] private LocationVfx waterAuraPrefab;
    [SerializeField] private float waterAuraDuration = 10f;
    [SerializeField] private float waterAuraRadius = 1f;

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

        character.AddEnergy(50);

        playerAnimationController.PlaySkill();

    }

    public void SkillUpdate()
    {
        if (playerAnimationController.IsTransition())
            return;

        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (stateInfo.IsName("Skill") &&
            stateInfo.normalizedTime >= 0.95f)
        {
            isFinished = true;
        }
    }

    public void OnSkillInput()
    {
    }

    public void Exit()
    {
    }

    private void SpawnSkillProjectile()
    {
        float healPercent =
            character.Data.SkillMultipliers[0];


        int healAmount = Mathf.RoundToInt(
            character.MaxHP * (healPercent / 100f)
        );

        Transform effectRoot =
            CharacterManager.Instance.EffectRoot;

        Transform characterTransform =
            CharacterManager.Instance.CharacterView.CurrentView.transform;

        LocationVfx waterAura = Instantiate(
            waterAuraPrefab,
            effectRoot
        );

        waterAura.transform.localPosition = Vector3.zero;
        waterAura.transform.localRotation = Quaternion.identity;

        WaterRing waterRing =
            waterAura.GetComponent<WaterRing>();

        waterRing.Initialize(healAmount);

        Vector3 source = effectRoot.position;
        Vector3 target =
            source + characterTransform.forward;

        VfxData data = new VfxData(
            source,
            target,
            waterAuraDuration,
            waterAuraRadius
        );

        waterAura.Play(data);
    }
}
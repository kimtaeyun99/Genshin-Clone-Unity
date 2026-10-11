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

        character.AddEnergy(50f);

        playerAnimationController.PlaySkill();

        Debug.Log("Barbara Skill 시작");
    }

    public void SkillUpdate()
    {
        AnimatorStateInfo stateInfo =
            playerAnimationController.GetCurrentStateInfo();

        if (!stateInfo.IsTag("Skill"))
        {
            return;
        }

        if (stateInfo.normalizedTime >= 0.95f)
        {
            isFinished = true;
        }
    }

    public void OnSkillInput()
    {
    }

    public void Exit()
    {
        Debug.Log("Barbara Skill 종료");
    }

    private void SpawnSkillProjectile()
    {
        // 회복량 계산
        float healPercent =
            character.Data.CharacterCombatData
                .GetSkillMultipliers(0);

        int healAmount = Mathf.RoundToInt(
            character.MaxHP * (healPercent / 100f)
        );

        Transform effectRoot =
            CharacterManager.Instance.EffectRoot;

        Transform characterTransform =
            CharacterManager.Instance.CharacterView.CurrentView.transform;

        // WaterAura 생성
        LocationVfx waterAura = Instantiate(
            waterAuraPrefab,
            effectRoot
        );

        waterAura.transform.localPosition = Vector3.zero;
        waterAura.transform.localRotation = Quaternion.identity;

        // 기존 WaterRing 회복 + 원소 부착 로직 시작
        WaterRing waterRing =
            waterAura.GetComponent<WaterRing>();

        waterRing.Initialize(healAmount);

        // WaterAura VFX 실행
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
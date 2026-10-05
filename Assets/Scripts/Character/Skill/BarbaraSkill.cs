using UnityEngine;

public class BarbaraSkill : MonoBehaviour, ICharacterSkill
{
    [Header("Water Ring")]
    [SerializeField] private WaterRing waterRingPrefab;

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

        playerAnimationController.PlaySkill();

        SpawnWaterRing();

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

    private void SpawnWaterRing()
    {
        float healPercent =
            character.Data.CharacterCombatData
                .GetSkillMultipliers(0);

        int healAmount = Mathf.RoundToInt(
            character.MaxHP * (healPercent / 100f)
        );

        WaterRing waterRing = Instantiate(
            waterRingPrefab,
            CharacterManager.Instance.CharacterView.CurrentView.transform.position,
            Quaternion.identity
        );

        waterRing.Initialize(healAmount);
    }
}
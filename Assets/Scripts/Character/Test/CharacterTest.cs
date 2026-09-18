using UnityEngine;

public class CharacterTest : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;
    [SerializeField] private CharacterStatCalculator characterStatCalculator;

    private CharacterRunTime runtime;

    private void Start()
    {
        runtime = new CharacterRunTime(characterData);

        characterStatCalculator.ApplyStat(runtime);

    }
    public void LevelUp()
    {
        runtime.LevelUp();

        characterStatCalculator.ApplyStat(runtime);

        DebugCharacter();

    }
    public void Ascension()
    {
        runtime.Ascension();

        characterStatCalculator.ApplyStat(runtime);

        DebugCharacter();
    }
    private void DebugCharacter()
    {
        Debug.Log($"캐릭터 : {runtime.Data.CharacterName}");
        Debug.Log($"돌파 단계 : {runtime.AscensionPhase}");
        Debug.Log($"레벨 : {runtime.Level}");
        Debug.Log($"최대 체력 : {runtime.MaxHP}");
        Debug.Log($"현재 체력 : {runtime.CurrentHP}");
    }
}

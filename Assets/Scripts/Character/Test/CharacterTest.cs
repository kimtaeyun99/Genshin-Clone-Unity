using UnityEngine;

public class CharacterTest : MonoBehaviour
{
    [SerializeField] private CharacterData dilucData;
    [SerializeField] private CharacterData barbaraData;
    [SerializeField] private CharacterStatCalculator characterStatCalculator;
    [SerializeField] private CharacterManager characterManager;

    public void LevelUp()
    {
        CharacterRunTime runtime = characterManager.CurrentCharacter;

        runtime.LevelUp();

        characterStatCalculator.ApplyStat(runtime);

        DebugCharacter();

    }
    public void SetDiluc()
    {
        characterManager.SetCurrentCharacter(dilucData);

        DebugCharacter();
    }
    public void SetBarbara()
    {
        characterManager.SetCurrentCharacter(barbaraData);

        DebugCharacter();
    }
    public void Ascension()
    {
        CharacterRunTime runtime = characterManager.CurrentCharacter;

        runtime.Ascension();

        characterStatCalculator.ApplyStat(runtime);

        DebugCharacter();
    }
    private void DebugCharacter()
    {
        CharacterRunTime runtime = characterManager.CurrentCharacter;

        Debug.Log($"캐릭터 : {runtime.Data.CharacterName}");
        Debug.Log($"돌파 단계 : {runtime.AscensionPhase}");
        Debug.Log($"레벨 : {runtime.Level}");
        Debug.Log($"최대 체력 : {runtime.MaxHP}");
        Debug.Log($"현재 체력 : {runtime.CurrentHP}");
    }
}

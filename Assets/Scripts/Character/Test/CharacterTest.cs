using UnityEngine;

public class CharacterTest : MonoBehaviour
{
    [SerializeField] private CharacterStatCalculator characterStatCalculator;
    [SerializeField] private CharacterManager characterManager;
    [SerializeField] private CharacterParty characterParty;
    private void Awake()
    {
        InputManager.Instance.OnSwitchCharacter += OnSwitchCharacter;
    }
    public void LevelUp()
    {
        CharacterRunTime runtime = characterManager.CurrentCharacter;

        runtime.LevelUp();

        characterStatCalculator.ApplyStat(runtime);

        DebugCharacter();

    }
    public void Ascension()
    {
        CharacterRunTime runtime = characterManager.CurrentCharacter;

        runtime.Ascension();

        characterStatCalculator.ApplyStat(runtime);

        DebugCharacter();
    }
    public void OnSwitchCharacter(int index)
    {
        characterParty.SelectCharacter(index-1);
        DebugCharacter();
    }
    private void DebugCharacter()
    {
        Debug.ClearDeveloperConsole();

        CharacterRunTime runtime = characterManager.CurrentCharacter;

        Debug.Log($"캐릭터 : {runtime.Data.CharacterName}");
        Debug.Log($"돌파 단계 : {runtime.AscensionPhase}");
        Debug.Log($"레벨 : {runtime.Level}");
        Debug.Log($"최대 체력 : {runtime.MaxHP}");
        Debug.Log($"현재 체력 : {runtime.CurrentHP}");
        
    }
}

public static class CharacterLevelRule
{
    private static readonly int[] MaxLevels =
    {
        20,
        40,
        50,
        60,
        70,
        80,
        90
    };
    public static int GetMaxLevel(int ascensionPhase)
    {
        if(ascensionPhase < 0 || ascensionPhase >= MaxLevels.Length)
        {
            return 20;
        }
        return MaxLevels[ascensionPhase];
    }
}

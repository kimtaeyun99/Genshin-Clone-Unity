using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public PlayerSaveData player;

    public List<string> partyCharacterIds;
    public int currentPartyIndex;

    public List<CharacterSaveData> characters;
}

[Serializable]
public class PlayerSaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationY;
}

[Serializable]
public class CharacterSaveData
{
    public string characterId;

    public int level;
    public int ascensionPhase;

    public int currentHP;
    public float currentEnergy;

    public bool isDead;
}
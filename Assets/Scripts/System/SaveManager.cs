using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance
    {
        get;
        private set;
    }

    private string SavePath =>
        Path.Combine(
            Application.persistentDataPath,
            "save.json"
        );

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SaveGame()
    {
        if (CharacterManager.Instance == null ||
            CharacterParty.Instance == null)
        {
            return;
        }

        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            return;
        }

        GameSaveData saveData =
            new GameSaveData();

        SavePlayer(saveData);
        SaveParty(saveData);
        SaveCharacters(saveData);

        string json =
            JsonUtility.ToJson(
                saveData,
                true
            );

        File.WriteAllText(
            SavePath,
            json
        );

        Debug.Log(
            $"게임 저장 완료\n{SavePath}"
        );
    }

    private void SavePlayer(
        GameSaveData saveData)
    {
        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            return;
        }

        Vector3 position =
            player.transform.position;

        saveData.player =
            new PlayerSaveData
            {
                positionX = position.x,
                positionY = position.y,
                positionZ = position.z,

                rotationY =
                    player.transform.eulerAngles.y
            };
    }

    private void SaveParty(
        GameSaveData saveData)
    {
        CharacterParty party =
            CharacterParty.Instance;

        saveData.partyCharacterIds =
            new List<string>();

        foreach (
            CharacterData data
            in party.PartyMembers)
        {
            saveData.partyCharacterIds.Add(
                data.ID
            );
        }

        saveData.currentPartyIndex =
            party.CurrentIndex;
    }

    private void SaveCharacters(
        GameSaveData saveData)
    {
        CharacterManager manager =
            CharacterManager.Instance;

        saveData.characters =
            new List<CharacterSaveData>();

        foreach (
            CharacterData data
            in manager.CharacterDatas)
        {
            CharacterRunTime runTime =
                manager.GetCharacter(data);

            if (runTime == null)
            {
                continue;
            }

            CharacterSaveData characterSave =
                new CharacterSaveData
                {
                    characterId = data.ID,

                    level = runTime.Level,
                    ascensionPhase =
                        runTime.AscensionPhase,

                    currentHP =
                        runTime.CurrentHP,

                    currentEnergy =
                        runTime.CurrentEnergy,

                    isDead =
                        runTime.IsDead
                };

            saveData.characters.Add(
                characterSave
            );
        }
    }
    private void LoadParty(GameSaveData saveData)
    {
        CharacterManager manager =
            CharacterManager.Instance;

        CharacterParty party =
            CharacterParty.Instance;

        if (saveData.partyCharacterIds == null)
        {
            return;
        }

        List<CharacterData> loadedParty =
            new List<CharacterData>();

        foreach (string characterId
                 in saveData.partyCharacterIds)
        {
            CharacterData data =
                manager.GetCharacterData(characterId);

            if (data == null)
            {
                Debug.LogWarning(
                    $"CharacterData를 찾을 수 없음 : {characterId}"
                );

                continue;
            }

            loadedParty.Add(data);
        }

        party.LoadParty(
            loadedParty,
            saveData.currentPartyIndex
        );

        if (party.Count == 0)
        {
            return;
        }

        // 저장 당시 사용 중이던 캐릭터 선택
        if (!party.SelectCharacter(party.CurrentIndex))
        {
            // 저장 당시 캐릭터가 사망 상태라면
            // 살아있는 캐릭터 선택
            party.SelectNextAlive();
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    public bool HasSaveData()
    {
        return File.Exists(SavePath);
    }
    public void LoadGame()
    {
        if (!HasSaveData())
        {
            Debug.LogWarning("저장 데이터가 없습니다.");
            return;
        }

        string json =
            File.ReadAllText(SavePath);

        GameSaveData saveData =
            JsonUtility.FromJson<GameSaveData>(json);

        if (saveData == null)
        {
            Debug.LogError("저장 데이터 로드 실패");
            return;
        }

        LoadCharacters(saveData);
        LoadParty(saveData);
        LoadPlayer(saveData);

        Debug.Log("게임 로드 완료");
    }
    private void LoadCharacters(GameSaveData saveData)
    {
        if (saveData.characters == null)
        {
            return;
        }

        CharacterManager manager =
            CharacterManager.Instance;

        foreach (CharacterSaveData characterSave
                 in saveData.characters)
        {
            CharacterData data =
                manager.GetCharacterData(
                    characterSave.characterId
                );

            if (data == null)
            {
                Debug.LogWarning(
                    $"CharacterData를 찾을 수 없음 : " +
                    $"{characterSave.characterId}"
                );

                continue;
            }

            CharacterRunTime runTime =
                manager.GetCharacter(data);

            if (runTime == null)
            {
                continue;
            }

            // 레벨 / 돌파 복구
            runTime.LoadGrowthData(
                characterSave.level,
                characterSave.ascensionPhase
            );

            // 레벨 / 돌파에 맞춰 스탯 다시 계산
            manager.RefreshCharacterStat(runTime);

            // HP / 에너지 / 사망 상태 복구
            runTime.LoadStateData(
                characterSave.currentHP,
                characterSave.currentEnergy,
                characterSave.isDead
            );
        }
    }
    private void LoadPlayer(GameSaveData saveData)
    {
        if (saveData.player == null)
        {
            return;
        }

        PlayerController player =
            FindFirstObjectByType<PlayerController>();

        if (player == null)
        {
            Debug.LogWarning(
                "PlayerController를 찾을 수 없습니다."
            );

            return;
        }

        Vector3 position =
            new Vector3(
                saveData.player.positionX,
                saveData.player.positionY,
                saveData.player.positionZ
            );

        Quaternion rotation =
            Quaternion.Euler(
                0f,
                saveData.player.rotationY,
                0f
            );

        player.Movement.Teleport(
            position,
            rotation
        );

        player.StateMachine.ChangeState(
            PlayerStateType.Idle
        );
    }
    private bool loadRequested;

    public bool LoadRequested => loadRequested;

    public void SetLoadRequested(bool value)
    {
        loadRequested = value;
    }
}
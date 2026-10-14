using System;
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

    private PlayerController player;

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

    public void RegisterPlayer(PlayerController controller)
    {
        player = controller;
    }

    public void UnregisterPlayer(PlayerController controller)
    {
        if (player == controller)
            player = null;
    }

    public void SaveGame()
    {
        // 씬 전환 중 매니저/플레이어가 없다면 기존 저장 파일은 유지한다.
        if (CharacterManager.Instance == null ||
            CharacterParty.Instance == null)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        try
        {
            GameSaveData saveData = new GameSaveData();
            SavePlayer(saveData, player);
            SaveParty(saveData);
            SaveCharacters(saveData);

            string json = JsonUtility.ToJson(saveData, true);
            WriteSaveFile(json);
        }
        catch (Exception exception)
        {
            // 저장 실패 시 예외를 앱 종료 콜백까지 전파하지 않는다.
            // 기존 save.json은 임시 파일 교체가 성공한 경우에만 변경된다.
            Debug.LogError($"[SaveManager] 저장 실패: {exception}", this);
        }
        Debug.Log($"저장 위치: {SavePath}");
    }

    private void SavePlayer(
        GameSaveData saveData,
        PlayerController player)
    {

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
    private void WriteSaveFile(string json)
    {
        // 동일 디렉터리의 임시 파일에 먼저 저장하여 중간에 쓰기가 실패해도
        // 기존 save.json을 직접 손상시키지 않는다.
        string savePath = SavePath;
        string tempPath = savePath + ".tmp";
        string backupPath = savePath + ".bak";

        try
        {
            File.WriteAllText(tempPath, json);

            if (File.Exists(savePath))
            {
                // Windows(PC)에서는 기존 파일 교체와 직전 파일 백업을 수행한다.
                File.Replace(tempPath, savePath, backupPath);
            }
            else
            {
                File.Move(tempPath, savePath);
            }
        }
        finally
        {
            // 교체에 실패하여 남은 임시 파일만 정리한다.
            // 삭제 실패가 원래 예외를 덮어쓰지 않도록 한다.
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[SaveManager] 임시 파일 정리 실패: {exception}", this);
            }
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

        if (!party.SelectCharacter(party.CurrentIndex))
        {
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
            return;

        // 파일 읽기·역직렬화가 성공하기 전까지 Runtime 상태를 변경하지 않는다.
        GameSaveData saveData;
        try
        {
            string json = File.ReadAllText(SavePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (Exception exception)
        {
            Debug.LogError($"[SaveManager] 저장 파일 읽기 실패: {exception}", this);
            return;
        }

        if (saveData == null)
        {
            Debug.LogWarning("[SaveManager] 저장 데이터가 비어 있어 불러오기를 중단합니다.", this);
            return;
        }

        // 캐릭터 스탯·파티 선택·플레이어 위치의 기존 복원 순서는 유지한다.
        LoadCharacters(saveData);
        LoadParty(saveData);
        LoadPlayer(saveData);
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

        if (player == null)
        {
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
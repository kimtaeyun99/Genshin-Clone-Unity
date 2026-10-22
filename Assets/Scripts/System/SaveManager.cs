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
            Debug.LogError($"[SaveManager] 저장 실패: {exception}");
        }
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

                    currentHP =
                        runTime.CurrentHP,

                    currentEnergy =
                        runTime.CurrentEnergy,
                };

            saveData.characters.Add(
                characterSave
            );
        }
    }
    private void WriteSaveFile(string json)
    {
        string savePath = SavePath;
        string tempPath = savePath + ".tmp";
        string backupPath = savePath + ".bak";

        try
        {
            File.WriteAllText(tempPath, json);

            if (File.Exists(savePath))
            {
                File.Replace(tempPath, savePath, backupPath);
            }
            else
            {
                File.Move(tempPath, savePath);
            }
        }
        finally
        {
            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[SaveManager] 임시 파일 삭제 실패: {exception}"
                );
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
        string savePath = SavePath;

        return File.Exists(savePath) ||
               File.Exists(savePath + ".bak") ||
               File.Exists(savePath + ".tmp");
    }
    public void LoadGame()
    {
        if (!HasSaveData())
            return;

        string savePath = SavePath;
        string[] loadPaths =
        {
            savePath,
            savePath + ".bak",
            savePath + ".tmp"
        };

        foreach (string path in loadPaths)
        {
            if (!TryReadSaveFile(path, out GameSaveData saveData, out string json))
            {
                continue;
            }

            if (path != savePath)
            {
                RestoreSaveFile(json, path);
            }

            LoadCharacters(saveData);
            LoadParty(saveData);
            LoadPlayer(saveData);
            return;
        }

        Debug.LogError(
            "[SaveManager] 불러오기 실패: 원본, 백업, 임시 파일에서 유효한 저장 데이터를 찾지 못했습니다."
        );
    }

    private bool TryReadSaveFile(
        string path,
        out GameSaveData saveData,
        out string json)
    {
        saveData = null;
        json = null;

        if (!File.Exists(path))
        {
            return false;
        }

        try
        {
            string loadedJson = File.ReadAllText(path);

            if (string.IsNullOrWhiteSpace(loadedJson))
            {
                throw new InvalidDataException("저장 파일이 비어 있습니다.");
            }

            GameSaveData loadedData =
                JsonUtility.FromJson<GameSaveData>(loadedJson);

            if (!IsValidSaveData(loadedData))
            {
                throw new InvalidDataException("필수 저장 데이터가 누락되었거나 올바르지 않습니다.");
            }

            saveData = loadedData;
            json = loadedJson;
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                $"[SaveManager] 저장 파일 읽기 실패 ({Path.GetFileName(path)}): {exception.Message}"
            );
            return false;
        }
    }

    private static bool IsValidSaveData(GameSaveData saveData)
    {
        if (saveData == null ||
            saveData.player == null ||
            saveData.partyCharacterIds == null ||
            saveData.partyCharacterIds.Count == 0 ||
            saveData.characters == null ||
            saveData.characters.Count == 0)
        {
            return false;
        }

        foreach (string characterId in saveData.partyCharacterIds)
        {
            if (string.IsNullOrWhiteSpace(characterId))
            {
                return false;
            }
        }

        foreach (CharacterSaveData character in saveData.characters)
        {
            if (character == null || string.IsNullOrWhiteSpace(character.characterId))
            {
                return false;
            }
        }

        return true;
    }

    private void RestoreSaveFile(string json, string sourcePath)
    {
        try
        {
            // 복구 원본인 .bak, .tmp를 보존하고 검증한 내용만 기록한다.
            File.WriteAllText(SavePath, json);

            Debug.LogWarning(
                $"[SaveManager] {Path.GetFileName(sourcePath)}에서 save.json을 복구했습니다."
            );
        }
        catch (Exception exception)
        {
            // 파일 복구에 실패해도 이미 읽은 데이터로 게임을 불러온다.
            Debug.LogWarning(
                $"[SaveManager] save.json 복구 실패, {Path.GetFileName(sourcePath)}의 데이터로 불러옵니다: {exception.Message}"
            );
        }
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

            runTime.LoadStateData(
                characterSave.currentHP,
                characterSave.currentEnergy
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

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

        GameSaveData saveData;
        try
        {
            string json = File.ReadAllText(SavePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
        }
        catch (Exception exception)
        {
            return;
        }

        if (saveData == null)
        {
            return;
        }

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
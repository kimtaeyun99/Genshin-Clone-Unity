using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

public class CharacterDataImporter : EditorWindow
{
    [SerializeField]
    private const string CharacterCsvPath =
        "Assets/Scripts/Editor/CSV/Character.csv";

    [SerializeField]
    private const string CharacterDataFolder =
        "Assets/Data/Character";

    [SerializeField]
    private const string CharacterPrefabFolder =
        "Assets/Prefabs/Character";

    [SerializeField]
    private const string CharacterAscensionCsvPath =
    "Assets/Scripts/Editor/CSV/CharacterAscension.csv";

    [SerializeField]
    private const string CharacterAscensionDataFolder =
        "Assets/Data/CharacterAscension";


    [MenuItem("Tools/Data Importer/Character")]
    public static void OpenWindow()
    {
        GetWindow<CharacterDataImporter>("Character Importer");
    }


    private void OnGUI()
    {
        GUILayout.Label("Character Data Importer", EditorStyles.boldLabel);

        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "CSV",
            CharacterCsvPath
        );

        EditorGUILayout.LabelField(
            "Output",
            CharacterDataFolder
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Import All Character Data"))
        {
            ImportCharacterAscensions();
            ImportCharacters();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Import Character Data"))
        {
            ImportCharacters();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Import Character Ascension Data"))
        {
            ImportCharacterAscensions();
        }
    }


    private void ImportCharacters()
    {
        // CSV 읽기
        List<Dictionary<string, string>> rows =
            CSVParser.ParseWithHeader(CharacterCsvPath);

        if (rows == null || rows.Count == 0)
        {
            Debug.LogError(
                $"Character CSV 데이터가 없습니다.\n{CharacterCsvPath}"
            );

            return;
        }

        EnsureFolder(CharacterDataFolder);

        int importCount = 0;

        foreach (Dictionary<string, string> row in rows)
        {
            string id = CSVParser.Get(row, "ID");
            string characterName = CSVParser.Get(row, "Name");
            string prefabName = CSVParser.Get(row, "PrefabName");

            // ID가 없으면 생성 불가능
            if (string.IsNullOrWhiteSpace(id))
            {
                Debug.LogWarning(
                    "Character ID가 비어있는 행을 건너뜁니다."
                );

                continue;
            }


            // ElementType 변환
            if (!Enum.TryParse(
                    CSVParser.Get(row, "ElementType"),
                    true,
                    out ElementType elementType))
            {
                Debug.LogError(
                    $"[{id}] ElementType이 잘못되었습니다."
                );

                continue;
            }


            // WeaponType 변환
            if (!Enum.TryParse(
                    CSVParser.Get(row, "WeaponType"),
                    true,
                    out WeaponType weaponType))
            {
                Debug.LogError(
                    $"[{id}] WeaponType이 잘못되었습니다."
                );

                continue;
            }

            // 기본 스탯
            int hp =
                CSVParser.GetInt(row, "HP");

            int hpPerLevel =
                CSVParser.GetInt(row, "HPPerLevel");

            int atk =
                CSVParser.GetInt(row, "ATK");

            int atkPerLevel =
                CSVParser.GetInt(row, "ATKPerLevel");

            int def =
                CSVParser.GetInt(row, "DEF");

            int defPerLevel =
                CSVParser.GetInt(row, "DEFPerLevel");

            int elementalMastery =
                CSVParser.GetInt(row, "ElementalMastery");


            // 캐릭터 Prefab 검색
            GameObject prefab =
                FindCharacterPrefab(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning(
                    $"[{id}] Prefab을 찾을 수 없습니다: {prefabName}"
                );
            }

            CharacterAscensionData ascensionData = FindOrCreateCharacterAscensionData(id);

            if (ascensionData == null)
            {
                Debug.LogWarning(
                    $"[{id}] CharacterAscensionData를 찾을 수 없습니다."
                );
            }
            // CharacterData SO 생성 또는 기존 데이터 가져오기
            CharacterData characterData =
                FindOrCreateCharacterData(id);

            characterData.SetData(
                id,
                characterName,
                prefab,
                elementType,
                weaponType,
                hp,
                hpPerLevel,
                atk,
                atkPerLevel,
                def,
                defPerLevel,
                elementalMastery,
                ascensionData
            );


            EditorUtility.SetDirty(characterData);

            importCount++;
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Character Import 완료 : {importCount}개"
        );
    }

    private CharacterData FindOrCreateCharacterData(string id)
    {
        string assetPath =
            $"{CharacterDataFolder}/{id}.asset";

        CharacterData data =
            AssetDatabase.LoadAssetAtPath<CharacterData>(
                assetPath
            );

        // 이미 존재하면 그대로 반환
        if (data != null)
        {
            return data;
        }


        // 없으면 새로 생성
        data =
            CreateInstance<CharacterData>();

        AssetDatabase.CreateAsset(
            data,
            assetPath
        );

        return data;
    }


    private GameObject FindCharacterPrefab(string prefabName)
    {
        if (string.IsNullOrWhiteSpace(prefabName))
            return null;

        prefabName = prefabName.Trim();

        // Character 폴더 및 모든 하위 폴더 검색
        string[] guids = AssetDatabase.FindAssets(
            "t:Model",
            new[] { CharacterPrefabFolder }
        );

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);

            // FBX 파일 이름과 PrefabName이 같은 경우
            if (string.Equals(
                fileName,
                prefabName,
                StringComparison.OrdinalIgnoreCase))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }

        Debug.LogWarning($"FBX를 찾을 수 없습니다: {prefabName}");
        return null;
    }


    private void EnsureFolder(string folderPath)
    {
        string[] folders =
            folderPath.Split('/');

        string currentPath =
            folders[0];


        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath =
                $"{currentPath}/{folders[i]}";


            if (!AssetDatabase.IsValidFolder(nextPath))
            {
                AssetDatabase.CreateFolder(
                    currentPath,
                    folders[i]
                );
            }


            currentPath =
                nextPath;
        }
    }

    private void ImportCharacterAscensions()
    {
        if (!File.Exists(CharacterAscensionCsvPath))
        {
            Debug.LogError(
                $"CharacterAscension.csv 파일을 찾을 수 없습니다.\n" +
                $"경로: {CharacterAscensionCsvPath}"
            );

            return;
        }

        List<Dictionary<string, string>> rows =
            CSVParser.ParseWithHeader(CharacterAscensionCsvPath);

        if (rows == null || rows.Count == 0)
        {
            Debug.LogError(
                "CharacterAscension CSV 데이터가 없습니다."
            );

            return;
        }

        EnsureFolder(CharacterAscensionDataFolder);

        int importCount = 0;

        foreach (Dictionary<string, string> row in rows)
        {
            string characterId =
                CSVParser.Get(row, "CharacterID");

            if (string.IsNullOrWhiteSpace(characterId))
            {
                Debug.LogWarning(
                    "CharacterID가 비어있는 행을 건너뜁니다."
                );

                continue;
            }

            // 돌파 스탯 종류
            if (!Enum.TryParse(
                    CSVParser.Get(row, "AscensionStatType"),
                    true,
                    out StatType ascensionStatType))
            {
                Debug.LogError(
                    $"[{characterId}] AscensionStatType이 잘못되었습니다."
                );

                continue;
            }

            // 돌파 단계별 수치
            float[] bonusValues = new float[7];

            for (int i = 0; i < bonusValues.Length; i++)
            {
                bonusValues[i] =
                    CSVParser.GetFloat(row, $"Phase{i}");
            }

            CharacterAscensionData data =
                FindOrCreateCharacterAscensionData(characterId);

            data.SetData(
                characterId,
                ascensionStatType,
                bonusValues
            );

            EditorUtility.SetDirty(data);

            importCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log(
            $"Character Ascension Import 완료 : {importCount}개"
        );
    }

    private CharacterAscensionData FindOrCreateCharacterAscensionData(
    string characterId)
    {
        string assetPath =
            $"{CharacterAscensionDataFolder}/{characterId}.asset";

        CharacterAscensionData data =
            AssetDatabase.LoadAssetAtPath<CharacterAscensionData>(
                assetPath
            );

        if (data != null)
        {
            return data;
        }

        data =
            CreateInstance<CharacterAscensionData>();

        AssetDatabase.CreateAsset(
            data,
            assetPath
        );

        return data;
    }
}
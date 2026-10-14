using System;
using System.Collections.Generic;
using UnityEngine;

public class BossWaveSpawner : MonoBehaviour
{
    [Serializable]
    private class SpawnData
    {
        public GameObject monsterPrefab;
        public Transform spawnPoint;
    }

    [Serializable]
    private class WaveData
    {
        public string waveName;
        public SpawnData[] spawnDatas;
    }

    [Header("Wave")]
    [SerializeField] private WaveData[] waves;

    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float startRange = 15f;

    [Header("Wave Delay")]
    [SerializeField] private float nextWaveDelay = 2f;

    private readonly List<GameObject> spawnedMonsters = new();
    private readonly Dictionary<MonsterRunTime, Action> deathHandlers = new();

    private int currentWaveIndex;
    private bool isStarted;
    private bool isWaveActive;
    private bool isWaitingNextWave;
    private bool isCleared;
    private float waveDelayTimer;

    private void Update()
    {
        if (isCleared)
            return;

        if (!isStarted)
        {
            CheckStartRange();
            return;
        }

        if (isWaveActive)
            return;

        if (isWaitingNextWave)
            UpdateWaveDelay();
    }

    private void OnDestroy()
    {
        UnsubscribeFromMonsterDeaths();
    }

    private void CheckStartRange()
    {
        if (!IsPlayerInRange())
            return;

        StartBattle();
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        Vector3 direction = player.position - transform.position;
        return direction.sqrMagnitude <= startRange * startRange;
    }

    private void StartBattle()
    {
        if (waves == null || waves.Length == 0)
            return;

        isStarted = true;
        currentWaveIndex = 0;
        SpawnWave();
    }

    private void SpawnWave()
    {
        if (currentWaveIndex >= waves.Length)
        {
            ClearBossRoom();
            return;
        }

        UnsubscribeFromMonsterDeaths();

        for (int i = spawnedMonsters.Count - 1; i >= 0; i--)
        {
            if (spawnedMonsters[i] == null)
                spawnedMonsters.RemoveAt(i);
        }

        WaveData wave = waves[currentWaveIndex];
        if (wave != null && wave.spawnDatas != null)
        {
            foreach (SpawnData spawnData in wave.spawnDatas)
            {
                if (spawnData == null || spawnData.monsterPrefab == null ||
                    spawnData.spawnPoint == null)
                    continue;

                if (!spawnData.monsterPrefab.TryGetComponent<MonsterController>(out _) ||
                    !spawnData.monsterPrefab.TryGetComponent<Health>(out _))
                {
                    Debug.LogError(
                        $"[BossWaveSpawner] '{spawnData.monsterPrefab.name}' 프리팹 루트에 MonsterController와 Health가 필요합니다.",
                        this);
                    continue;
                }

                GameObject monster = Instantiate(
                    spawnData.monsterPrefab,
                    spawnData.spawnPoint.position,
                    spawnData.spawnPoint.rotation
                );

                MonsterRunTime runtime = monster.GetComponent<MonsterController>().Runtime;
                spawnedMonsters.Add(monster);

                Action handler = () => HandleMonsterDead(runtime);
                deathHandlers.Add(runtime, handler);
                runtime.OnDead += handler;
            }
        }

        isWaveActive = true;
        isWaitingNextWave = false;

        // 기존 버전처럼 비어 있는 웨이브는 한 단계 넘긴다.
        if (deathHandlers.Count == 0)
            OnWaveClear();
    }

    private void HandleMonsterDead(MonsterRunTime runtime)
    {
        if (!deathHandlers.TryGetValue(runtime, out Action handler))
            return;

        runtime.OnDead -= handler;
        deathHandlers.Remove(runtime);

        if (isWaveActive && deathHandlers.Count == 0)
            OnWaveClear();
    }

    private void OnWaveClear()
    {
        isWaveActive = false;
        currentWaveIndex++;

        if (currentWaveIndex >= waves.Length)
        {
            ClearBossRoom();
            return;
        }

        isWaitingNextWave = true;
        waveDelayTimer = nextWaveDelay;
    }

    private void UpdateWaveDelay()
    {
        waveDelayTimer -= Time.deltaTime;
        if (waveDelayTimer > 0f)
            return;

        waveDelayTimer = 0f;
        isWaitingNextWave = false;
        SpawnWave();
    }

    private void ClearBossRoom()
    {
        UnsubscribeFromMonsterDeaths();
        isCleared = true;
        isWaveActive = false;
        isWaitingNextWave = false;
    }

    public void ResetSpawner()
    {
        UnsubscribeFromMonsterDeaths();

        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
                Destroy(monster);
        }

        spawnedMonsters.Clear();
        currentWaveIndex = 0;
        isStarted = false;
        isWaveActive = false;
        isWaitingNextWave = false;
        isCleared = false;
        waveDelayTimer = 0f;
    }

    private void UnsubscribeFromMonsterDeaths()
    {
        foreach (KeyValuePair<MonsterRunTime, Action> entry in deathHandlers)
            entry.Key.OnDead -= entry.Value;

        deathHandlers.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, startRange);
    }
}

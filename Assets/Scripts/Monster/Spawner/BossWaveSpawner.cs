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

    private int currentWaveIndex;

    private bool isStarted;
    private bool isWaveActive;
    private bool isWaitingNextWave;
    private bool isCleared;

    private float waveDelayTimer;

    private void Update()
    {
        if (isCleared)
        {
            return;
        }

        if (!isStarted)
        {
            CheckStartRange();
            return;
        }

        if (isWaveActive)
        {
            CheckWaveClear();
            return;
        }

        if (isWaitingNextWave)
        {
            UpdateWaveDelay();
        }
    }

    private void CheckStartRange()
    {
        if (!IsPlayerInRange())
        {
            return;
        }

        StartBattle();
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
        {
            return false;
        }

        Vector3 direction =
            player.position - transform.position;

        return direction.sqrMagnitude <=
               startRange * startRange;
    }

    private void StartBattle()
    {
        if (waves == null || waves.Length == 0)
        {
            return;
        }

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

        spawnedMonsters.Clear();

        WaveData wave = waves[currentWaveIndex];

        foreach (SpawnData spawnData in wave.spawnDatas)
        {
            if (spawnData.monsterPrefab == null ||
                spawnData.spawnPoint == null)
            {
                continue;
            }

            GameObject monster = Instantiate(
                spawnData.monsterPrefab,
                spawnData.spawnPoint.position,
                spawnData.spawnPoint.rotation
            );

            spawnedMonsters.Add(monster);
        }

        isWaveActive = true;
        isWaitingNextWave = false;

        Debug.Log(
            $"{wave.waveName} 시작"
        );
    }

    private void CheckWaveClear()
    {
        for (int i = 0; i < spawnedMonsters.Count; i++)
        {
            if (spawnedMonsters[i] != null)
            {
                return;
            }
        }

        OnWaveClear();
    }

    private void OnWaveClear()
    {
        spawnedMonsters.Clear();

        isWaveActive = false;

        Debug.Log(
            $"{waves[currentWaveIndex].waveName} 클리어"
        );

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
        {
            return;
        }

        waveDelayTimer = 0f;
        isWaitingNextWave = false;

        SpawnWave();
    }

    private void ClearBossRoom()
    {
        isCleared = true;
        isWaveActive = false;
        isWaitingNextWave = false;

        Debug.Log("보스룸 클리어");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            startRange
        );
    }
}
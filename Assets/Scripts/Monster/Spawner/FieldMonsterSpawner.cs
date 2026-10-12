using System;
using System.Collections.Generic;
using UnityEngine;

public class FieldMonsterSpawner : MonoBehaviour
{
    [Serializable]
    private class SpawnData
    {
        public GameObject monsterPrefab;
        public Transform spawnPoint;
    }

    [Header("Spawn")]
    [SerializeField] private SpawnData[] spawnDatas;

    [Header("Detection")]
    [SerializeField] private Transform player;
    [SerializeField] private float spawnRange = 20f;

    [Header("Respawn")]
    [SerializeField] private float respawnTime = 180f;

    private readonly List<GameObject> spawnedMonsters = new();

    private bool isSpawned;
    private bool isRespawnWaiting;
    private float respawnTimer;

    private void Update()
    {
        if (!isSpawned)
        {
            if (!isRespawnWaiting)
            {
                CheckSpawnRange();
            }
            else
            {
                UpdateRespawnTimer();
            }

            return;
        }

        CheckAllMonstersDead();
    }

    private void CheckSpawnRange()
    {
        if (!IsPlayerInRange())
        {
            return;
        }

        SpawnMonsters();
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
               spawnRange * spawnRange;
    }

    private void SpawnMonsters()
    {
        spawnedMonsters.Clear();

        foreach (SpawnData spawnData in spawnDatas)
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

        if (spawnedMonsters.Count == 0)
        {
            return;
        }

        isSpawned = true;
        isRespawnWaiting = false;
        respawnTimer = 0f;
    }

    private void CheckAllMonstersDead()
    {
        for (int i = 0; i < spawnedMonsters.Count; i++)
        {
            if (spawnedMonsters[i] != null)
            {
                return;
            }
        }

        OnAllMonstersDead();
    }

    private void OnAllMonstersDead()
    {
        spawnedMonsters.Clear();

        isSpawned = false;
        isRespawnWaiting = true;
        respawnTimer = respawnTime;
    }

    private void UpdateRespawnTimer()
    {
        if (respawnTimer > 0f)
        {
            respawnTimer -= Time.deltaTime;
            return;
        }

        respawnTimer = 0f;
        isRespawnWaiting = false;

        // 시간이 끝났을 때 플레이어가 범위 안이면 즉시 재소환
        CheckSpawnRange();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            spawnRange
        );
    }
}
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
    private readonly Dictionary<MonsterRunTime, Action<MonsterRunTime>> deathHandlers = new();

    private bool isSpawned;
    private bool isRespawnWaiting;
    private float respawnTimer;

    private void Update()
    {
        if (isSpawned)
            return;

        if (isRespawnWaiting)
        {
            UpdateRespawnTimer();
            return;
        }

        CheckSpawnRange();
    }

    private void OnDestroy()
    {
        UnsubscribeFromMonsterDeaths();
    }

    private void CheckSpawnRange()
    {
        if (!IsPlayerInRange())
            return;

        SpawnMonsters();
    }

    private bool IsPlayerInRange()
    {
        if (player == null)
            return false;

        Vector3 direction = player.position - transform.position;
        return direction.sqrMagnitude <= spawnRange * spawnRange;
    }

    private void SpawnMonsters()
    {
        UnsubscribeFromMonsterDeaths();

        for (int i = spawnedMonsters.Count - 1; i >= 0; i--)
        {
            if (spawnedMonsters[i] == null)
                spawnedMonsters.RemoveAt(i);
        }

        int newMonsterCount = 0;
        foreach (SpawnData spawnData in spawnDatas)
        {
            if (spawnData.monsterPrefab == null || spawnData.spawnPoint == null)
                continue;

            if (!spawnData.monsterPrefab.TryGetComponent<MonsterController>(out _) ||
                !spawnData.monsterPrefab.TryGetComponent<Health>(out _))
            {
                continue;
            }

            GameObject monster = Instantiate(
                spawnData.monsterPrefab,
                spawnData.spawnPoint.position,
                spawnData.spawnPoint.rotation
            );

            MonsterRunTime runtime = monster.GetComponent<MonsterController>().Runtime;
            spawnedMonsters.Add(monster);
            newMonsterCount++;

            Action<MonsterRunTime> handler = HandleMonsterDead;

            deathHandlers.Add(runtime, handler);
            runtime.OnDead += handler;
        }

        if (newMonsterCount == 0)
            return;

        isSpawned = true;
        isRespawnWaiting = false;
        respawnTimer = 0f;
    }

    private void HandleMonsterDead(MonsterRunTime runtime)
    {
        if (!deathHandlers.TryGetValue(runtime, out Action<MonsterRunTime> handler))
            return;

        runtime.OnDead -= handler;
        deathHandlers.Remove(runtime);
        deathHandlers.Remove(runtime);

        if (isSpawned && deathHandlers.Count == 0)
            OnAllMonstersDead();
    }

    private void OnAllMonstersDead()
    {
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
        CheckSpawnRange();
    }

    public void ResetSpawner()
    {
        UnsubscribeFromMonsterDeaths();
        respawnTimer = 0f;
        isRespawnWaiting = false;

        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
                Destroy(monster);
        }

        spawnedMonsters.Clear();
        isSpawned = false;
    }

    private void UnsubscribeFromMonsterDeaths()
    {
        foreach (KeyValuePair<MonsterRunTime, Action<MonsterRunTime>> entry
                 in deathHandlers)
        {
            entry.Key.OnDead -= entry.Value;
        }

        deathHandlers.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}

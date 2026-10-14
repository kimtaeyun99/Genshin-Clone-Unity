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

    // OnDead에는 발생한 몬스터 정보가 전달되지 않으므로,
    // 몬스터별 콜백을 보관하여 한 번만 집계하고 정확히 구독 해제한다.
    private readonly Dictionary<MonsterHealth, Action> deathHandlers = new();

    private bool isSpawned;
    private bool isRespawnWaiting;
    private float respawnTimer;

    private void Update()
    {
        // 소환 중에는 매 프레임 몬스터 목록을 순회하지 않는다.
        // 마지막 몬스터의 OnDead가 전멸 상태로 전환한다.
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
        // 지난 소환 몬스터의 사망 연출 중인 오브젝트는 그대로 두되,
        // 실제로 파괴된 참조만 정리한다. ResetSpawner에서 남은 오브젝트도 제거할 수 있다.
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

            // 현재 MonsterController도 루트의 MonsterHealth를 사용한다.
            // 필수 컴포넌트가 없는 잘못된 프리팹은 생성하지 않고 명확히 알린다.
            if (!spawnData.monsterPrefab.TryGetComponent<MonsterHealth>(out _))
            {
                Debug.LogError(
                    $"[FieldMonsterSpawner] '{spawnData.monsterPrefab.name}' 프리팹에 MonsterHealth가 없어 OnDead 이벤트를 구독할 수 없습니다.",
                    this);
                continue;
            }

            GameObject monster = Instantiate(
                spawnData.monsterPrefab,
                spawnData.spawnPoint.position,
                spawnData.spawnPoint.rotation
            );

            spawnedMonsters.Add(monster);
            newMonsterCount++;

            MonsterHealth health = monster.GetComponent<MonsterHealth>();
            Action handler = () => HandleMonsterDead(health);
            deathHandlers.Add(health, handler);
            health.OnDead += handler;
        }

        if (newMonsterCount == 0)
            return;

        isSpawned = true;
        isRespawnWaiting = false;
        respawnTimer = 0f;
    }

    private void HandleMonsterDead(MonsterHealth health)
    {
        // 중복 OnDead가 발생해도 같은 몬스터를 두 번 집계하지 않는다.
        if (!deathHandlers.TryGetValue(health, out Action handler))
            return;

        if (health != null)
            health.OnDead -= handler;

        deathHandlers.Remove(health);

        if (isSpawned && deathHandlers.Count == 0)
            OnAllMonstersDead();
    }

    private void OnAllMonstersDead()
    {
        // 마지막 OnDead에서 즉시 리스폰 대기를 시작한다.
        // 사망 중인 모델은 기존 MonsterDeadState의 Destroy(delay)에 맡긴다.
        // spawnedMonsters 목록은 ResetSpawner가 시체까지 정리할 수 있도록 유지한다.
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

    public void ResetSpawner()
    {
        // Destroy 전에 구독을 해제하여 이전 웨이브 이벤트가 새 상태에 영향을 주지 않게 한다.
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
        foreach (KeyValuePair<MonsterHealth, Action> entry in deathHandlers)
        {
            if (entry.Key != null)
                entry.Key.OnDead -= entry.Value;
        }

        deathHandlers.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }
}

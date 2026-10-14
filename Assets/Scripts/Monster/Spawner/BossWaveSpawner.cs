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

    // 지금까지 생성한 몬스터를 보관한다. 마지막 OnDead 이후에도
    // 사망 연출 중인 오브젝트를 ResetSpawner()에서 정리하기 위함이다.
    private readonly List<GameObject> spawnedMonsters = new();

    // MonsterHealth.OnDead는 발생한 몬스터를 인자로 넘기지 않는다.
    // 몬스터별 콜백을 저장하여 중복 집계를 막고 구독을 정확히 해제한다.
    private readonly Dictionary<MonsterHealth, Action> deathHandlers = new();

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

        // 활성 웨이브는 OnDead 이벤트로만 전멸을 판정한다.
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

        // 이전 웨이브에서 파괴된 참조만 정리한다.
        // 아직 사망 연출 중인 오브젝트는 그대로 두고 ResetSpawner()에서 관리한다.
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
                if (spawnData == null ||
                    spawnData.monsterPrefab == null ||
                    spawnData.spawnPoint == null)
                {
                    continue;
                }

                // MonsterController가 요구하는 루트 MonsterHealth를 확인한다.
                if (!spawnData.monsterPrefab.TryGetComponent<MonsterHealth>(out _))
                {
                    Debug.LogError(
                        $"[BossWaveSpawner] '{spawnData.monsterPrefab.name}' 프리팹 루트에 MonsterHealth가 없어 OnDead를 구독할 수 없습니다.",
                        this);
                    continue;
                }

                GameObject monster = Instantiate(
                    spawnData.monsterPrefab,
                    spawnData.spawnPoint.position,
                    spawnData.spawnPoint.rotation
                );

                spawnedMonsters.Add(monster);

                MonsterHealth health = monster.GetComponent<MonsterHealth>();
                Action handler = () => HandleMonsterDead(health);
                deathHandlers.Add(health, handler);
                health.OnDead += handler;
            }
        }

        isWaveActive = true;
        isWaitingNextWave = false;

        // 비어 있거나 유효한 소환 데이터가 없는 웨이브는 다음 단계로 넘긴다.
        // 과거에는 다음 프레임의 CheckWaveClear()에서 비어 있음을 확인했다.
        if (deathHandlers.Count == 0)
            OnWaveClear();
    }

    private void HandleMonsterDead(MonsterHealth health)
    {
        // 중복 OnDead를 동일 몬스터에 대해 두 번 집계하지 않는다.
        if (!deathHandlers.TryGetValue(health, out Action handler))
            return;

        if (health != null)
            health.OnDead -= handler;

        deathHandlers.Remove(health);

        if (isWaveActive && deathHandlers.Count == 0)
            OnWaveClear();
    }

    private void OnWaveClear()
    {
        // 마지막 OnDead 즉시 다음 웨이브 대기 또는 최종 클리어 처리.
        // 사망 애니메이션/Destroy(delay)는 MonsterDeadState가 계속 담당한다.
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
        // Destroy 전에 먼저 구독을 해제하여 이전 웨이브의 콜백을 무효화한다.
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
        foreach (KeyValuePair<MonsterHealth, Action> entry in deathHandlers)
        {
            if (entry.Key != null)
                entry.Key.OnDead -= entry.Value;
        }

        deathHandlers.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, startRange);
    }
}

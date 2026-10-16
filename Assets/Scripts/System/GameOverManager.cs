using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance
    {
        get;
        private set;
    }

    [Header("UI")]
    [SerializeField] private GameObject gameOverUI;

    [Header("Player")]
    [SerializeField] private PlayerController player;

    [Header("Spawner")]
    [SerializeField] private FieldMonsterSpawner[] fieldMonsterSpawners;
    [SerializeField] private BossWaveSpawner bossWaveSpawner;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private void Awake()
    {
        Instance = this;

        spawnPosition = player.transform.position;
        spawnRotation = player.transform.rotation;

        gameOverUI.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverUI.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;

        CharacterManager.Instance.RespawnParty();

        player.Movement.Teleport(
            spawnPosition,
            spawnRotation
        );

        player.StateMachine.ChangeState(
            PlayerStateType.Idle
        );

        foreach (FieldMonsterSpawner spawner in fieldMonsterSpawners)
        {
            if (spawner != null)
                spawner.ResetSpawner();
        }

        if (bossWaveSpawner != null)
            bossWaveSpawner.ResetSpawner();

        gameOverUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
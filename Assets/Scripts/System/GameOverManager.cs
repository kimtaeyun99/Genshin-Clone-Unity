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

        Time.timeScale = 0f;
    }

    public void Restart()
    {
        // 시간 복구
        Time.timeScale = 1f;

        // 파티 전원 부활
        CharacterManager.Instance.RespawnParty();

        // 플레이어 시작 위치로 이동
        player.transform.SetPositionAndRotation(
            spawnPosition,
            spawnRotation
        );

        // Dead 상태에서 Idle 상태로 복귀
        player.StateMachine.ChangeState(
            PlayerStateType.Idle
        );

        // 게임오버 UI 닫기
        gameOverUI.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
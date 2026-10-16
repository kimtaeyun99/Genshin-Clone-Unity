using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnMainUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject returnMainPopup;

    [Header("Manager")]
    [SerializeField] private GameCursorManager cursorManager;

    [Header("Scene")]
    [SerializeField] private string mainSceneName = "Main";

    private void Start()
    {
        returnMainPopup.SetActive(false);

        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMenu += OpenPopup;
        }
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnMenu -= OpenPopup;
        }
    }

    private void OpenPopup()
    {
        returnMainPopup.SetActive(true);

        Time.timeScale = 0f;

        cursorManager.UnlockCursor();
    }

    public void ClosePopup()
    {
        returnMainPopup.SetActive(false);

        Time.timeScale = 1f;

        cursorManager.LockCursor();
    }

    public void ReturnToMain()
    {
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.SaveGame();
        }

        Time.timeScale = 1f;

        cursorManager.UnlockCursor();

        SceneManager.LoadScene(mainSceneName);
    }
}
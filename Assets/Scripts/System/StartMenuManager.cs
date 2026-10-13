using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuManager : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Button")]
    [SerializeField] private GameObject continueButton;

    private void Start()
    {
        UpdateContinueButton();
    }

    private void UpdateContinueButton()
    {
        if (continueButton == null)
        {
            return;
        }

        bool hasSaveData =
            SaveManager.Instance != null &&
            SaveManager.Instance.HasSaveData();

        continueButton.SetActive(hasSaveData);
    }

    public void NewGame()
    {
        SaveManager.Instance.SetLoadRequested(false);

        SceneManager.LoadScene(gameSceneName);
    }

    public void ContinueGame()
    {
        if (!SaveManager.Instance.HasSaveData())
        {
            return;
        }

        SaveManager.Instance.SetLoadRequested(true);

        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
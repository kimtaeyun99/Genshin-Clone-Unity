using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadHandler : MonoBehaviour
{
    private IEnumerator Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        yield return null;

        SaveManager saveManager = SaveManager.Instance;

        if (saveManager == null || !saveManager.LoadRequested)
            yield break;

        // 새 게임으로 씬을 다시 열어도 로드를 반복하지 않도록 먼저 해제한다.
        saveManager.SetLoadRequested(false);

        if (saveManager.LoadGame())
            yield break;

        Debug.LogWarning("[GameLoadHandler] 저장 데이터를 불러올 수 없어 새 게임을 시작합니다.");

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

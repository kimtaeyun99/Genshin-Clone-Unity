using UnityEngine;

public class GameCursorManager : MonoBehaviour
{
    private void Start()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogError("InputManager.Instance가 없음");
            return;
        }

        InputManager.Instance.OnToggle += ToggleCursor;

        Debug.Log("OnToggle 구독 완료");
    }

    private void OnDestroy()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnToggle -= ToggleCursor;
        }
    }

    private void ToggleCursor()
    {
        Debug.Log(
            $"ToggleCursor 호출됨 / 현재 상태: {Cursor.lockState}"
        );

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
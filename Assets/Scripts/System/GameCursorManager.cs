using UnityEngine;

public class GameCursorManager : MonoBehaviour
{
    private void Start()
    {
        if (InputManager.Instance == null)
        {
            return;
        }

        InputManager.Instance.OnToggle += ToggleCursor;
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
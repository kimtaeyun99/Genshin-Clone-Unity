using UnityEngine;

public class GameLoadHandler : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (SaveManager.Instance == null)
        {
            return;
        }

        if (!SaveManager.Instance.LoadRequested)
        {
            return;
        }

        SaveManager.Instance.LoadGame();
        SaveManager.Instance.SetLoadRequested(false);
    }
}
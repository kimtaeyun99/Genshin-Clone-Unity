using System.Collections;
using UnityEngine;

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

        saveManager.LoadGame();
        saveManager.SetLoadRequested(false);
    }
}
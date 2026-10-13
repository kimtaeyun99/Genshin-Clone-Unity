using UnityEngine;

public class GameLoadHandler : MonoBehaviour
{
    private void Start()
    {
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
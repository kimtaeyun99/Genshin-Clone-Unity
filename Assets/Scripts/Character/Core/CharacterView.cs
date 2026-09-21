using System.Collections.Generic;
using UnityEngine;

public class CharacterView : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;

    private readonly Dictionary<CharacterData, GameObject> viewCache = new();

    private GameObject currentView;

    public void ChangeView(CharacterData data)
    {
        if (data == null || data.Prefab == null)
            return;

        // 이미 현재 캐릭터라면 아무것도 안 함
        if (viewCache.TryGetValue(data, out GameObject cachedView)
            && cachedView == currentView)
        {
            return;
        }

        // 현재 보이는 캐릭터 끄기
        if (currentView != null)
        {
            currentView.SetActive(false);
        }

        // 캐시에 없으면 최초 1회 생성
        if (!viewCache.TryGetValue(data, out GameObject newView))
        {
            newView = Instantiate(data.Prefab, modelRoot);

            newView.transform.localPosition = Vector3.zero;
            newView.transform.localRotation = Quaternion.identity;

            // 생성 직후 캐시에 등록
            viewCache.Add(data, newView);
        }

        // 선택된 캐릭터만 활성화
        newView.SetActive(true);

        currentView = newView;
    }
}
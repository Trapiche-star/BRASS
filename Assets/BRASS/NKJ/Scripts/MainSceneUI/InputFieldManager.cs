using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 전역 InputField 포커스 감지 유틸리티
/// 어떤 InputField든 포커스 중이면 단축키를 막습니다.
/// </summary>
public class InputFieldManager : MonoBehaviour
{
    private static InputFieldManager instance;
    public static InputFieldManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("InputFieldManager");
                instance = go.AddComponent<InputFieldManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 현재 어떤 InputField라도 포커스 중인지 확인
    /// </summary>
    public static bool IsAnyInputFieldFocused()
    {
        // EventSystem이 없으면 false
        if (EventSystem.current == null)
            return false;

        // 현재 선택된 GameObject 가져오기
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
            return false;

        // TMP_InputField 체크
        if (selectedObject.TryGetComponent<TMP_InputField>(out var tmpInput))
        {
            return tmpInput.isFocused;
        }

        // 기본 Unity InputField 체크 (혹시 사용 중이라면)
        if (selectedObject.TryGetComponent<UnityEngine.UI.InputField>(out var unityInput))
        {
            return unityInput.isFocused;
        }

        return false;
    }

    /// <summary>
    /// 단축키를 받을 수 있는 상태인지 확인
    /// (InputField가 포커스 중이 아닐 때만 true)
    /// </summary>
    public static bool CanReceiveHotkey()
    {
        return !IsAnyInputFieldFocused();
    }

    /// <summary>
    /// 현재 포커스된 InputField의 이름 가져오기 (디버깅용)
    /// </summary>
    public static string GetFocusedInputFieldName()
    {
        if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
            return "None";

        var selected = EventSystem.current.currentSelectedGameObject;

        if (selected.TryGetComponent<TMP_InputField>(out _) ||
            selected.TryGetComponent<UnityEngine.UI.InputField>(out _))
        {
            return selected.name;
        }

        return "None";
    }
}
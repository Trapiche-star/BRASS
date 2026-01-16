using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    


    // 각 UI 패널 연결
    [Header("UI Panels")]
    public GameObject mainPanel;
    public GameObject gameHUD;
    public GameObject subMenu;
    public GameObject optionPanel;

    // 게임 상태 정의
    public enum UIState { Main, Playing, SubMenu, Options }
    private UIState currentState;

    void Start()
    {
        // 게임 시작 시 메인 메뉴만 활성화
        ChangeState(UIState.Main);
    }

    // 상태 변경 함수 (버튼에서 호출)
    public void ChangeState(UIState newState)
    {


        currentState = newState;

        // 모든 패널 일단 끄기
        mainPanel.SetActive(false);
        gameHUD.SetActive(false);
        subMenu.SetActive(false);
        optionPanel.SetActive(false);

        // 현재 상태에 맞는 패널만 켜기
        switch (currentState)
        {
            case UIState.Main:
                mainPanel.SetActive(true);
                break;
            case UIState.Playing:
                gameHUD.SetActive(true);
                break;
            case UIState.SubMenu:
                gameHUD.SetActive(true);
                subMenu.SetActive(true);
                break;
            case UIState.Options:
                optionPanel.SetActive(true);
                break;
        }

        Debug.Log($"현재 UI 상태: {newState}");
    }

    // 버튼 클릭 이벤트를 위한 헬퍼 함수들
    public void OnClickStart() => ChangeState(UIState.Playing);
    public void OnClickSubMenu() => ChangeState(UIState.SubMenu);
    public void OnClickOptions() => ChangeState(UIState.Options);
    public void OnClickBack() => ChangeState(UIState.Main);
    public void OnClickCloseSub() => ChangeState(UIState.Playing);
}
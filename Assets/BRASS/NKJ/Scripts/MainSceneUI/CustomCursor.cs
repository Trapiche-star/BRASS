using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{
    // 1. 싱글톤 인스턴스를 저장할 변수
    private static CustomCursor instance;

    private RectTransform rectTransform;
    private Image cursorImage;

    void Awake()
    {
        // --- [오류 방지: 중복 생성 체크] ---
        // 이미 인스턴스가 존재한다면 (예: 다른 씬에서 넘어온 경우)
        if (instance != null && instance != this)
        {
            // 새로 생성된 이 오브젝트의 최상위 부모(Canvas 포함)를 삭제하고 종료
            Destroy(transform.root.gameObject);
            return;
        }

        // 처음 생성되는 경우라면 자신을 인스턴스로 등록
        instance = this;

        // 씬이 바뀌어도 이 커서가 들어있는 캔버스가 파괴되지 않도록 설정
        DontDestroyOnLoad(transform.root.gameObject);
        // ----------------------------------

        rectTransform = GetComponent<RectTransform>();
        cursorImage = GetComponent<Image>();

        // 마우스 클릭 방지 설정 (실수로 켜져 있어도 코드에서 강제로 끔)
        if (cursorImage != null)
        {
            cursorImage.raycastTarget = false;
        }

        // 시스템 마우스 숨기기
        Cursor.visible = false;
    }

    void Update()
    {
        // 뉴 인풋 시스템 마우스 좌표 업데이트
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            rectTransform.position = mousePos;
        }
    }

    // 게임 창에서 마우스가 나갔다 들어올 때나, 일시정지 시 커서 상태를 강제로 다시 설정
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.visible = false;
        }
    }

    // 에디터에서 플레이 모드를 끄거나 오브젝트가 비활성화될 때 마우스 복구
    void OnDisable()
    {
        Cursor.visible = true;
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
    }
}
/*using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // 뉴 인풋 시스템 네임스페이스 추가

public class CustomCursor : MonoBehaviour
{
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 마우스 커서 숨기기 (이건 동일합니다)
        Cursor.visible = false;
    }

    void Update()
    {
        // New Input System 방식의 마우스 위치 가져오기
        if (Mouse.current != null)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            rectTransform.position = mousePos;
        }
    }

    // 오브젝트가 파괴될 때나 게임 종료 시 커서를 다시 보이게 설정 (에디터 편의용)
    void OnDisable()
    {
        Cursor.visible = true;
    }
}*/
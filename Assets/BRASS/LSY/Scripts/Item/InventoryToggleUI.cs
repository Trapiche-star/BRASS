using UnityEngine;
using UnityEngine.InputSystem;
using Team1;

namespace Team1
{
    public class InventoryToggleUI : MonoBehaviour
    {
        [Header("상점 UI 루트")]
        [SerializeField] private GameObject shopRoot;

        [Header("플레이어 입력")]
        [SerializeField] private PlayerInput playerInput;

        [Header("플레이어 이동 스크립트")]
        [SerializeField] private MonoBehaviour playerMoveScript;

        [Header("상호작용 컨트롤러")]
        [SerializeField] private InteractionController interactionController;

        public bool IsOpen => shopRoot != null && shopRoot.activeSelf;

        private void Awake()
        {
            if (shopRoot != null)
                shopRoot.SetActive(false);

            if (playerInput == null)
                playerInput = FindObjectOfType<PlayerInput>();

            if (interactionController == null)
                interactionController = FindObjectOfType<InteractionController>();
        }

        public void Open()
        {
            if (shopRoot == null) return;

            shopRoot.SetActive(true);

            // ✅ 플레이어 입력 차단
            if (playerInput != null)
                playerInput.enabled = false;

            // ✅ 이동 스크립트 차단 (중요!!)
            if (playerMoveScript != null)
                playerMoveScript.enabled = false;

            // ✅ 상호작용 차단 (G키)
            if (interactionController != null)
                interactionController.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            Time.timeScale = 0f;

            Debug.Log("🏪 상점 열림 - 모든 입력 차단");
        }

        public void Close()
        {
            if (shopRoot == null) return;

            shopRoot.SetActive(false);

            // ✅ 입력 복구
            if (playerInput != null)
                playerInput.enabled = true;

            // ✅ 이동 복구
            if (playerMoveScript != null)
                playerMoveScript.enabled = true;

            // ✅ 상호작용 복구
            if (interactionController != null)
                interactionController.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;

            Debug.Log("🏪 상점 닫힘 - 입력 복구");
        }

        public void Toggle()
        {
            if (IsOpen) Close();
            else Open();
        }
    }
}

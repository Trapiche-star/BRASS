using UnityEngine;

namespace Team1
{
    public class MinimapPlayerTracker : MonoBehaviour
    {
        [Header("Reference")]
        [SerializeField] private Transform player;          // 실제 플레이어
        [SerializeField] private RectTransform minimapRect; // 미니맵 이미지
        [SerializeField] private RectTransform playerDot;   // 빨간 점

        [Header("World Size (배 크기)")]
        [SerializeField] private Vector2 worldSize = new Vector2(50f, 30f);
        // X = 배 가로 길이, Z = 배 세로 길이 (월드 기준)

        private void Update()
        {
            if (player == null || minimapRect == null || playerDot == null)
                return;

            UpdateDotPosition();
        }

        private void UpdateDotPosition()
        {
            Vector3 playerPos = player.position;

            // 기존 정규화
            float normalizedX = (playerPos.x / worldSize.x) + 0.5f;
            float normalizedY = (playerPos.z / worldSize.y) + 0.5f;

            // ⭐ 90도 회전 적용 (시계방향)
            float rotatedX = normalizedY;
            float rotatedY = 1f - normalizedX;

            float mapWidth = minimapRect.rect.width;
            float mapHeight = minimapRect.rect.height;

            float uiX = (rotatedX - 0.5f) * mapWidth;
            float uiY = (rotatedY - 0.5f) * mapHeight;

            playerDot.anchoredPosition = new Vector2(uiX, uiY);
        }

    }
}

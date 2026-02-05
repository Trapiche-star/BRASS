using UnityEngine;

namespace Team1
{
    public class FloatingAndRotating : MonoBehaviour
    {
        [Header("회전 설정")]
        [SerializeField] private float rotateSpeed = 50f; // 초당 회전 각도

        [Header("둥실거림 설정")]
        [SerializeField] private float floatAmplitude = 0.5f; // 위아래 이동 범위
        [SerializeField] private float floatFrequency = 1f;  // 이동 속도 (주기)

        private Vector3 startPos;

        void Start()
        {
            // 시작 위치를 저장합니다.
            startPos = transform.position;
        }

        void Update()
        {
            // 1. 360도 회전 (Y축 기준)
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            // 2. 위아래 둥실거림 (Sin 그래프 활용)
            // Mathf.Sin(Time.time)은 -1에서 1 사이를 왕복합니다.
            float newY = startPos.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

            transform.position = new Vector3(startPos.x, newY, startPos.z);
        }
    }
}
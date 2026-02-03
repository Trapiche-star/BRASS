using UnityEngine;

namespace BRASS
{
    /// <summary>
    /// [ON HOLD]
    /// PlayerController에서 분리된
    /// 타겟 기반 자동 이동 / 접근 / 공격 진입 판단용 스크립트
    ///
    /// 현재는 구조 분리 목적이며
    /// 모든 로직은 주석 처리된 상태로 유지한다
    /// </summary>
    /*public class PlayerPathfinder : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private PlayerState state;
        [SerializeField] private CharacterController controller;

        [Header("Move Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float rotationSpeed = 15f;

        [Header("Stop Area")]
        [SerializeField] private float stopDistance = 1.8f;
        [SerializeField] private float stopAngle = 35f;

        private Transform target;
        private bool isPathfinding;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            state = GetComponent<PlayerState>();
        }

        private void Update()
        {
            if (!isPathfinding || target == null)
                return;

            UpdateMovement();
        }
        #endregion

        #region Custom Method
        // PlayerController / Combat / Skill 에서 호출 예정
        public void StartPathfinding(Transform newTarget)
        {
            target = newTarget;
            isPathfinding = true;
        }

        public void StopPathfinding()
        {
            isPathfinding = false;
            target = null;
        }

        private void UpdateMovement()
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;

            if (IsInStopArea(toTarget))
            {
                StopPathfinding();
                return;
            }

            Vector3 dir = toTarget.normalized;

            controller.Move(dir * moveSpeed * Time.deltaTime);

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion rot = Quaternion.LookRotation(dir);
                transform.rotation =
                    Quaternion.Slerp(transform.rotation, rot, rotationSpeed * Time.deltaTime);
            }
        }

        private bool IsInStopArea(Vector3 toTarget)
        {
            if (toTarget.magnitude > stopDistance)
                return false;

            float angle = Vector3.Angle(transform.forward, toTarget);
            return angle <= stopAngle;
        }
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            Vector3 origin = transform.position;
            Vector3 left =
                Quaternion.Euler(0, -stopAngle, 0) * transform.forward;
            Vector3 right =
                Quaternion.Euler(0, stopAngle, 0) * transform.forward;

            Gizmos.DrawLine(origin, origin + left * stopDistance);
            Gizmos.DrawLine(origin, origin + right * stopDistance);
            Gizmos.DrawWireSphere(origin, stopDistance);
        }
        #endregion
    }*/
}

using UnityEngine;

namespace BRASS
{
    /// 공격 입력 타이밍과 애니메이션 이벤트를 기반으로 단일 3타 공격의 콤보 진행을 제어하는 클래스
    public class PlayerCombat : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float comboInputWindow = 1.0f; // 다음 콤보 입력을 허용하는 유효 시간 범위
        [SerializeField] private PlayerAnimationController animationController; // 애니메이션 재생 제어 컴포넌트 참조
        [SerializeField] private PlayerController playerController; // 캐릭터 이동 제어 컴포넌트 참조

        [Header("Combo Step Move")]
        [SerializeField] private float[] comboStepDistances = { 0.15f, 0.2f, 0.25f }; // 각 타수별 전진 거리 데이터

        private Vector3 cachedAttackDirection; // 공격 시작 시 고정된 카메라 기준 정면 수평 방향
        private int attackInputCount; // 현재 시퀀스 내 누적된 공격 입력 횟수
        private float lastAttackInputTime; // 마지막으로 공격 입력이 발생한 게임 시간
        private bool isAttackSequenceActive; // 현재 공격 시퀀스가 진행 중인지 나타내는 상태 플래그
        #endregion

        #region Unity Event Method
        private void LateUpdate()
        {
            if (!isAttackSequenceActive) // 공격 시퀀스가 활성화 상태가 아니라면
                return; // 회전 고정 로직을 수행하지 않고 중단한다

            if (cachedAttackDirection == Vector3.zero) // 계산된 공격 방향이 유효하지 않다면
                return; // 회전 처리를 건너뛴다

            // 공격 시퀀스 도중에는 프레임 마지막에 캐릭터 회전을 고정된 방향으로 강제 유지한다
            transform.rotation = Quaternion.LookRotation(cachedAttackDirection);
        }
        #endregion

        #region Custom Method
        // 입력 시스템으로부터 기본 공격 버튼 신호를 수신하여 콤보 로직을 시작함
        public void OnBasicAttackStarted()
        {
            float now = Time.time; // 현재 시점의 게임 시간 측정

            if (!isAttackSequenceActive) // 현재 공격 중이 아닌 첫 번째 입력인 경우
            {
                isAttackSequenceActive = true; // 시퀀스 활성화
                attackInputCount = 1; // 입력 카운트 시작
                lastAttackInputTime = now; // 입력 시점 기록

                CacheAttackDirection(); // 카메라 방향을 기준으로 공격 방향 저장
                ApplyAttackRotation(); // 저장된 방향으로 캐릭터 회전 즉시 적용

                if (animationController != null) // 애니메이션 컨트롤러가 유효하다면
                    animationController.PlayAttack(); // 첫 공격 애니메이션 실행

                return; // 첫 입력 처리가 완료되었으므로 종료한다
            }

            if (now - lastAttackInputTime <= comboInputWindow) // 유효 시간 내에 추가 입력이 발생했다면
            {
                attackInputCount++; // 다음 타수 연계를 위해 입력 카운트 누적
                lastAttackInputTime = now; // 마지막 입력 시점 갱신
            }
        }

        // 애니메이션의 연계 판정 지점에서 호출되어 다음 콤보 진행 가능 여부를 판단함
        public bool OnComboSectionReached(int sectionIndex)
        {
            int requiredInput = sectionIndex + 1; // 연계에 필요한 최소 입력 수 계산

            if (attackInputCount < requiredInput) // 누적된 입력이 요구치보다 적은 경우
            {
                ForceEndAttack(); // 공격 시퀀스 강제 종료
                return false; // 연계 불가 반환
            }

            if (Time.time - lastAttackInputTime > comboInputWindow) // 입력 유효 시간을 초과한 경우
            {
                ForceEndAttack(); // 공격 시퀀스 강제 종료
                return false; // 연계 불가 반환
            }

            return true; // 조건을 만족하면 다음 콤보 애니메이션 진행 허가
        }

        // 전체 콤보 애니메이션이 완전히 끝난 시점에 호출되어 상태를 리셋함
        public void OnComboAnimationFinished()
        {
            ForceEndAttack(); // 모든 공격 데이터 초기화
        }

        // 애니메이션 이벤트 지점에서 고정된 공격 방향으로 캐릭터를 미세 전진시킴
        public void ApplyComboStep(int comboIndex)
        {
            if (cachedAttackDirection == Vector3.zero) // 방향 데이터가 없으면 이동하지 않는다
                return;

            if (comboIndex < 0 || comboIndex >= comboStepDistances.Length) // 인덱스가 범위를 벗어나면 무시한다
                return;

            Vector3 delta = cachedAttackDirection * comboStepDistances[comboIndex]; // 이동할 거리와 방향 계산

            if (playerController != null) // 컨트롤러 참조가 유효하다면
                playerController.MoveExternal(delta); // 외부 힘에 의한 이동 로직 수행
        }

        // 공격 시퀀스를 중단하고 애니메이션 및 상태 데이터를 초기값으로 되돌림
        private void ForceEndAttack()
        {
            isAttackSequenceActive = false; // 시퀀스 비활성
            attackInputCount = 0; // 카운트 초기화
            cachedAttackDirection = Vector3.zero; // 방향 데이터 초기화
            lastAttackInputTime = 0f; // 시간 기록 초기화

            if (animationController != null) // 애니메이션 컨트롤러가 있다면 연출 중단 요청
                animationController.StopAttack();
        }

        // 현재 메인 카메라가 바라보는 방향에서 수평 벡터를 추출하여 저장함
        private void CacheAttackDirection()
        {
            Camera cam = Camera.main; // 메인 카메라 참조
            if (cam == null) // 카메라가 존재하지 않는 경우
            {
                cachedAttackDirection = Vector3.zero; // 방향을 제로 벡터로 설정하고 중단한다
                return;
            }

            Vector3 forward = cam.transform.forward; // 카메라 전방 벡터 추출
            forward.y = 0f; // 수평 이동을 위해 수직 성분을 제거한다

            // 벡터 크기가 유효하면 정규화하고 유효하지 않으면 zero를 대입한다
            cachedAttackDirection = forward.sqrMagnitude < 0.01f
                ? Vector3.zero
                : forward.normalized;
        }

        // 캐릭터의 트랜스폼 회전값을 캐싱된 공격 방향으로 일치시킴
        private void ApplyAttackRotation()
        {
            if (cachedAttackDirection == Vector3.zero) // 유효한 방향 데이터가 없다면 회전하지 않는다
                return;

            transform.rotation = Quaternion.LookRotation(cachedAttackDirection); // 방향 일치 적용
        }
        #endregion
    }
}
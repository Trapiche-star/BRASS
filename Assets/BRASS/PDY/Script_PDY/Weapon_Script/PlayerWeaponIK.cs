using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace BRASS
{
    /// 양손무기 사용 시 왼손 IK(Target/Weight)를 제어하는 전담 클래스
    public class PlayerWeaponIK : MonoBehaviour
    {
        #region Variables
        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        #endregion

        #region Custom Methods

        // 왼손 그립 위치에 IK 타겟을 바인딩하고 가중치를 1로 설정
        public void BindLeftHand(Transform leftHandGrip)
        {
            if (leftHandGrip == null) return; // 그립 위치가 유효하지 않으면 중단

            leftHandIK.data.target = leftHandGrip;  
            leftHandIK.weight = 1f;
        }

        // 왼손 IK 타겟을 해제하고 가중치를 0으로 설정
        public void UnbindLeftHand()
        {
            leftHandIK.weight = 0f;
            leftHandIK.data.target = null;
        }

        // 왼손 IK 가중치 조절
        public void SetLeftHandWeight(float weight) 
        {
            leftHandIK.weight = weight;
        }
        #endregion
    }
}

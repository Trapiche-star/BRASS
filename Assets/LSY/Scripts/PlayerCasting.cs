using UnityEngine;

namespace Team1
{
    /// <summary>
    /// 플레이어와 정면에 있는 오브젝트와의 거리 구하기
    /// </summary>
    public class PlayerCasting : MonoBehaviour
    {
        #region Variables
        public static float distanceFromTarget;    //플레이어와 정면에 있는 오브젝트와의 거리

        [SerializeField]
        private float toTarget;     //플레이어와 타겟과의 거리
        #endregion

        #region Unity Event Method
        private void Update()
        {
            //오브젝트와의 거리 구하기
            RaycastHit hit;             //hit했을때 hit정보를 저장
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
                Interactive interactive = hit.transform.GetComponent<Interactive>();

                //Interactive한 오브젝트에 hit 성공하면
                if (interactive != null)
                {
                    distanceFromTarget = hit.distance;
                }
                else
                {
                    distanceFromTarget = Mathf.Infinity;
                }
                toTarget = distanceFromTarget;
            }
            else
            {
                distanceFromTarget = Mathf.Infinity;
                toTarget = distanceFromTarget;
            }
        }

        // 레이케스트 기즈모로 그리기
        private void OnDrawGizmosSelected()
        {
            //레이쏘는 거리
            float maxDistance = 100f;

            RaycastHit hit;
            bool isHit = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, maxDistance);

            Gizmos.color = Color.red;
            if (isHit)
            {
                Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
            }
            else
            {
                Gizmos.DrawRay(transform.position, transform.forward * maxDistance);
            }
        }
        #endregion
    }
}
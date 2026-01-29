using UnityEngine;

namespace BRASS
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float lifeTime = 3f;

        void Start()
        {
            // 일정 시간 뒤에 총알 삭제
            Destroy(gameObject, lifeTime);
        }

        void Update()
        {
            // 총알을 앞으로 전진시킴
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 부딪힌 대상이 대미지를 입을 수 있다면
            IDamageable target = other.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
            }

            // 무엇인가에 부딪히면 총알 삭제 (이펙트 추가 가능)
            Destroy(gameObject);
        }
    }
}
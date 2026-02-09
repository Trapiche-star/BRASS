using BRASS;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    #region Variables

    private float damage;           // 외부에서 주입받은 데미지
    private Vector3 moveDirection;  // 이동 방향
    private float moveSpeed;        // 이동 속도

    [SerializeField] private float lifeTime = 3f;

    #endregion

    #region Unity Event Method

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        if (moveDirection == Vector3.zero) return;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    #endregion

    #region Custom Method

    public void Init(Vector3 direction, float speed, float damage)
    {
        moveDirection = direction.normalized;
        moveSpeed = speed;
        this.damage = damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable target = other.GetComponent<IDamageable>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
    #endregion
}

using BRASS;
using Unity.VisualScripting;
using UnityEngine;

public class Missile : MonoBehaviour, IDamageable
{
    [SerializeField]
    private float maxHp = 10f;

    private float currentHp;
    public float CurrentHp
    {
        get { return currentHp; }
        private set
        {
            currentHp = Mathf.Clamp(value, 0, maxHp);
            if (currentHp <= 0f && !isDestroy)
            {
                Destroy();
            }
        }
    }
    private bool isDestroy = false;

    Transform target;
    public float attackDamage = 5.0f;
    public float speed = 25.0f;
    public float rotationSpeed = 5.0f; // 회전 속도 조절 변수

    void Start()
    {
        var player = FindAnyObjectByType<PlayerController>();
        if (player != null) target = player.transform;
    }

    void Update()
    {
        if (target == null) return;

        Vector3 targetDir = (target.position - transform.position).normalized;

        if (targetDir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(targetDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }

        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);
            }

        }
        Destroy(gameObject);
    }

    public void TakeDamage(float damageAmount)
    {
        CurrentHp -= damageAmount;
    }
    void Destroy()
    {
        Destroy(gameObject);
        isDestroy = true;
    }
}
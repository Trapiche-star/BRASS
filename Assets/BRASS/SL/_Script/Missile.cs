using BRASS;
using UnityEngine;
using UnityEngine.UIElements;

public class Missile : MonoBehaviour
{
    Transform target;
    Vector3 targetDir;
    public float attackDamage = 5.0f;

    void Start()
    {
        target = FindAnyObjectByType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
       targetDir = (target.position - transform.position).normalized;
       transform.Translate(targetDir * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.CompareTag("Player");
        other.GetComponent<IDamageable>().TakeDamage(attackDamage);
        Destroy(this);
    }
}

using BRASS;
using UnityEngine;

public class RobotBossHealth : MonoBehaviour, IDamageable
{
    private float hp = 1000.0f;
    public float Hp
    {
        get
        {
            return hp;
        }
        set
        {
            if (hp > 0f && hp <= 1000f)
            {
                hp = value;
                isDaed = false;
            }
            else if (hp <= 0f)
            {
                hp = 0f;
                isDaed = true;
            }
        }
    }
    
    private bool isDaed = false;
    public void TakeDamage(float damageAmount)
    {
        Hp -= damageAmount;
    }

}

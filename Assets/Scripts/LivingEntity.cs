using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour,IDamageable
{
    public float heathStart;
    protected float heath;
    protected bool dead;

    public event System.Action OnDeath;
    protected virtual void Start()
    {
        heath = heathStart;
    }
    public virtual void TakeHit(float damage,Vector3 hitPoint,Vector3 hitDirection)
    {
        TakeDamage(damage);
    }
    public void TakeDamage(float damage)
    {
        heath -= damage;
        if (heath <= 0 && !dead)
        {
            Die();
        }
    }

    [ContextMenu("Self Destory")]
    protected void Die()
    {
        dead = true;
        if (OnDeath!=null)
        {
            OnDeath();
        }
        Destroy(gameObject);
    }
}

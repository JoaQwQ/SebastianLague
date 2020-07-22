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
    public void TakeHit(float damage,RaycastHit hit)
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

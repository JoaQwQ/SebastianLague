using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    /// <summary>
    /// 枪口
    /// </summary>
    public Transform muzzle;
    /// <summary>
    /// 子弹
    /// </summary>
    public Projectile projectile;
    /// <summary>
    /// 限制枪口发射频率
    /// </summary>
    public float msBetweenShots;
    /// <summary>
    /// 发射速度
    /// </summary>
    public float muzzleVelocity;
    /// <summary>
    /// 下次发射时间
    /// </summary>
    float nextShotTime;
    public void Shoot()
    {
        if (Time.time>nextShotTime)
        {
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProject = Instantiate(projectile,muzzle.position,muzzle.rotation) as Projectile;
            newProject.SetSpeed(muzzleVelocity);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FireMode { Auto, Burst, Single };
    public FireMode fireMode;
    /// <summary>
    /// 枪口
    /// </summary>
    public Transform[] projectilesSpawn;
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
    public int burstCount;
    public int projectilesPerMag;
    public float reloadTime = 3f;

    [Header("枪械后坐力")]
    public Vector2 kickMinMax=new Vector2(.08f,.15f);
    public Vector2 recoilAngleMinMax=new Vector2(3,5);
    public float recoilMoveSettingTime=0.1f;
    public float recoilRotationSettingTime=0.1f;

    [Header("特效")]
    public Transform shell;
    public Transform shellPoint;
    MuzzleFlash muzzleFlash;

    bool triggerReleasedSinceLastShot;
    int shotsRemainingInBurst;
    int projectilesRemainingInMag;
    bool isReloading;

    Vector3 recoilSmoothDampVelocity;
    float recoilRotSmoothDampVelocity;
    float recoilAngle;

    private void Start()
    {
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = burstCount;
        projectilesRemainingInMag = projectilesPerMag;
    }

    private void LateUpdate()
    {
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero,ref recoilSmoothDampVelocity, 0.1f);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotSmoothDampVelocity, recoilRotationSettingTime);
        transform.localEulerAngles = transform.localEulerAngles+ Vector3.left * recoilAngle;

        if (!isReloading && projectilesRemainingInMag == 0)
        {
            Reload();
        }
        Debug.Log(projectilesRemainingInMag);
    }

    public void Shoot()
    {
        if (!isReloading&& Time.time > nextShotTime&&projectilesRemainingInMag>0)
        {
            if (fireMode == FireMode.Burst)
            {
                if (shotsRemainingInBurst == 0)
                {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.Single)
            {
                if (!triggerReleasedSinceLastShot)
                {
                    return;
                }
            }

            for (int i = 0; i < projectilesSpawn.Length; i++)
            {
                if (projectilesRemainingInMag==0)
                {
                    break;
                }
                projectilesRemainingInMag--;
                nextShotTime = Time.time + msBetweenShots / 1000;
                Projectile newProject = Instantiate(projectile, projectilesSpawn[i].position, projectilesSpawn[i].rotation) as Projectile;
                newProject.SetSpeed(muzzleVelocity);
            }

            Instantiate(shell, shellPoint.position, shellPoint.rotation);
            muzzleFlash.Activate();
            transform.localPosition -= Vector3.forward * Random.Range(kickMinMax.x,kickMinMax.y);
            recoilAngle += Random.Range(recoilAngleMinMax.x, recoilAngleMinMax.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
        }
    }

    public void Reload()
    {
        if (!isReloading && projectilesRemainingInMag != projectilesPerMag)
        {
            StartCoroutine(AnimateReload());
        }
    }

    IEnumerator AnimateReload()
    {
        isReloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float percent = 0;
        Vector3 initialRot = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (percent < 1)
        {
            percent += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRot + Vector3.left * reloadAngle;

            yield return null;
        }


        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void Aim(Vector3 aimPoint)
    {
        if (!isReloading)
        {
            transform.LookAt(aimPoint);
        }
    }
    public void OnTriggerHold()
    {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerRelease()
    {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = burstCount;
    }
}

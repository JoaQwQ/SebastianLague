using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle,Chasing,Attacking}
    State currentState;
    public ParticleSystem enemyDeathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    Material skinMaterial;
    Color orginalColor;

    /// <summary>
    /// 路径更新刷新率
    /// </summary>
    public float refreshRate=0.25f;

    float attackDistanceThreshould=.5f;
    float timeBetweenAttacks = 1;
    float damage = 1;
    float myCollisionRadius;
    float targetCollisionRadius;
    float nextAttackTime;

    bool hasTarget;

    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            hasTarget = true;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        }
    }
    protected override void Start()
    {
        base.Start();

        if (hasTarget)
        {
            currentState = State.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdataPath());
        }
    }

    public void SetCharacteristics(float moveSpeed,int hitToKillPlayer,float enemyHeath,Color skinColour)
    {
        pathfinder.speed = moveSpeed;
        if (hasTarget)
        {
            //取距离最近的整数
            damage = Mathf.Ceil(targetEntity.heathStart / hitToKillPlayer);
        }
        heathStart = enemyHeath;

        skinMaterial = GetComponent<Renderer>().material;
        skinMaterial.color = skinColour;
        orginalColor = skinMaterial.color;
    }

    public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage>=heath)
        {
            Destroy(Instantiate(enemyDeathEffect.gameObject,hitPoint,Quaternion.FromToRotation(Vector3.forward,hitDirection))as GameObject, enemyDeathEffect.main.startLifetimeMultiplier);
        }
        base.TakeHit(damage, hitPoint, hitDirection);
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                //没有使用Vevtor3.Distance方法，因为需要开平根
                float sqrDstToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDstToTarget < Mathf.Pow(attackDistanceThreshould + myCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }
    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;
        Vector3 originalPositon = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPositon = target.position - dirToTarget * (myCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;
        skinMaterial.color = Color.red;
        bool hasAppliedDamage=false;

        while (percent<=1)
        {
            if (percent>=0.5f&&!hasAppliedDamage)
            {
                hasAppliedDamage = true;
                targetEntity.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            //
            float interpolation = ((-Mathf.Pow(percent, 2) + percent) * 4);
            //
            transform.position = Vector3.Lerp(originalPositon,attackPositon,interpolation);

            yield return null;
        }

        currentState = State.Chasing;
        pathfinder.enabled = true;
        skinMaterial.color = orginalColor;
    }
    IEnumerator UpdataPath()
    {
        while (target!=null)
        {
            if (currentState==State.Chasing)
            {
                //当时想贪吃蛇的一个问题，坐标
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPositon = target.position-dirToTarget*(myCollisionRadius+targetCollisionRadius+ attackDistanceThreshould/2);
                if (!dead)
                {
                    pathfinder.SetDestination(targetPositon);
                }
            }

            yield return new WaitForSeconds(refreshRate);
        }
    }
}

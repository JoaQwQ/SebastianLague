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
    protected override void Start()
    {
        base.Start();
        pathfinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        orginalColor = skinMaterial.color;
        if (GameObject.FindGameObjectWithTag("Player")!=null)
        {
            currentState = State.Chasing;

            target = GameObject.FindGameObjectWithTag("Player").transform;
            hasTarget = true;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            StartCoroutine(UpdataPath());
        }
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

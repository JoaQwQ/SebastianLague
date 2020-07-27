using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collosionMask;
    public float damage=1;
    float speed;
    float skinWidth = 0.15f;

    float lifetime=3;
    private void Start()
    {
        Destroy(gameObject, lifetime);
        Collider[] initialCollision = Physics.OverlapSphere(transform.position,0.15f,collosionMask);
        if (initialCollision.Length>0)
        {
            OnHitObj(initialCollision[0],transform.position);
        }
    }

    public void SetSpeed(float _speed)
    {
        speed = _speed;
    }
    private void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward* moveDistance);
    }
    void CheckCollisions(float movedistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,movedistance+skinWidth, collosionMask))
        {
            OnHitObj(hit.collider,hit.point);
        }
    }
    //void OnHitObj(RaycastHit hit)
    //{
    //    //Debug.Log(hit.collider.gameObject.name);
    //    IDamageable damageableObj = hit.collider.gameObject.GetComponent<IDamageable>();
    //    if (damageableObj!=null)
    //    {
    //        damageableObj.TakeHit(damage,hit);
    //    }
    //    Destroy(gameObject);
    //}
    void OnHitObj(Collider collider,Vector3 hitPoint)
    {
        //Debug.Log(hit.collider.gameObject.name);
        IDamageable damageableObj = collider.GetComponent<IDamageable>();
        if (damageableObj != null)
        {
            damageableObj.TakeHit(damage,hitPoint,transform.forward);
        }
        Destroy(gameObject);
    }
}

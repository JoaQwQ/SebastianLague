using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Vector3 velocity;
    Rigidbody rigi;
    void Start()
    {
        rigi = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rigi.MovePosition(rigi.position + velocity * Time.fixedDeltaTime);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_velocity"></param>
    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void LookAt(Vector3 _point)
    {
        Vector3 modyPoint = new Vector3(_point.x, transform.position.y, _point.z);
        transform.LookAt(modyPoint);
    }
}

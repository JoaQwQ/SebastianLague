using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public float moveSpeed=5;
    public new Camera camera;
    public Crosshairs crosshairs;
    PlayerController playerController;
    GunController gunController;

    protected override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
    }
    void Update()
    {
        //移动
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelpcity = moveInput.normalized * moveSpeed;
        playerController.Move(moveVelpcity);

        //转向
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.up*gunController.gunHeight);
        float rayDistance;
        if (plane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            playerController.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            if((new Vector2(point.x,point.z)-new Vector2(transform.position.x, transform.position.z)).sqrMagnitude>0.5f)
            {
                gunController.Aim(point);
            }
        }
        //射击
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : MonoBehaviour
{
    public float moveSpeed=5;
    public Camera camera;
    PlayerController playerController;
    GunController gunController;

    private void Start()
    {
        playerController = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        camera = Camera.main;
    }
    void Update()
    {
        //移动
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelpcity = moveInput.normalized * moveSpeed;
        playerController.Move(moveVelpcity);

        //转向
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;
        if (plane.Raycast(ray,out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            //Debug.DrawLine(ray.origin, point, Color.red);
            playerController.LookAt(point);
        }
        //射击
        if (Input.GetMouseButtonDown(0))
        {
            gunController.Shoot();
        }
    }
}

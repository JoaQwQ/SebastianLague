using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Gun startGun;
    public Transform gunHolder;
    Gun equipedGun;
    private void Start()
    {
        if (startGun!=null)
        {
            EquipGun(startGun);
        }
    }
    public void EquipGun(Gun gunToEquip) 
    {
        if (equipedGun != null)
        {
            Destroy(equipedGun.gameObject);
        }
        equipedGun = Instantiate(gunToEquip, gunHolder.transform.position, gunHolder.transform.rotation)as Gun;
        equipedGun.transform.SetParent(gunHolder);
    }

    public void OnTriggerHold() 
    {
        if (equipedGun!=null)
        {
            equipedGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease()
    {
        if (equipedGun != null)
        {
            equipedGun.OnTriggerRelease();
        }
    }

    public float gunHeight
    {
        get { return gunHolder.position.y; }
    }

    public void Aim(Vector3 aimPoint)
    {
        if (equipedGun != null)
        {
            equipedGun.Aim(aimPoint);
        }
    }

    public void Reload()
    {
        if (equipedGun != null)
        {
            equipedGun.Reload();
        }
    }
}

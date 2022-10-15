using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MegaPhone : MonoBehaviour
{
    public int battery
    {
        get { return inventory.batteries; }
        set { inventory.batteries = value; }
    }

    public float shootCooldown = 10f;

    private bool canShoot = true;

    private bool hasBattery => battery > 0;

    private Inventory inventory;

    private void Start()
    {
        inventory = this.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (inventory.currGunId != 0)
        {
            return;
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!canShoot || !hasBattery)
        {
            return;
        }

        canShoot = false;

        battery--;
        //play audio
        //add particles of shockwave
        //attack all enemies in the area

        canShoot = true;
    }
}

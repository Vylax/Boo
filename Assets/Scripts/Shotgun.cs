using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    public int ammo
    {
        get { return inventory.ammo; }
        set { inventory.ammo = value; }
    }
    public int loadedAmmo;

    public float reloadTime = 1f;
    public float justShotTime = .1f;
    public float recoilMoveCooldown = .4f;

    public bool canReload = true;
    private bool canShoot = true;
    public bool canMove = true;
    public bool justShot = false;
    public bool GunFull => loadedAmmo == 2;
    public bool GunEmpty => loadedAmmo == 0;

    public float forceMagnitude = 1f;

    public LayerMask objectsMask;
    private Rigidbody rigidbody;

    private Inventory inventory;

    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        inventory = this.GetComponent<Inventory>();
    }

    public void Reload()
    {
        if(!canReload || GunFull || ammo == 0) //returns if we can't reload the gun (already reloading/gun is full/we don't have extra ammo
        {
            return;
        }

        canReload = false;

        //play anim here

        while (!GunFull && ammo > 0) //while the gun isn't full and we still have extra ammo, put ammo in the gun
        {
            ammo--;
            loadedAmmo++;
        }

        StartCoroutine(ReloadCoroutine());
    }

    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        canReload = true;
    }

    private IEnumerator CanMoveCooldown()
    {
        yield return new WaitForSeconds(recoilMoveCooldown);
        canMove = true;
    }

    private IEnumerator ShootCoroutine()
    {
        yield return new WaitForSeconds(justShotTime);
        justShot = false;
    }

    public void ForceCanMove()
    {
        StopCoroutine(CanMoveCooldown());
        canMove = true;
    }

    public void Shoot()
    {
        if(!canShoot || GunEmpty || !canReload) //return if we can't shoot (already shooting/no loaded ammo/reloading
        {
            return;
        }

        canShoot = false;
        canMove = false;
        justShot = true;

        loadedAmmo--;

        //play anim here

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, objectsMask, QueryTriggerInteraction.Ignore)) //get hit informations
        {
            //Debug
            Debug.Log($"hit name: {hit.transform.name}");
            Vector3 hitPoint = hit.point;
            Vector3 hitReactionDir = hit.point + (Camera.main.transform.position - hit.point).normalized;

            //instantiate particles here
        }

        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.AddForce(-forceMagnitude * Camera.main.transform.forward, ForceMode.Impulse); //add rocket jumping force

        StartCoroutine(ShootCoroutine());
        canShoot = true;
        StartCoroutine(CanMoveCooldown());
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
    }
}

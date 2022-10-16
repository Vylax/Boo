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
    Animation anim;

    public GameObject shootParticles;

    private void Start()
    {
        rigidbody = this.GetComponent<Rigidbody>();
        inventory = this.GetComponent<Inventory>();
        anim = inventory.weapons[0].GetComponent<Animation>();
    }

    public void Reload()
    {
        if(!canReload || GunFull || ammo == 0) //returns if we can't reload the gun (already reloading/gun is full/we don't have extra ammo
        {
            return;
        }

        canReload = false;

        //play anim here
        anim.Stop();
        anim.Rewind();
        anim.Play("Armature|Reloading");
        Player.Instance.PlayAudio($"crick crack 0{Random.Range(1, 4)}");

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
        anim.Rewind();
        anim.Play("Armature|Shooting2");
        Player.Instance.PlayAudio($"tir {Random.Range(1, 5)}", inventory.GunExit.position);

        GameObject temp = Instantiate(shootParticles, inventory.GunExit.position, inventory.GunExit.rotation * Quaternion.Euler(-89.98f, 0, 11.744f));
        Destroy(temp, temp.GetComponent<ParticleSystem>().duration);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, objectsMask, QueryTriggerInteraction.Ignore)) //get hit informations
        {
            //Debug
            Debug.Log($"hit name: {hit.transform.name}");
            Vector3 hitPoint = hit.point;
            Vector3 hitReactionDir = hit.point + (Camera.main.transform.position - hit.point).normalized;

            //instantiate hit particles here
            
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
        if(inventory.currGunId != 0)
        {
            return;
        }

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

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
    public float shockWaveRadius = 10f;
    public float shockWaveDelay = 5f;
    public LayerMask enemyMask;

    private bool canShoot = true;

    private bool hasBattery => battery > 0;

    private Inventory inventory;

    private void Start()
    {
        inventory = this.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (inventory.currGunId != 1)
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
        inventory.canSwitch = false;

        battery--;

        StartCoroutine(Shooting());
    }

    private IEnumerator Shooting()
    {
        //start playing audio
        Player.Instance.PlayAudio($"boo megaphone v2", 0, .7f);

        yield return new WaitForSeconds(shockWaveDelay);
        //add particles of shockwave

        //attack all enemies in the area
        Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, shockWaveRadius, enemyMask, QueryTriggerInteraction.Collide);
        Debug.Log($"hhh{hitColliders.Length}");
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponentInParent<Ghost>() != null)
            {
                hitCollider.GetComponentInParent<Ghost>().Die();
            }
        }

        canShoot = true;
        inventory.canSwitch = true;
    }
}

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

    public GameObject ShockWavePrefab;
    public int shockWaveCount = 10;
    public float shockWaveBurstTime = 0.3f;
    public float shockWaveLifeTime = 7f;

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
        StartCoroutine(SpawnShockWave());

        //attack all enemies in the area
        //done by the shockwave
        /*Collider[] hitColliders = Physics.OverlapSphere(Player.Instance.transform.position, shockWaveRadius, enemyMask, QueryTriggerInteraction.Collide);
        Debug.Log($"hhh{hitColliders.Length}");
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.GetComponentInParent<Ghost>() != null)
            {
                hitCollider.GetComponentInParent<Ghost>().Die();
            }
        }*/

        canShoot = true;
        inventory.canSwitch = true;
    }

    private IEnumerator SpawnShockWave()
    {
        for (int i = 0; i < shockWaveCount; i++)
        {
            GameObject temp = Instantiate(ShockWavePrefab, Camera.main.transform.position, Quaternion.identity);
            temp.transform.forward = Camera.main.transform.forward;
            Destroy(temp, shockWaveLifeTime);
            yield return new WaitForSeconds(shockWaveBurstTime);
        }
    }
}

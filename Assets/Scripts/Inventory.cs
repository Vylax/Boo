using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int ammo = 10;
    public int batteries = 2;

    public int currGunId = 0;

    private bool canReload => this.GetComponent<Shotgun>().canReload;
    private bool GunEmpty => this.GetComponent<Shotgun>().GunEmpty;
    private bool GunFull => this.GetComponent<Shotgun>().GunFull;

    public GameObject[] weapons = new GameObject[2];

    public LayerMask itemMask;
    public LayerMask worldMask;

    public float itemPickUpRange = 3f;
    public float itemPickUpOffset = .3f;

    //debug
    private Vector3 pickupPoint;

    private void Start()
    {
        this.GetComponent<Shotgun>().enabled = false;
        this.GetComponent<MegaPhone>().enabled = false;

        SwitchGun(0);
    }

    private void Update()
    {
        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            SwitchGun(currGunId + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            SwitchGun(currGunId - 1);
        }

        if (Input.GetKeyDown(KeyCode.E)) //try to pickup item
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, itemPickUpRange, worldMask, QueryTriggerInteraction.Collide)) //get hit informations
            {
                //Debug
                Debug.Log($"hit name: {hit.transform.name}");
                pickupPoint = hit.point;
                Vector3 hitReactionDir = hit.point + (Camera.main.transform.position - hit.point).normalized;

                Collider[] items = Physics.OverlapSphere(hit.point, itemPickUpOffset, itemMask, QueryTriggerInteraction.Collide); //detect all items within a certain radius of hit point (to pick up item more easily)

                if (items.Length > 0)
                {
                    float minDist = Mathf.Infinity;
                    Collider item = null;

                    for (int i = 0; i < items.Length; i++)
                    {
                        float itemDist = Vector3.Distance(items[i].transform.position, hit.point);
                        if (itemDist < minDist)
                        {
                            minDist = itemDist;
                            item = items[i];
                        }
                    }

                    PickUp(item.GetComponent<Item>());
                }
            }
        }
    }

    private void OnGUI()
    {
        if (currGunId == 0)
        {
            GUI.Box(new Rect(Screen.width - 10 - 100, Screen.height - 10 - 50, 100, 50), $"Ammo: {GetAmmoSymbol()} | {ammo}\n{(canReload ? "" : "Reloading...")}");
        }
    }

    public void PickUp(Item item)
    {
        //Add particles


        if(item.itemId == 0)
        {
            ammo++;
        }
        else if(item.itemId == 1)
        {
            batteries++;
        }

        Destroy(item.gameObject);
    }

    public void SwitchGun(int gunId)
    {
        weapons[currGunId].SetActive(false);

        if (gunId > 1)
        {
            currGunId = 0;
        }
        else if(gunId < 0)
        {
            currGunId = 1;
        }
        else
        {
            currGunId = gunId;
        }
        this.GetComponent<Shotgun>().enabled = currGunId == 0 ? true : false;
        this.GetComponent<MegaPhone>().enabled = currGunId == 1 ? true : false;
        weapons[currGunId].SetActive(true);
    }

    private string GetAmmoSymbol()
    {
        if (GunEmpty)
        {
            return "OO";
        }
        else if (GunFull)
        {
            return "XX";
        }
        else
        {
            return "OX";
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(pickupPoint, .2f);
    }
}

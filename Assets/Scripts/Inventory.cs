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
    }

    private void OnGUI()
    {
        if (currGunId == 0)
        {
            GUI.Box(new Rect(Screen.width - 10 - 100, Screen.height - 10 - 50, 100, 50), $"Ammo: {GetAmmoSymbol()} | {ammo}\n{(canReload ? "" : "Reloading...")}");
        }
    }

    public void PickUp(int itemId)
    {
        if(itemId == 0)
        {
            ammo++;
        }
        else if(itemId == 1)
        {
            batteries++;
        }
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
}

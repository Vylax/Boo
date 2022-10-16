using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.GraphicsBuffer;

public class Compass : MonoBehaviour
{
    public Vector3 dir;
    public float angle;
    public GameObject arrow;
    public GameObject target;
    public int targetType;

    private void Start()
    {
        InvokeRepeating("RefreshTarget", 0.5f, 5f);
    }

    void RefreshTarget()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Item");
        float minDist = Mathf.Infinity;
        GameObject tempTarget = null;
        for (int i = 0; i < temp.Length; i++)
        {
            float dist = Vector3.Distance(temp[i].transform.position, transform.position);
            if (temp[i].GetComponent<Item>().itemId == targetType && dist < minDist)
            {
                minDist = dist;
                tempTarget = temp[i];
            }
        }

        target = tempTarget;
    }

    private void Update()
    {
        if(target == null)
        {
            return;
        }

        Vector3 temp = Camera.main.transform.forward;
        temp.y = 0;

        Vector3 temp2 = target.transform.position - transform.position;
        temp2.y = 0;

        angle = Vector3.SignedAngle(temp, temp2, Vector3.up);
        arrow.transform.localEulerAngles = new Vector3(0, angle, 0);
    }
}

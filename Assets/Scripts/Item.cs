using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemId;

    public void Initialize(int itemId = 0)
    {
        this.itemId = itemId;
        this.GetComponent<Renderer>().material = Resources.Load<Material>($"Item{itemId}");
        Color color = this.GetComponent<Renderer>().material.color;
        GetComponentInChildren<Light>().color = color;
        GetComponentInChildren<ParticleSystemRenderer>().material = Resources.Load<Material>($"Item{itemId}");
    }
}

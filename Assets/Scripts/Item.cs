using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int itemId;

    private void Awake()
    {
        this.GetComponent<Renderer>().material = Resources.Load<Material>($"Item{itemId}");
    }
}

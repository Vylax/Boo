using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    public float speed = .3f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Time.timeScale = Time.timeScale == 1f ? speed : 1f;
        }
    }
}

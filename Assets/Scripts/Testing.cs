using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Time.timeScale = Time.timeScale == 1f ? 0.1f : 1f;
        }
    }
}

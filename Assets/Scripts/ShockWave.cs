using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    public float scaleSpeed = 1f;
    public float moveSpeed = 1f;

    public float spawnTime = Mathf.Infinity;

    public enum scaleMode
    {
        exponential,
        linear
    }

    public scaleMode mode;

    private bool moving = false;
    Vector3 startPos;

    private void Update()
    {
        if (!moving)
        {
            moving = true;
            startPos = transform.position;
            spawnTime = Time.time;
        }

        if(transform.localScale.x > 600f && GetComponent<MeshCollider>().enabled)
        {
            GetComponent<MeshCollider>().enabled = false;
        }

        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        if(mode == scaleMode.exponential)
        {
            transform.localScale *= (1f + scaleSpeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.Distance(startPos, transform.position) * scaleSpeed * Vector3.one;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wtf : MonoBehaviour
{
    public float forceMagnitude = 1f;
    public float moveSpeed = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    void Move()
    {
        Vector3 targetVelocity = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;

        targetVelocity = transform.TransformDirection(targetVelocity) * moveSpeed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rb.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.y = 0;

        Debug.Log(velocityChange.magnitude);

        if (true)
        {
            rb.AddForce(velocityChange, ForceMode.VelocityChange);
        }
    }

    void Pulse()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(-forceMagnitude * Camera.main.transform.forward, ForceMode.Impulse); //add rocket jumping force
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        movement.x = Input.GetAxis("Horizontal");
        movement.z = Input.GetAxis("Vertical");

        if(Input.GetKey(KeyCode.E))
        {
            movement.y = 1f;
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            movement.y = -1f;
        }

        movement *= Time.deltaTime*2f;

        rb.MovePosition(transform.position + movement);
    }
}

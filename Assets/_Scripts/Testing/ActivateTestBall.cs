using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateTestBall : MonoBehaviour
{

    public Rigidbody ballRB;

    void Start()
    {
        ballRB.isKinematic = true;
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ballRB.isKinematic = false;

            GetComponent<PlaneGenerator>().UpdateCollider();
        }
    }
}

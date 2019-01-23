using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField]
    PlaneGenerator planeGenerator;

    [SerializeField]
    DeformTool tool;

    [SerializeField]
    GameObject ball;

    [SerializeField]
    Transform spawnPoint;

    GameObject ballInstance;

    // Use this for initialization
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.JoystickButton16) || Input.GetKeyDown(KeyCode.S))
        {
            SpawnBall();
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton8))
        {
            ResetGame();
        }
    }

    private void ResetGame()
    {
        if(ballInstance != null)
        {
            Destroy(ballInstance);
            tool.Activate();
        }

        planeGenerator.ResetMesh();
    }

    private void SpawnBall()
    {
        if (ballInstance == null)
        {
            ballInstance = Instantiate(ball, spawnPoint.position, Quaternion.identity);

            tool.Deactivate();
        }
        else
        {
            Destroy(ballInstance);
            tool.Activate();
        }

    }
}

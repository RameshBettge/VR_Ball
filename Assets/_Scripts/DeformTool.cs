using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Can deform objects on layer 'deformable'. Make sure to only move it via it's rigidbody.
/// </summary>
[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class DeformTool : MonoBehaviour
{
    [SerializeField]
    Material heightenMat;

    [SerializeField]
    Material indentMat;

    [SerializeField]
    Material deactivatedMat;

    [SerializeField]
    float heightChange = 2f;

    [SerializeField]
    float range = 6f;

    [SerializeField]
    AnimationCurve deformCurve;

    [HideInInspector]
    public bool deactivated;

    int deformDirection = 1;

    Rigidbody rb;

    List<PlaneGenerator> planeGenerators;

    SphereCollider sphereCol;

    Renderer rend;

    float debugTick;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sphereCol = GetComponent<SphereCollider>();
        rend = GetComponent<Renderer>();

        planeGenerators = new List<PlaneGenerator>();

        rb.isKinematic = true;
        sphereCol.isTrigger = true;

    }

    void Update()
    {
        if (deactivated) { return; }

        // TODO: remove everything in update. Deforming should only be called from a hand script

        if(Input.GetKeyDown(KeyCode.JoystickButton17))
        {
            bool upwards = (deformDirection > 0) ? false : true;
            SetDeformDirection(upwards);
        }

        if (Input.GetKeyUp(KeyCode.JoystickButton15) || ((Time.time - debugTick) > 0.5f))
        {
            debugTick = Time.time;

            for (int i = 0; i < planeGenerators.Count; i++)
            {
                planeGenerators[i].UpdateCollider();
            }
        }


        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.JoystickButton15))
        {
            for (int i = 0; i < planeGenerators.Count; i++)
            {
                Deform(planeGenerators[i]);
                planeGenerators[i].UpdateMesh();
            }
        }
    }

    public void SetDeformDirection(bool upwards)
    {
        if (deactivated) { return; }

        deformDirection = upwards ? 1 : -1;
        Material nextMat = upwards ? heightenMat : indentMat;

        rend.material = nextMat;
    }

    public void Activate()
    {
        Debug.Log("Activating");
        deactivated = false;
        SetDeformDirection(true);
    }

    public void Deactivate()
    {
        rend.material = deactivatedMat;
        deactivated = true;
    }

    void Deform(PlaneGenerator generator)
    {
        float xHit = transform.position.x - generator.transform.position.x;
        float zHit = transform.position.z - generator.transform.position.z;

        xHit /= generator.Scale;
        zHit /= generator.Scale;

        int xPos = Mathf.RoundToInt(xHit);
        int zPos = Mathf.RoundToInt(zHit);


        float scaledRange = range / generator.Scale;

        for (int x = (int)(xPos - scaledRange); x < (xPos + scaledRange); x++)
        {
            if (x >= generator.resolution) { continue; }
            else if (x < 0) { continue; }

            for (int z = (int)(zPos - scaledRange); z < (zPos + scaledRange); z++)
            {
                if (z >= generator.resolution)
                {
                    continue;
                }
                if (z < 0 || z >= generator.resolution)
                {
                    continue;
                }

                float xDist = xHit - x;
                float zDist = zHit - z;

                float distance = Mathf.Sqrt(xDist * xDist + zDist * zDist);

                if (distance >= scaledRange) { continue; }

                float percDistance = distance / scaledRange;

                float adjustedDistance = deformCurve.Evaluate(percDistance);

                float deformHeight = heightChange * adjustedDistance * deformDirection;

                float triggerInput = Input.GetAxis("Vive_Trigger_R");
                deformHeight *= triggerInput;

                //int scaledX = Mathf.RoundToInt(x / generator.Scale);
                //int scaledZ = Mathf.RoundToInt(z / generator.Scale);
                //generator.ChangeVertexHeight(scaledX, scaledZ, deformHeight);
                generator.ChangeVertexHeight(x, z, deformHeight);
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        PlaneGenerator planeGenerator = other.GetComponent<PlaneGenerator>();

        if (planeGenerator == null)
        {
            Debug.LogError("Deform tool was triggered by something that isn't deformable.");
            return;
        }

        planeGenerators.Add(planeGenerator);
    }

    private void OnTriggerExit(Collider other)
    {
        // TODO: Consider only removing something after stopping to draw.

        for (int i = 0; i < planeGenerators.Count; i++)
        {
            if (other.gameObject == planeGenerators[i].gameObject)
            {
                planeGenerators.Remove(planeGenerators[i]);
            }
        }
    }
}

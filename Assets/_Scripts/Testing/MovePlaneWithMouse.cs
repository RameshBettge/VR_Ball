using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlaneWithMouse : MonoBehaviour
{
    [SerializeField]
    AnimationCurve deformCurve;

    [SerializeField]
    int range = 3;

    [SerializeField]
    float heightChange = 1f;

    // 1 is up, -1 is down
    int deformDirection = 1;

    //Make sure the mask only hits 'deformable' objects
    [SerializeField]
    LayerMask mask;

    PlaneGenerator generator;

    void Start()
    {
        generator = GetComponent<PlaneGenerator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            deformDirection = 1;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            deformDirection = -1;
        }

        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Mouse1))
        {
            DrawOnPlane();
        }


        if (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1))
        {
            generator.UpdateMesh();
        }
    }

    void DrawOnPlane()
    {
        Vector3 mousePos = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask, QueryTriggerInteraction.Ignore))
        {
            //generator.ChangeVertexHeight(hit.point.x, hit.point.z, heightChange);
            Deform(hit.point.x, hit.point.z);
        }
    }

    // TODO: Make sure deformation works when the plane isn't at the origin
    void Deform(float xHit, float zHit)
    {
        //int xPos = Mathf.RoundToInt(xHit - transform.position.x);
        //int zPos = Mathf.RoundToInt(zHit - transform.position.z);

        xHit -= transform.position.x;
        zHit -= transform.position.z;

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
                if(z >= generator.resolution)
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

                //int scaledX = Mathf.RoundToInt(x / generator.Scale);
                //int scaledZ = Mathf.RoundToInt(z / generator.Scale);
                //generator.ChangeVertexHeight(scaledX, scaledZ, deformHeight);
                generator.ChangeVertexHeight(x, z, deformHeight);
            }
        }

    }
}

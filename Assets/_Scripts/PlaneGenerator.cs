using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class PlaneGenerator : MonoBehaviour
{
    [SerializeField]
    float scale = 0.1f;

    public float Scale { get { return scale; } }

    [SerializeField]
    float startHeight = 10f;

    [SerializeField]
    float maxHeight = 10f;

    private MeshFilter filter;

    MeshCollider col;

    private Mesh mesh;


    public int resolution = 16;
    private Vector3[] vertices;
    private int[] tris;
    Vector2[] uvs;

    [Header("Debugging")]
    public int debugInt;

    public bool displayGizmos;


    void Start()
    {
        if(maxHeight < startHeight)
        {
            Debug.LogError("maxHeight is smaller than startHeight.");
        }

        filter = GetComponent<MeshFilter>();
        col = GetComponent<MeshCollider>();

        CreateVertices();
        CreateTris();
        GenerateMesh();
        UpdateCollider();

        //Testing
        //SetVertexHeight(2, 2, 2f);
        //UpdateMesh();
    }

    private void GenerateMesh()
    {
        mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;

        filter.mesh = mesh;

        // TODO: Update mesh collider here
    }

    private void CreateTris()
    {
        int numOfFaces = (resolution - 1) * (resolution - 1);
        int numOfTris = 2 * numOfFaces;
        int row = resolution;
        tris = new int[numOfTris * 3];

        int t = 0;// Index for tris
        for (int z = 0; z < (row - 1); z++)
        {
            for (int x = 0; x < (row-1); x++, t += 6)
            {
                int i = (row * z) + x;

                tris[t] = i;
                tris[t + 1] = i + row;
                tris[t + 2] = i + 1;

                tris[t + 3] = i + 1;
                tris[t + 4] = i + row;
                tris[t + 5] = i + row + 1;
            }
        }

    }

    public void ResetMesh()
    {
        CreateVertices();
        CreateTris();
        GenerateMesh();
        UpdateCollider();
    }

    private void CreateVertices()
    {
        vertices = new Vector3[resolution * resolution];
        uvs = new Vector2[vertices.Length];

        // vertices are generated in rows in x direction
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int ind = (z * resolution) + x;
                vertices[ind] = new Vector3(x * scale, startHeight, z * scale);
                uvs[ind] = new Vector2(x * scale, z * scale);
            }
        }
    }

    /// <summary>
    /// Depends on hitX and hitZ to be in world position.
    /// Call UpdateMesh() to make changes visible.
    /// </summary>
    public void ChangeVertexHeight(float hitX, float hitZ, float heightChange)
    {
        int x = (int)((hitX - transform.position.x) / scale);
        int z = (int)((hitZ - transform.position.z) / scale);

        ChangeVertexHeight(x, z, heightChange);
    }

    /// <summary>
    /// Depends on x and z being in local Space of the generator.
    /// Call UpdateMesh() to make changes visible.
    /// </summary>
    public void ChangeVertexHeight(int x, int z, float heightChange)
    {
        int index = (z * resolution) + x;

        if (index >= vertices.Length)
        {
            //Debug.LogWarning("Cannot set vertex height - index " + index + " is out of range!");
            return;
        }
        if(index <= 0)
        {
            //Debug.LogWarning("Cannot set vertex height - index " + index + " is below 0!");
            return;
        }

        Vector3 pos = vertices[index];

        //Debug.Log("Setting height of vertex nr. " + index + ".  x = " + x + "; z = " + z + " to " +pos.z+heightChange);

        pos.y += (heightChange * Time.deltaTime);

        pos.y = Mathf.Clamp(pos.y, 0f, maxHeight);

        vertices[index] = pos;
    }

    public void UpdateMesh()
    {
        mesh.vertices = vertices;
        filter.mesh = mesh;
        filter.mesh.RecalculateNormals();
    }

    public void UpdateCollider()
    {
        col.sharedMesh = mesh;
    }

    private void OnDrawGizmos()
    {
        if (!displayGizmos) { return; }

        if (vertices != null)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.color = Color.cyan;

                if (i == debugInt)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(vertices[i] * scale + transform.position, 1f * scale);
                }
                else
                {
                    Gizmos.DrawWireSphere(vertices[i] * scale + transform.position, 0.25f * scale);
                }
            }
        }
    }
}
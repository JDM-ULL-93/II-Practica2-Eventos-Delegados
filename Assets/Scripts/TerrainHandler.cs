using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void fOnAfterCreation(TerrainHandler.ReservedSpace[] rspace);
public delegate void fOnBeforeCreation(TerrainHandler handler);

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
public class TerrainHandler : MonoBehaviour
{
    public event fOnAfterCreation eventOnAfterCreation;
    public event fOnBeforeCreation eventOnBeforeCreation;

    private static TerrainHandler instance;
    public static TerrainHandler Instance { get => instance; }
    private TerrainHandler() { }


    //private MeshCollider collider;
    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;

    public int xSize = 20;
    public int zSize = 20;

    public class ReservedSpace
    {
        protected Vector2 origin;
        public Vector2 Origin { get => origin; }
        protected Vector2 end;
        public Vector2 End { get => end; }
        public ReservedSpace(int x1, int z1, int x2, int z2)
        {
            origin = new Vector2(x1, z1);
            end = new Vector2(x2, z2);
        }

    }
    private List<ReservedSpace> reservedSpaces = new List<ReservedSpace>();
    public void addReservedSpace(ReservedSpace space)
    {
        reservedSpaces.Add(space);
    }
    void Awake()
    {
        instance = this;

        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
    }

    // Start is called before the first frame update
    void Start()
    {
      
        eventOnBeforeCreation?.Invoke(instance);
        CreateShape(); //StartCoroutine(CreateShape());
        UpdateMesh();
        eventOnAfterCreation?.Invoke(reservedSpaces.ToArray());

        var collider = this.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        //collider.convex = true;
        //var rb = this.gameObject.AddComponent<Rigidbody>();
        //rb.isKinematic = true;
    }

    private /*IEnumerator*/void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        
        //int vert = 0, tris = 0;
        for (int i = 0, z = 0; z <= zSize; z++)
            for (var x = 0; x <= xSize; x++, i++)
            {
                float y = Mathf.PerlinNoise(x* 0.3f,z*0.3f) * 2f;
                bool reserved = false;
                foreach(ReservedSpace rSpace in reservedSpaces)
                {
                    if ( reserved = (x >= rSpace.Origin.x && x <= rSpace.End.x && z >= rSpace.Origin.y && z <= rSpace.End.y) )
                        break;
                }
                vertices[i] = new Vector3(x, reserved ? 0 : y, z);
            }
                

        triangles = new int[xSize * zSize * 6];
        var vert = 0;
        var tris = 0;
        for(var z = 0; z < zSize; z++,vert++)
            for (var x = 0; x < xSize; x++, vert++, tris += 6)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                //yield return new WaitForSeconds(0.01f);
            }
    }

    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

    }


    /*private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        for(var i = 0; i < vertices.Length; i++)
            Gizmos.DrawSphere(vertices[i], 0.1f);
    }*/

}

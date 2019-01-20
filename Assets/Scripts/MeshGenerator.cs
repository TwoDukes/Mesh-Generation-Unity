using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    [Header("Settings")]
    public int xSize, zSize;

    [Header("Colors")]
    public Gradient gradient;

    [Header("Noise Generators")]
    public bool smoothFlow = false;

    [Header("Noise 1")]
    public float noise01Scale;
    public float noise01Amp;
    public float noise1XOffset;
    public float noise1ZOffset;

    [Header("Noise 2")]
    public float noise02Scale;
    public float noise02Amp;
    public float noise2XOffset;
    public float noise2ZOffset;

    [Header("Noise 3")]
    public float noise03Scale;
    public float noise03Amp;
    public float noise3XOffset;
    public float noise3ZOffset;

    private float minTerrainHeight, maxTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.G))
        //{
            CreateShape();
            UpdateMesh();
        //}
    }

    void CreateShape()
    {
        // Vertices
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        for(int i = 0, z = 0; z <= zSize; z++)
        {
            for(int x = 0; x <= xSize; x++)
            {
                float y = calculatePerlinNoise(x, z);
                vertices[i] = new Vector3(x, y, z);

                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y;

                i++;
            }
        }


        // Triangles
        triangles = new int[xSize * zSize * 6];

        for(int vert = 0, tris = 0, z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        // Colors
        colors = new Color[vertices.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float height = Mathf.InverseLerp(minTerrainHeight,maxTerrainHeight,vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }
        }
    }

    float calculatePerlinNoise(int x, int z)
    {
        if (smoothFlow)
        {
            noise1XOffset += Time.deltaTime / 80000;
            noise2ZOffset += Time.deltaTime / 20000;
            noise3ZOffset += Time.deltaTime / 10000;
        }
        float noise1Perlin = Mathf.PerlinNoise(x * noise01Scale + noise1XOffset, z * noise01Scale + noise1ZOffset) * noise01Amp;
        float noise2Perlin = Mathf.PerlinNoise(x * noise02Scale + noise2XOffset, z * noise02Scale + noise2ZOffset) * noise02Amp;
        float noise3perlin = Mathf.PerlinNoise(x * noise03Scale + noise3XOffset, z * noise03Scale + noise3ZOffset) * noise03Amp;
        return noise1Perlin + noise2Perlin + noise3perlin;

        //float newdetailPerlineMultiplier = noise02Amp * (-noise1Perlin + 0.5f);
        //return noise1Perlin * noise01Amp + noise2Perlin * Mathf.Max(0, newdetailPerlineMultiplier);       
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}

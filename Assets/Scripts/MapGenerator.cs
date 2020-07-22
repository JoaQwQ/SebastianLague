using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Vector2 mapSize;

    [Range(0,1)]
    public float outlinePrecent;
    private void Start()
    {
        GeneratorMap();
    }
    public void GeneratorMap()
    {
        string holderName = "GeneratedMap";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosion = new Vector3(-mapSize.x/2+x+0.5f,0, -mapSize.y / 2 + y + 0.5f);
                Transform newTile = Instantiate(tilePrefab,tilePosion,Quaternion.Euler(Vector3.right*90))as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePrecent);
                newTile.parent = mapHolder;
            }
        }
    }
}

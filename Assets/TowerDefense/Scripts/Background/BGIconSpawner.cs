using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class BGIconSpawner : MonoBehaviour
{
    [SerializeField] protected GameObject iconPrefab;
    [SerializeField] protected Vector2 offset;
    [SerializeField] protected List<Vector2Int> ignoreTiles;
    [SerializeField] protected Vector2Int size;
    [SerializeField] protected Dictionary<Vector2Int, GameObject> grid = new();

    void Start()
    {
        SpawnTiles();
    }

    

    protected virtual void SpawnTiles()
    {
        Vector2Int p;
        for (int x = -size.x / 2; x < size.x / 2; x++)
        {
            for (int y = -size.y / 2; y < size.y / 2; y++)
            {
                Vector3 spawnPos = new Vector3(x + offset.x, y + offset.y, 0);
                p = new Vector2Int(x, y);
                if (!ignoreTiles.Contains(p))
                {
                    var go = Instantiate(iconPrefab, spawnPos, Quaternion.identity, transform);
                    grid.Add(p, go);
                }
            }
        }

        //hex grid
        /*for (int x = -size.x / 2; x < size.x / 2; x++) 
        {
            for (int y = -size.y / 2; y < size.y / 2; y++)
            {
                float offset = y % 2 == 0 ? 0 : (2f/3f*distance);
                Vector3 spawnPos = new Vector3(x * Mathf.Sqrt(3) * (distance * 2 / 3) + offset, y * (3 / 2) * distance, 0);
                p = new Vector2Int(x, y);
                if (!ignoreTiles.Contains(p))
                    Instantiate(iconPrefab, spawnPos, Quaternion.identity);
            }
        }*/
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Tile
{
    public GameObject tile;
    public Vector3Int size;
    public string name;
};

public class Gridmap : MonoBehaviour
{
    // Inspector fields
    public Vector3Int mapSize;
    public Vector3 tileSize;
    public Vector3 tileOffset;
    public Tile[] tileset;

    // Elements
    private readonly Dictionary<Vector3Int, GameObject> map;

    public Dictionary<Vector3Int, GameObject> Map { get { return map; } }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlaceAt(string name, Vector3Int position)
    { }
    void GetAt(Vector3Int position)
    { }
}

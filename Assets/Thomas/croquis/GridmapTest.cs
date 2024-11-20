using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridmapTest : MonoBehaviour
{
    Grid grid;

    public GameObject blenderTileset;

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();

        PlaceTile("ground", new Vector3Int(0, 0, 0));
        PlaceTile("cliff-ground", new Vector3Int(-1, 0, 0), 0);
        PlaceTile("cliff-corner", new Vector3Int(-1, 1, 0), 0);
        PlaceTile("cliff-ground", new Vector3Int(0, 1, 0), 1);
        PlaceTile("cliff-corner", new Vector3Int(1, 1, 0), 1);
        PlaceTile("cliff-ground", new Vector3Int(1, 0, 0), 2);
        PlaceTile("cliff-corner", new Vector3Int(1, -1, 0), 2);
        PlaceTile("cliff-ground", new Vector3Int(0, -1, 0), 3);
        PlaceTile("cliff-corner", new Vector3Int(-1, -1, 0), 3);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void PlaceTile(string name, Vector3Int cell, int rotationH = 0, int rotationV = 0)
    {
        GameObject tile = GetTile(name);
        tile.transform.position = grid.GetCellCenterWorld(cell);
        tile.transform.rotation = Quaternion.Euler(-90 + rotationV * 90, rotationH * 90, 0);
        tile.transform.parent = transform;
    }

    GameObject GetTile(int index)
    {
        GameObject tile = blenderTileset.transform.GetChild(index).gameObject;
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        return Instantiate(tile, new Vector3(0, 0, 0), rotation);
    }

    GameObject GetTile(string name)
    {
        GameObject tile = blenderTileset.transform.Find(name).gameObject;
        Quaternion rotation = Quaternion.Euler(-90, 0, 0);
        return Instantiate(tile, new Vector3(0, 0, 0), rotation);
    }
}

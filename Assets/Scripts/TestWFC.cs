using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class TestWFC : MonoBehaviour
{
    public Vector3Int min, max;
    public WFCSockets borderSockets;
    public List<GeneralWFCTile> tileset;

    readonly WaveFunctionCollapse wfc = new();

    bool halted = false;
    int iteration = 0;

    // Start is called before the first frame update
    void Start()
    {
        List<WFCTile> tiles = new();
        foreach (var generalTile in tileset)
        {
            tiles.AddRange(generalTile.ToWFCTiles());
        }
        wfc.tileset = tiles;
        wfc.min = min;
        wfc.max = max;
        wfc.borderSockets = borderSockets;
        wfc.Initialize();
        // try
        // {
        //     wfc.DoTheThing();
        // }
        // catch (System.Exception err)
        // {
        //     Debug.Log(err);
        // }
        // RenderWFC();

        wfc.SetAt(new(0, 0, 0), wfc.tileset.Find(t => t.prefab.name == "ground"));
    }

    // Update is called once per frame
    void Update()
    {
        if (halted) return;
        if (wfc.IsCollapsed())
        {
            halted = true;
        }
        else
        {
            try
            {
                iteration++;
                wfc.Iterate();
            }
            catch (System.Exception err)
            {
                Debug.Log($"At iteration {iteration}");
                halted = true;
                Debug.LogError(err);
            }
        }
        RenderWFC();
    }

    void RenderWFC()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var (pos, tile) in wfc.Result)
        {
            if (tile == null || tile.prefab == null) continue;
            var go = tile.ToGameObject();
            go.transform.position = pos * 2;
            go.transform.parent = transform;
        }
    }
}

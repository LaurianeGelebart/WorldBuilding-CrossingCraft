using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class TestWFC : MonoBehaviour
{
    public Vector3Int min, max;
    public WFCSockets borderSockets;
    public List<GeneralWFCTile> tileset;

    public TextMeshProUGUI stateText;

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

        Reinitialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (halted)
        {
            return;
        }
        if (wfc.IsCollapsed())
        {
            stateText.text = "Collapsed";
            halted = true;
        }
        else
        {
            try
            {
                iteration++;
                stateText.text = $"Iteration {iteration}";
                wfc.Iterate();
            }
            catch (System.Exception err)
            {
                stateText.text = $"Error at iteration {iteration}";
                Debug.Log($"At iteration {iteration}");
                halted = true;
                Debug.LogError(err);
            }
        }
        RenderWFC();
    }

    public void Reinitialize()
    {
        wfc.Initialize();
        // wfc.SetAt(new(0, 0, 0), wfc.tileset.Find(t => t.prefab.name == "ground"));
        for (int x = wfc.min.x; x <= wfc.max.x; x++)
        {
            for (int z = wfc.min.z; z <= wfc.max.z; z++)
            {
                wfc.SetAt(new(x, wfc.max.y, z), wfc.tileset.Find(t => t.sockets.IsAll("-1"))); // air tiles
                wfc.SetAt(new(x, wfc.min.y, z), wfc.tileset.Find(t => t.sockets.IsAll("-2"))); // underground tiles
            }
        }
        halted = false;
        iteration = 0;
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

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WFCViewerManager : MonoBehaviour
{
    public WFCTileRenderer tileRenderer;
    public WFCTileRenderer mainTile;
    public GameObject xPositive, xNegative, yPositive, yNegative, zPositive, zNegative;
    public GameObject emptyCube;
    public TMP_InputField inputField;

    public float neighboursDistance = 30;
    public float neighboursScale = 1;
    public float neighboursPadding = 5;

    public List<GeneralWFCTile> generalTiles;
    private List<WFCTile> tiles = new();

    private int currentTileIndex = 0;
    private bool updated = true;

    public int CurrentTileIndex
    {
        get => currentTileIndex;
        set
        {
            currentTileIndex = value;
            updated = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tiles.Clear();
        foreach (var generalTile in generalTiles)
        {
            tiles.AddRange(generalTile.ToWFCTiles());
        }
        foreach (var tile in tiles)
        {
            tile.FetchValidNeighbors(tiles);
        }

        xPositive.transform.localPosition = new Vector3(neighboursDistance, 0, 0);
        xPositive.transform.localScale = Vector3.one * neighboursScale;
        xNegative.transform.localPosition = new Vector3(-neighboursDistance, 0, 0);
        xNegative.transform.localScale = Vector3.one * neighboursScale;
        yPositive.transform.localPosition = new Vector3(0, neighboursDistance, 0) / 2;
        yPositive.transform.localScale = Vector3.one * neighboursScale;
        yNegative.transform.localPosition = new Vector3(0, -neighboursDistance, 0) / 2;
        yNegative.transform.localScale = Vector3.one * neighboursScale;
        zPositive.transform.localPosition = new Vector3(0, 0, neighboursDistance);
        zPositive.transform.localScale = Vector3.one * neighboursScale;
        zNegative.transform.localPosition = new Vector3(0, 0, -neighboursDistance);
        zNegative.transform.localScale = Vector3.one * neighboursScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (!updated) return;

        updated = false;

        mainTile.Tile = tiles[currentTileIndex];

        DisplayNeighbors(xPositive.transform, tiles[currentTileIndex].XPosNeighbors);
        DisplayNeighbors(xNegative.transform, tiles[currentTileIndex].XNegNeighbors);
        DisplayNeighbors(yPositive.transform, tiles[currentTileIndex].YPosNeighbors);
        DisplayNeighbors(yNegative.transform, tiles[currentTileIndex].YNegNeighbors);
        DisplayNeighbors(zPositive.transform, tiles[currentTileIndex].ZPosNeighbors);
        DisplayNeighbors(zNegative.transform, tiles[currentTileIndex].ZNegNeighbors);
    }

    void DisplayNeighbors(Transform parent, List<WFCTile> neighbors)
    {
        foreach (Transform obj in parent) Destroy(obj.gameObject);

        var size = neighbors.Count;
        var width = Mathf.Ceil(Mathf.Sqrt(size));
        // foreach (var tile in neighbors)
        for (int i = 0; i < size; i++)
        {
            var tile = neighbors[i];
            var tileObj = Instantiate(tileRenderer, parent);
            tileObj.Tile = tile;
            tileObj.textDistance = 2;
            tileObj.transform.localPosition =
                neighboursPadding * new Vector3(i % width, 0, (float)Math.Floor(i / width))
                - new Vector3(width / 2, 0, width / 2);
        }
    }

    public void NextPress()
    {
        CurrentTileIndex = (currentTileIndex + 1) % tiles.Count;
        inputField.text = currentTileIndex.ToString();
    }
    public void PreviousPress()
    {
        CurrentTileIndex = (currentTileIndex - 1 + tiles.Count) % tiles.Count;
        inputField.text = currentTileIndex.ToString();
    }
    public void OnChange(string value)
    {
        CurrentTileIndex = int.Parse(value);
    }
}

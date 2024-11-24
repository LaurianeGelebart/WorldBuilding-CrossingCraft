using UnityEngine;

public class TestMap : MonoBehaviour
{
    public Gridmap gridmap;

    // Start is called before the first frame update
    void Start()
    {
        gridmap.ClearGameObjects();

        var simpleUnits = gridmap.CellTileset.FindAll(unit => unit.tile.size == Vector3Int.one);
        for (int idx = 0; idx < simpleUnits.Count; idx++)
        {
            int x = idx % 32;
            int z = idx / 32;

            var unitTile = simpleUnits[idx];
            gridmap.PlaceCell(unitTile, new Vector3Int(x, 0, z));
        }

        var bigTiles = gridmap.tileset.FindAll(tile => tile.size != Vector3Int.one);
        for (int p = 0; p < bigTiles.Count; p++)
        {
            var tile = bigTiles[p];
            for (int r = 0; r < 4; r++)
            {
                for (int f = 0; f < 8; f++)
                {
                    gridmap.PlaceTile(tile, new Vector3Int((r * 8 + f) * 3, 2, p), new Vector3Int(f & 1, f >> 1 & 1, f >> 2 & 1), (GridOrientation)r);
                }
            }
        }

        gridmap.RefreshGameObjects();
    }
}

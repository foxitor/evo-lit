using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSpriteReplacer : MonoBehaviour {
    public Tilemap targetTilemap;
    public TileBase[] originalTiles;
    public TileBase[] variationTiles;
    [Range(0f, 1f)]
    public float replaceChance = 0.5f;

    void Start() {
        if (Application.isPlaying) {
            ReplaceTilesRandomly();
        }
    }

    void ReplaceTilesRandomly() {
        if (targetTilemap == null) {
            Debug.LogError("No Tilemap fond."); return;
        }
        if (originalTiles.Length != variationTiles.Length) {
            Debug.LogError("Massives arent long enough"); return;
        }

        System.Random rand = new System.Random();
        BoundsInt bounds = targetTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++) {
            for (int y = bounds.yMin; y < bounds.yMax; y++) {
                Vector3Int cellPosition = new Vector3Int(x, y, 0);
                TileBase currentTile = targetTilemap.GetTile(cellPosition);
                if (currentTile == null)
                    continue;
                int index = System.Array.IndexOf(originalTiles, currentTile);
                if (index != -1) {
                    if (rand.NextDouble() <= replaceChance) {
                        targetTilemap.SetTile(cellPosition, variationTiles[index]);
                    }
                }
            }
        }
        targetTilemap.RefreshAllTiles();
    }
}
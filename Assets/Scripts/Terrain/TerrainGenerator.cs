using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;

        private void PlaceTile(Vector3Int pos, TileBase tile)
        {
            tilemap.SetTile(pos, tile);
        }
    }
}
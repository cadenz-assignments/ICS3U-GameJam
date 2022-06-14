using Save;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Players
{
    public class PlayerPlace : MonoBehaviour
    {
        private PlayerInventoryBehaviour _playerInv;
        private WorldSave _worldSave;
        private Camera _camera;
        
        [SerializeField] private Grid grid;
        [SerializeField] private TileRegistry tileRegistry;
        [SerializeField] private GameObject tilemapPrefab;

        private void Start()
        {
            _playerInv = GetComponent<PlayerInventoryBehaviour>();
            _worldSave = SaveManager.Instance.CurrentSave;
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = _camera.ScreenToWorldPoint(Input.mousePosition);

                var layer = 0;
                bool hasNextLayer;
                bool spotAvailable;
                LayerSave thisLayer;

                do
                {
                    layer++;
                    hasNextLayer = _worldSave.HasLayer(layer);
                    thisLayer = _worldSave.layers[layer - 1];
                    spotAvailable = thisLayer.IsPosAvailableSafe(thisLayer.tilemap.WorldToCell(mousePos));
                } while (!spotAvailable && hasNextLayer);

                if (!hasNextLayer)
                {
                    var instantiated = Instantiate(tilemapPrefab, grid.transform).GetComponent<Tilemap>();
                    instantiated.tileAnchor = new Vector3(0.5f, 0.5f, 0);
                    instantiated.GetComponent<TilemapRenderer>().sortingOrder = layer;
                    _worldSave.AddLayer(new LayerSave(_worldSave.path, "placed_y" + layer, instantiated, tileRegistry));
                    var nextLayer = _worldSave.layers[layer];
                    SetTile(mousePos, nextLayer, layer);
                }
                else
                {
                    SetTile(mousePos, thisLayer, layer);
                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // TODO break   
            }
        }

        private void SetTile(Vector3 mousePos, LayerSave layer, int layerNum)
        {
            var p = layer.tilemap.WorldToCell(mousePos);
            layer.LoadOrCreateChunk(p);
            var c = layer.GetChunkFromTilePos(p);
            c?.Set(new Vector2Int(p.x, p.y), tileRegistry.Get("sand"));
            layer.tilemap.SetTile(new Vector3Int(p.x, p.y, layerNum), tileRegistry.Get("sand"));
        }
    }
}

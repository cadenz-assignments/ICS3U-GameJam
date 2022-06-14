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
                LayerSave nextLayer;

                do
                {
                    layer++;
                    hasNextLayer = _worldSave.HasLayer(layer);
                    nextLayer = hasNextLayer ? _worldSave.layers[layer] : null;
                    var thisLayer = _worldSave.layers[layer - 1];
                    spotAvailable = thisLayer.IsPosAvailableSafe(thisLayer.Tilemap.WorldToCell(mousePos));
                } while (!spotAvailable && hasNextLayer);

                if (!hasNextLayer)
                {
                    var instantiated = Instantiate(tilemapPrefab, grid.transform).GetComponent<Tilemap>();
                    instantiated.tileAnchor = new Vector3(0f, 0f, 0);
                    _worldSave.AddLayer(new LayerSave(_worldSave.path, "placed_y" + layer, layer, instantiated, tileRegistry));
                    nextLayer = _worldSave.layers[layer];
                }
                SetTile(mousePos, nextLayer, layer);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                // TODO break   
            }
        }

        private void SetTile(Vector3 mousePos, LayerSave layer, int layerNum)
        {
            var p = layer.Tilemap.WorldToCell(mousePos);
            layer.LoadOrCreateChunk(p);
            var c = layer.GetChunkFromTilePos(p);
            c?.Set(new Vector2Int(p.x, p.y), tileRegistry.Get("sand"));
            layer.Tilemap.SetTile(new Vector3Int(p.x, p.y, layerNum), tileRegistry.Get("sand"));
        }
    }
}

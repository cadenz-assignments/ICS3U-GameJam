using System;
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
        
        [SerializeField] private Tilemap environment;
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
                LayerSave nextLayer;
                bool spotAvailable;

                do
                {
                    layer++;
                    hasNextLayer = _worldSave.HasLayer(layer);
                    nextLayer = hasNextLayer ? _worldSave.layers[layer] : null;
                    spotAvailable = hasNextLayer && nextLayer.IsPosAvailableSafe(nextLayer.tilemap.WorldToCell(mousePos));
                } while (!spotAvailable && hasNextLayer);

                if (!hasNextLayer)
                {
                    var instantiated = Instantiate(tilemapPrefab).GetComponent<Tilemap>();
                    instantiated.tileAnchor = new Vector3(0, layer, 0);
                    _worldSave.AddLayer(new LayerSave(_worldSave.path, "placed_y" + layer, instantiated, tileRegistry));
                    nextLayer = _worldSave.layers[layer];
                }
                
                var p = nextLayer.tilemap.WorldToCell(mousePos);
                var c = nextLayer.GetChunkFromTilePos(p);
                c?.Set(new Vector2Int(p.x, p.y), tileRegistry.Get("sand"));
            } 
            else if (Input.GetMouseButtonDown(1))
            {
                // TODO break    
            }
        }
    }
}

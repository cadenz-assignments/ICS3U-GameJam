using System.Collections.Generic;
using System.IO;
using System.Text;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Save
{
    public class WorldSave
    {
        public readonly string name;
        public readonly string path;
        public readonly int seed;

        public readonly List<LayerSave> layers;
        
        public WorldSave(string name, int seed = 54343)
        {
            var sb = new StringBuilder(Application.persistentDataPath);
            sb.Append("/Saves/");
            sb.Append(name);
            sb.Append("/");
            path = sb.ToString();

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            layers = new List<LayerSave>();
            this.name = name;
            this.seed = seed;
        }

        public void AddLayer(LayerSave layerSave)
        {
            layers.Add(layerSave);
        }

        public void LoadExistingLayers(TileRegistry tileRegistry)
        {
            var grid = Object.FindObjectOfType<Grid>();
            var prefab = Resources.Load<GameObject>("Prefabs/PlayerPlacedLevel");

            foreach (var layerName in Directory.GetDirectories(path + "layers/", "placed_y*"))
            {
                var ln = layerName[(layerName.LastIndexOf('/') + 1)..];
                var instantiated = Object.Instantiate(prefab, grid.transform).GetComponent<Tilemap>();
                instantiated.tileAnchor = new Vector3(0, 0, 0);
                AddLayer(new LayerSave(path, ln, int.Parse(ln.Replace("placed_y", "")), instantiated, tileRegistry));
            }
        }

        public void UnloadAll()
        {
            foreach (var layer in layers)
            {
                layer.UnloadAllChunks();
            }
        }

        public bool HasLayer(int index)
        {
            return index < layers.Count && index >= 0;
        }
    }
}
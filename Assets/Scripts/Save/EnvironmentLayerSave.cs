using System;
using System.Collections;
using System.IO;
using Terrain;
using UnityEngine;

namespace Save
{
    [Serializable]
    public class EnvironmentLayerSave : LayerSave
    {
        private readonly TerrainGenerator _terrainGenerator;
        
        public EnvironmentLayerSave(string path, TerrainGenerator terrainGenerator) : base(path, "environment", terrainGenerator.tilemap, terrainGenerator.tileRegistry)
        {
            _terrainGenerator = terrainGenerator;
        }

        public override void LoadOrCreateChunk(Vector2Int pos)
        {
            if (DoesChunkExist(pos, out var isLoaded))
            {
                if (isLoaded) return;
                var c = JsonUtility.FromJson<Chunk>(File.ReadAllText(chunkPath + GetChunkFileName(pos)));
                c.tileRegistry = _terrainGenerator.tileRegistry;
                c.Fill(_terrainGenerator.tilemap);
                loadedChunks.Add(pos, c);
            }
            else
            {
                loadedChunks.Add(pos, _terrainGenerator.GenerateNewChunk(pos));
            }
        }
    }
}
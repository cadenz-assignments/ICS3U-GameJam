using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Terrain
{
    [Serializable]
    public class Save
    {
        private readonly string _name;
        private readonly string _path;
        private readonly string _chunkPath;
        private readonly TerrainManager _terrainManager;
        
        public Dictionary<Vector2Int, Chunk> LoadedChunks;

        public Save(string name, TerrainManager terrainManager)
        {
            _name = name;
            
            var sb = new StringBuilder(Application.persistentDataPath);
            sb.Append("/Save/");
            sb.Append(name);
            sb.Append("/");
            _path = sb.ToString();

            sb.Append("Chunks/");
            _chunkPath = sb.ToString();

            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            
            if (!Directory.Exists(_chunkPath))
            {
                Directory.CreateDirectory(_chunkPath);
            }

            LoadedChunks = new Dictionary<Vector2Int, Chunk>();
            _terrainManager = terrainManager;
        }

        public bool DoesChunkExist(Vector2Int pos, out bool isLoaded)
        {
            if (LoadedChunks.ContainsKey(pos))
            {
                isLoaded = true;
                return true;
            }

            isLoaded = false;
            return File.Exists(_chunkPath + GetChunkFileName(pos));
        }

        private string GetChunkFileName(Vector2Int pos)
        {
            return pos.ToString().Replace(", ", "_").Replace("(", "").Replace(")", "") + ".json";
        }
        
        public void Add(Chunk chunk)
        {
            LoadedChunks.Add(chunk.pos, chunk);
        }

        public void LoadOrCreateChunk(Vector2Int pos)
        {
            if (DoesChunkExist(pos, out var isLoaded))
            {
                if (isLoaded) return;
                var c = JsonUtility.FromJson<Chunk>(File.ReadAllText(_chunkPath + GetChunkFileName(pos)));
                c.Fill(_terrainManager.tilemap);
                LoadedChunks.Add(pos, c);
            }
            else
            {
                LoadedChunks.Add(pos, _terrainManager.GenerateNewChunk(pos));
            }
        }
        
        public void UnloadChunks(params Vector2Int[] poses)
        {
            foreach (var pos in poses)
            {
                if (!LoadedChunks.ContainsKey(pos)) continue;
                LoadedChunks.Remove(pos, out var chunk);
                File.WriteAllText(_chunkPath + GetChunkFileName(pos), JsonUtility.ToJson(chunk, false));
                foreach (var p in chunk.poses)
                {
                    _terrainManager.tilemap.SetTile(new Vector3Int(p.x, p.y, 0), null);
                }
            }
        }
    }
}
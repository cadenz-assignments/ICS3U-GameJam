using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Save
{
    public class LayerSave
    {
        public readonly Tilemap Tilemap;
        public readonly int Layer;
        protected readonly string ChunkPath;
        protected readonly TileRegistry TileRegistry;
        
        public Dictionary<Vector2Int, Chunk> loadedChunks;

        public LayerSave(string path, string layerName, int layer, Tilemap tilemap, TileRegistry tileRegistry)
        {
            var sb = new StringBuilder(path);
            
            sb.Append("layers/");
            sb.Append(layerName);
            sb.Append("/");
            
            ChunkPath = sb.ToString();
            
            if (!Directory.Exists(ChunkPath))
            {
                Directory.CreateDirectory(ChunkPath);
            }
            
            loadedChunks = new Dictionary<Vector2Int, Chunk>();
            Tilemap = tilemap;
            TileRegistry = tileRegistry;
            Layer = layer;
        }
        
        public bool DoesChunkExist(Vector2Int pos, out bool isLoaded)
        {
            if (loadedChunks.ContainsKey(pos))
            {
                isLoaded = true;
                return true;
            }

            isLoaded = false;
            return File.Exists(ChunkPath + GetChunkFileName(pos));
        }

        public void Add(Chunk chunk)
        {
            loadedChunks.Add(chunk.pos, chunk);
        }

        public void UnloadChunks(params Vector2Int[] poses)
        {
            UnloadChunks((IEnumerable<Vector2Int>) poses);
        }

        /// <summary>
        /// Checks if there is an empty space at pos. Will return false if chunk is not loaded.
        /// </summary>
        /// <param name="pos">position to check</param>
        /// <returns></returns>
        public bool IsPosAvailableSafe(Vector3Int pos)
        {
            var cp = Chunk.ToChunkPos(pos);
            
            if (DoesChunkExist(cp, out var isLoaded) && isLoaded)
            {
                return loadedChunks[cp].poses.Contains(new Vector2Int(pos.x, pos.y));
            }

            return false;
        }

        [CanBeNull]
        public Chunk GetChunkFromTilePos(Vector3Int pos)
        {
            var cp = Chunk.ToChunkPos(pos);
            
            if (DoesChunkExist(cp, out var isLoaded) && isLoaded)
            {
                return loadedChunks[cp];
            }

            return null;
        }

        public void UnloadChunks(IEnumerable<Vector2Int> poses)
        {
            foreach (var pos in poses)
            {
                if (!loadedChunks.ContainsKey(pos)) continue;
                loadedChunks.Remove(pos, out var chunk);
                SaveChunk(chunk);
                foreach (var p in chunk.poses)
                {
                    Tilemap.SetTile(new Vector3Int(p.x, p.y, 0), null);
                }
            }
        }

        public void UnloadAllChunks()
        {
            UnloadChunks(new List<Vector2Int>(loadedChunks.Keys));
        }
        
        private void SaveChunk(Chunk chunk)
        {
            File.WriteAllText(ChunkPath + GetChunkFileName(chunk.pos), JsonUtility.ToJson(chunk, false));
        }
        
        protected string GetChunkFileName(Vector2Int pos)
        {
            return pos.ToString().Replace(", ", "_").Replace("(", "").Replace(")", "") + ".json";
        }

        public void LoadOrCreateChunk(Vector3Int pos)
        { 
            LoadOrCreateChunk(Chunk.ToChunkPos(pos));
        }
        
        public virtual void LoadOrCreateChunk(Vector2Int pos)
        {
            if (DoesChunkExist(pos, out var isLoaded))
            {
                if (isLoaded) return;
                var c = JsonUtility.FromJson<Chunk>(File.ReadAllText(ChunkPath + GetChunkFileName(pos)));
                c.tileRegistry = TileRegistry;
                c.Fill(Tilemap);
                loadedChunks.Add(pos, c);
            }
            else
            {
                loadedChunks.Add(pos, new Chunk(TileRegistry, pos, Layer));
            }
        }
    }
}
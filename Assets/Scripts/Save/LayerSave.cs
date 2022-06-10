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
        protected readonly string chunkPath;
        public readonly Tilemap tilemap;
        protected readonly TileRegistry tileRegistry;
        
        public Dictionary<Vector2Int, Chunk> loadedChunks;

        public LayerSave(string path, string layerName, Tilemap tilemap, TileRegistry tileRegistry)
        {
            var sb = new StringBuilder(path);
            
            sb.Append("layers/");
            sb.Append(layerName);
            sb.Append("/");
            
            chunkPath = sb.ToString();
            
            if (!Directory.Exists(chunkPath))
            {
                Directory.CreateDirectory(chunkPath);
            }
            
            loadedChunks = new Dictionary<Vector2Int, Chunk>();
            this.tilemap = tilemap;
            this.tileRegistry = tileRegistry;
        }
        
        public bool DoesChunkExist(Vector2Int pos, out bool isLoaded)
        {
            if (loadedChunks.ContainsKey(pos))
            {
                isLoaded = true;
                return true;
            }

            isLoaded = false;
            return File.Exists(chunkPath + GetChunkFileName(pos));
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
                    tilemap.SetTile(new Vector3Int(p.x, p.y, 0), null);
                }
            }
        }

        public void UnloadAllChunks()
        {
            UnloadChunks(new List<Vector2Int>(loadedChunks.Keys));
        }
        
        private void SaveChunk(Chunk chunk)
        {
            File.WriteAllText(chunkPath + GetChunkFileName(chunk.pos), JsonUtility.ToJson(chunk, false));
        }
        
        protected string GetChunkFileName(Vector2Int pos)
        {
            return pos.ToString().Replace(", ", "_").Replace("(", "").Replace(")", "") + ".json";
        }

        public virtual IEnumerator LoadOrCreateChunk(Vector2Int pos)
        {
            if (DoesChunkExist(pos, out var isLoaded))
            {
                if (isLoaded) yield break;
                var c = JsonUtility.FromJson<Chunk>(File.ReadAllText(chunkPath + GetChunkFileName(pos)));
                c.tileRegistry = tileRegistry;
                c.Fill(tilemap);
                loadedChunks.Add(pos, c);
            }
            else
            {
                loadedChunks.Add(pos, new Chunk(tileRegistry, pos));
            }
        }
    }
}
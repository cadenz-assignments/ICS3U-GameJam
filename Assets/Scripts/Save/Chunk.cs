using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Terrain;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Save
{
    [Serializable]
    public class Chunk
    {
        [NonSerialized] public const int ChunkSize = 16;
        
        public Vector2Int pos;
        public int layer;
        
        public Vector2Int min;
        public Vector2Int max;

        public List<Vector2Int> poses;
        public List<string> tiles;

        [NonSerialized] public TileRegistry tileRegistry;

        /// Creates the chunk object but does NOT generate the tiles. Use LoadOrGenerate instead. 
        public Chunk(TileRegistry tileRegistry, Vector2Int pos, int layer)
        {
            poses = new List<Vector2Int>();
            tiles = new List<string>();
            
            this.pos = pos;
            this.layer = layer;
            min = pos * ChunkSize;
            max = min + new Vector2Int(ChunkSize, ChunkSize);

            this.tileRegistry = tileRegistry;
        }

        public void Set(Vector2Int p, TileBase tile)
        {
            // replaces current tile at position if it already exists
            if (poses.Contains(p))
            {
                var index = poses.IndexOf(p);
                tiles[index] = tileRegistry.GetId(tile);
                Invalidate();
                return;
            }
            
            poses.Add(p);
            tiles.Add(tileRegistry.GetId(tile));
            Invalidate();
        }

        public void Remove(Vector2Int p)
        {
            var index = poses.IndexOf(p);
            poses.RemoveAt(index);
            tiles.RemoveAt(index);
            Invalidate();
        }
        
        [CanBeNull]
        public TileBase Get(Vector2Int p)
        {
            Invalidate();
            var index = poses.IndexOf(p);
            return index == -1 ? null : tileRegistry.Get(tiles[index]);
        }

        public void Fill(Tilemap tilemap)
        {
            foreach (var p in poses)
            {
                var t = Get(p);
                tilemap.SetTile(new Vector3Int(p.x, p.y, layer), t);
            }
        }

        public bool InsideChunk(Vector3 p)
        {
            return p.x >= min.x && p.x < max.x && p.y >= min.y && p.y < max.y;
        }

        public bool InsideChunk(Vector2 p)
        {
            return p.x >= min.x && p.x < max.x && p.y >= min.y && p.y < max.y;
        }

        private void Invalidate()
        {
            // for debugging to check for desync. Shouldn't be a problem in production if it doesn't happen in dev environment
            #if UNITY_EDITOR
                if (poses.Count != tiles.Count)
                {
                    throw new Exception("Pos Tile de-sync in chunk " + pos);
                }
            #endif
        }

        public static implicit operator BoundsInt(Chunk chunk)
        {
            var min = chunk.min;
            return new BoundsInt(min.x, min.y, 0, ChunkSize, ChunkSize, 0);
        }

        public static Vector2Int ToChunkPos(Vector3 pos)
        {
            return new Vector2Int((int) MathF.Floor(pos.x / ChunkSize), (int) MathF.Floor(pos.y / ChunkSize));
        }
        
        public static Vector2Int ToChunkPos(Vector2Int pos)
        {
            return new Vector2Int((int) MathF.Floor((float) pos.x / ChunkSize), (int) MathF.Floor((float) pos.y / ChunkSize));
        }
    }
}
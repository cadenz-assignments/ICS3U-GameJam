using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Terrain.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    [Serializable]
    public class Chunk
    {
        [NonSerialized] public const int ChunkSize = 16;
        
        public Vector2Int pos;
        
        public Vector2Int min;
        public Vector2Int max;

        public List<Vector2Int> poses;
        public List<string> tiles;

        public TileRegistry tileRegistry;
        
        /// Creates the chunk object but does NOT generate the tiles. You should NEVER call this. Use LoadOrGenerate instead. 
        public Chunk(TileRegistry tileRegistry, Save save, Vector2Int pos)
        {
            poses = new List<Vector2Int>();
            tiles = new List<string>();
            
            this.pos = pos;
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
                tilemap.SetTile(new Vector3Int(p.x, p.y, 0), t);
            }
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

        public static Vector2Int ToChunkPos(Vector2Int pos)
        {
            return new Vector2Int((int) Math.Floor(pos.x / 32f), (int) Math.Floor(pos.y / 32f));
        }
    }
}
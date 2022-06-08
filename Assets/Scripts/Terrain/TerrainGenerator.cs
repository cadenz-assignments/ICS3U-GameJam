using System;
using PathCreation;
using Terrain.Tiles;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        public Tilemap tilemap;
        public TileRegistry tileRegistry;

        [NonSerialized] public TileBase SandTile;
        [NonSerialized] public TileBase WaterTile;
        [NonSerialized] public TileBase GrassTile;
        [NonSerialized] public Save Save;

        private void Awake()
        {
            Save = new Save("test_save", this);
            SandTile = tileRegistry.Get("sand");
            WaterTile = tileRegistry.Get("water");
            GrassTile = tileRegistry.Get("grass");
        }

        public Chunk GenerateNewChunk(Vector2Int chunkPos)
        {
            var chunk = new Chunk(tileRegistry, Save, chunkPos);
            var min = chunk.min;
            var max = chunk.max;
            
            for (float i = min.x; i < max.x; i++)
            {
                for (float j = min.y; j < max.y; j++)
                {
                    var pos = new Vector3Int((int)i, (int)j);
                    var noiseValue = Mathf.Clamp(Mathf.PerlinNoise(i/Chunk.ChunkSize, j/Chunk.ChunkSize), 0, 1);
                    if (tilemap.GetTile(pos) is null)
                    {
                        PlaceTile(chunk, pos, noiseValue > 0.6 ? WaterTile : GrassTile);
                    }
                }
            }

            AddSand(chunk);

            return chunk;
        }

        // uses a bezier curve generator of with n amount of control points at random locations between the 2 ends and connect points using water tiles
        private void PlaceRiver(Chunk chunk, Vector2Int start, Vector2Int end, int offset, int thickness)
        {
            // amount of control points between the start and end point, scales with the length of the river
            var length = (end - start).magnitude;
            
            var controlCount = Random.Range(1, (int) length / 3);
            var points = new Vector2[controlCount + 2];
            
            // first point being the start point
            points[0] = start;
            
            for (var i = 1; i < controlCount - 1; i++)
            {
                var offsetX = Random.Range(-offset, offset);
                var offsetY = Random.Range(-offset, offset);

                var lastPos = i == 0 ? start : points[i - 1];

                points[i] = lastPos + new Vector2Int(offsetX, offsetY);
            }

            // last point being the end point
            points[^1] = end;

            var delta = length / 2000;
            var halfThickness = thickness / 2f * length / 80;
            
            var bp = new BezierPath(points, PathSpace.xy);
            var vp = new VertexPath(bp, GetComponent<Transform>(), delta);

            for (var t = 0f; t < 1; t += delta)
            {
                var pos = vp.GetPointAtTime(t, EndOfPathInstruction.Stop);
                
                for (var i = pos.x - halfThickness; i < pos.x + halfThickness; i++)
                {
                    for (var j = pos.y - halfThickness; j < pos.y + halfThickness; j++)
                    {
                        PlaceTile(chunk, new Vector3(i, j, 0), WaterTile);
                    }
                }
            }
        }

        private void AddSand(Chunk chunk)
        {
            var min = chunk.min;
            var max = chunk.max;
            
            for (var i = min.x; i < max.x; i++)
            {
                for (var j = min.y; j < max.y; j++)
                {
                    if (tilemap.GetTile(new Vector3Int(i, j, 0)) != WaterTile) continue;
                    
                    for (var x = -1; x <= 1; x++) {
                        for (var y = -1; y <= 1; y++) {
                            if (x == 0 && y == 0) {
                                continue;
                            }

                            var pos = new Vector3Int(i + x, j + y, 0);
                               
                            if (!chunk.InsideChunk(pos)) continue;

                            if (tilemap.GetTile(pos) != WaterTile)
                            {
                                PlaceTile(chunk, pos, SandTile);
                            }
                        }
                    }
                }    
            }
        }

        private void PlaceTile(Chunk chunk, Vector3Int pos, TileBase tile)
        {
            chunk.Set(new Vector2Int(pos.x, pos.y), tile);
            tilemap.SetTile(pos, tile);
        }
        
        private void PlaceTile(Chunk chunk, Vector3 pos, TileBase tile)
        {
            PlaceTile(chunk, new Vector3Int((int) pos.x, (int) pos.y, (int) pos.z), tile);
        }
    }
}
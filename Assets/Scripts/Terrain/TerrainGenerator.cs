using PathCreation;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Terrain
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap tilemap;
        [SerializeField] private TileBase grassTile;
        [SerializeField] private TileBase waterTile;
        [SerializeField] private TileBase sandTile;

        public void GenerateNew(BoundsInt bound)
        {
            if (Random.value > 0.6)
            {
                for (var i = 0; i < Random.Range(1, 2); i++)
                {
                    var x1 = Random.Range(bound.xMin, bound.xMax);
                    var x2 = Random.Range(bound.xMin, bound.xMax);
                    var y1 = Random.Range(bound.yMin, bound.yMax);
                    var y2 = Random.Range(bound.yMin, bound.yMax);
                
                    CreateRiver(new Vector2Int(x1, y1), new Vector2Int(x2, y2), Random.Range(2, 7), Random.Range(2, 6));
                }
            }
            
            AddSand(bound);
            
            for (var i = 0; i < Random.Range(1, 2); i++)
            {
                var x1 = Random.Range(bound.xMin, bound.xMax);
                var x2 = Random.Range(bound.xMin, bound.xMax);
                var y1 = Random.Range(bound.yMin, bound.yMax);
                var y2 = Random.Range(bound.yMin, bound.yMax);
                
                CreateRiver(new Vector2Int(x1, y1), new Vector2Int(x2, y2), Random.Range(2, 7), Random.Range(2, 6));
            }
        }

        // uses a bezier curve generator of with n amount of control points at random locations between the 2 ends and connect points using water tiles
        private void CreateRiver(Vector2Int start, Vector2Int end, int offset, int thickness)
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

                points[i] = lastPos + new Vector2Int((int) offsetX, (int) offsetY);
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
                        PlaceTile(new Vector3(i, j, 0), waterTile);
                    }
                }
            }
        }

        private void AddSand(BoundsInt bounds)
        {
            for (var i = bounds.xMin; i < bounds.xMax; i++)
            {
                for (var j = bounds.yMin; j < bounds.yMax; j++)
                {
                    if (tilemap.GetTile(new Vector3Int(i, j, 0)) != waterTile) continue;
                    
                    for (var x = -1; x <= 1; x++) {
                        for (var y = -1; y <= 1; y++) {
                            if (x == 0 && y == 0) {
                                continue;
                            }

                            var pos = new Vector3Int(i + x, j + y, 0);
                                
                            if (tilemap.GetTile(pos) is null)
                            {
                                PlaceTile(pos, sandTile);
                            }
                        }
                    }
                }    
            }
        }
        
        private void PlaceTile(Vector3Int pos, TileBase tile)
        {
            tilemap.SetTile(pos, tile);
        }
        
        private void PlaceTile(Vector3 pos, TileBase tile)
        {
            PlaceTile(new Vector3Int((int) pos.x, (int) pos.y, (int) pos.z), tile);
        }
    }
}
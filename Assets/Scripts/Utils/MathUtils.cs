using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public static IEnumerable<Vector3Int> GetAllPositionsBetween(this Vector2Int min, Vector2Int max)
        {
            for (var i = min.x; i < max.x; i++)
            {
                for (var j = min.y; j < max.y; j++)
                {
                    yield return new Vector3Int(i, j, 0);
                }
            }
        }
    }
}
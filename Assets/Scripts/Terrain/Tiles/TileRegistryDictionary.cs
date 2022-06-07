using System;
using UnityEngine.Tilemaps;

namespace Terrain.Tiles
{
    [Serializable]
    public class TileRegistryDictionary : SerializableDictionary<string, TileBase>
    {
    }
}
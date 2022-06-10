using System;
using UnityEngine.Tilemaps;

namespace Terrain
{
    [Serializable]
    public class TileRegistryDictionary : SerializableDictionary<string, TileBase>
    {
    }
}
using Terrain.Tiles;
using UnityEditor;

namespace Editor
{
    [CustomPropertyDrawer(typeof(TileRegistryDictionary))]
    public class TileRegistryDictionaryDrawer : SerializableDictionaryPropertyDrawer
    {
    }
}
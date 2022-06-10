using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Terrain
{
    [CreateAssetMenu(fileName = "Tile Registry", menuName = "Registry/Tile Registry")]
    public class TileRegistry : ScriptableObject
    {
        [SerializeField] private TileRegistryDictionary registry;

        [CanBeNull]
        public TileBase Get(string id)
        {
            return !registry.ContainsKey(id) ? null : registry[id];
        }

        [CanBeNull]
        public string GetId(TileBase tileBase)
        {
            return registry.First(kv => kv.Value.name == tileBase.name).Key;
        }
    }
}
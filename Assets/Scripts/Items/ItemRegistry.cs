using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Item Registry", menuName = "Registry/Item Registry")]
    public class ItemRegistry : ScriptableObject
    {
        public ItemRegistryDictionary registry;
    }
}
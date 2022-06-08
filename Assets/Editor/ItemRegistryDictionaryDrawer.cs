using Items;
using UnityEditor;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ItemRegistryDictionary))]
    public class ItemRegistryDictionaryDrawer : SerializableDictionaryPropertyDrawer
    {
        
    }
}
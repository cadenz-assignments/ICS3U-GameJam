using System;

namespace Items
{
    [Serializable]
    public class ItemRegistryDictionary : SerializableDictionary<string, Item>
    {
    }
}
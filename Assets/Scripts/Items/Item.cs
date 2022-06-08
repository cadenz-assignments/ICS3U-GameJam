using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Item")]
    public class Item : ScriptableObject
    {
        public Sprite sprite;
        public string itemName;
        public string itemDescription;
        public int maxStackCount = 16;
    }
}
using System;

namespace Items
{
    [Serializable]
    public class ItemInstance
    {
        public static readonly ItemInstance Empty = new(null, 0);
        
        public Item item;
        public int count;

        public ItemInstance(Item item, int count)
        {
            this.item = item;
            this.count = count;
        }

        public static bool operator ==(ItemInstance a, ItemInstance b)
        {
            return a is not null && b is not null && a.item == b.item && a.count == b.count;
        }
        
        public static bool operator !=(ItemInstance a, ItemInstance b)
        {
            return a is null || b is null || a.item != b.item || a.count != b.count;
        }
    }
}
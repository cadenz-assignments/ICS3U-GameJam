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

        public ItemInstance(ItemInstance copy) : this(copy.item, copy.count)
        {
        }
        
        public static bool operator ==(ItemInstance a, ItemInstance b)
        {
            return a is not null && b is not null && a.item == b.item && a.count == b.count;
        }
        
        public static bool operator !=(ItemInstance a, ItemInstance b)
        {
            return a is null || b is null || a.item != b.item || a.count != b.count;
        }
        
        protected bool Equals(ItemInstance other)
        {
            return Equals(item, other.item) && count == other.count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemInstance) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(item, count);
        }
    }
}
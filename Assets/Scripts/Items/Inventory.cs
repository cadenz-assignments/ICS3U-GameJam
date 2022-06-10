using System;
using System.Linq;

namespace Items
{
    [Serializable]
    public class Inventory
    {
        public event Action<int, ItemInstance> SlotChanged; 

        private ItemInstance[] _items;

        public Inventory(int size)
        {
            _items = new ItemInstance[size];
            Array.Fill(_items, ItemInstance.Empty);
        }

        public ItemInstance this[int index]
        {
            get => _items[index];
            set
            {
                var v = value ?? ItemInstance.Empty;
                _items[index] = v;
                SlotChanged?.Invoke(index, v);
            }
        }

        public void SetWithoutNotification(int index, ItemInstance item)
        {
            item ??= ItemInstance.Empty;
            _items[index] = item;
        }

        public void Add(ItemInstance instance)
        {
            var index = Array.FindIndex(_items, i =>
            {
                var item = i.item;
                return item == instance.item && i.count < item.maxStackCount;
            });
            
            if (index != -1)
            {
                var item = this[index];
                item.count++;
                SlotChanged?.Invoke(index, item);
            }
            else
            {
                var emptySpot = Array.FindIndex(_items, i => i == ItemInstance.Empty);
                if (emptySpot != -1)
                {
                    this[emptySpot] = instance;
                }
            }
        }

        public void Remove(int index)
        {
            this[index] = ItemInstance.Empty;
        }

        public bool IsFull()
        {
            // check if if all the items in the inventory are not empty items and if they all reached their max stack size.
            return _items.Count(i => i != ItemInstance.Empty && i.count >= i.item.maxStackCount) >= _items.Length;
        }
    }
}
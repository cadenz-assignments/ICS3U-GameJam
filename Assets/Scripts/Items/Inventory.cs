using System;
using System.Linq;

namespace Items
{
    [Serializable]
    public class Inventory
    {
        public event Action<int, ItemInstance> SlotChanged; 

        public ItemInstance[] items;
        private int _index;

        public Inventory(int size)
        {
            items = new ItemInstance[size];
            _index = 0;
        }

        public ItemInstance this[int index]
        {
            get => items[index];
            set
            {
                var v = value;
                
                if (v == null)
                {
                    v = ItemInstance.Empty;
                }
                
                items[index] = v;
                SlotChanged?.Invoke(index, v);
            }
        }

        public void Push(ItemInstance instance)
        {
            if (_index < items.Length)
            {
                items[_index] = instance;
                _index++;   
            }
        }

        public void Pop()
        {
            if (_index > 0)
            {
                items[_index] = ItemInstance.Empty;
                _index--;
            }
        }

        public bool IsFull()
        {
            return items.Count(i => i != null && i != ItemInstance.Empty) >= items.Length;
        }
    }
}
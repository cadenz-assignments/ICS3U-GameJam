using System;
using Items;

namespace Players
{
    public class PlayerInventoryBehaviour : InventoryBehaviour
    {
        public event Action<int, ItemInstance> InventoryChanged;
        public Inventory hotbar;

        protected override void Start()
        {
            base.Start();
            hotbar = new Inventory(5);

            hotbar.SlotChanged += UpdateHotbar;
            items.SlotChanged += UpdateInventory;
        }

        private void OnDisable()
        {
            hotbar.SlotChanged -= UpdateHotbar;
            items.SlotChanged -= UpdateInventory;
        }

        public bool Add(ItemInstance item)
        {
            if (hotbar.IsFull())
            {
                if (items.IsFull())
                {
                    return false;
                }
                items.Push(item);
            }
            else
            {
                hotbar.Push(item);
            }

            return true;
        }

        private void UpdateHotbar(int i, ItemInstance item)
        {
            InventoryChanged?.Invoke(i, item);
        }

        private void UpdateInventory(int i, ItemInstance item)
        {
            InventoryChanged?.Invoke(i + 5, item);
        }
    }
}
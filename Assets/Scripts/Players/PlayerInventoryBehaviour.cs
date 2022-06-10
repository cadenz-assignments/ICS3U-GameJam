using System;
using Items;
using UnityEngine;

namespace Players
{
    public class PlayerInventoryBehaviour : InventoryBehaviour
    {
        public event Action<int, ItemInstance> InventoryChanged;
        public event Action<int> SelectedItemChanged;
        
        public Inventory hotbar;
        public int selectedItem;
        
        protected override void Start()
        {
            base.Start();
            selectedItem = 0;
            hotbar = new Inventory(5);

            hotbar.SlotChanged += UpdateHotbar;
            items.SlotChanged += UpdateInventory;
        }

        private void Update()
        {
            // there is probably a lot better ways of doing this but it is fine for now.
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedItem = 0;
                SelectedItemChanged?.Invoke(selectedItem);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                selectedItem = 1;
                SelectedItemChanged?.Invoke(selectedItem);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                selectedItem = 2;
                SelectedItemChanged?.Invoke(selectedItem);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                selectedItem = 3;
                SelectedItemChanged?.Invoke(selectedItem);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                selectedItem = 4;
                SelectedItemChanged?.Invoke(selectedItem);
            }
        }

        private void OnDisable()
        {
            hotbar.SlotChanged -= UpdateHotbar;
            items.SlotChanged -= UpdateInventory;
        }

        public override bool Add(ItemInstance item)
        {
            if (hotbar.IsFull())
            {
                if (items.IsFull())
                {
                    return false;
                }
                items.Add(item);
            }
            else
            {
                hotbar.Add(item);
            }

            return true;
        }

        public ItemInstance this[int index]
        {
            get => index >= 5 ? items[index - 5] : hotbar[index];
            set
            {
                if (index >= 5)
                {
                    items[index - 5] = value;
                    return;
                }

                hotbar[index] = value;
            }
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
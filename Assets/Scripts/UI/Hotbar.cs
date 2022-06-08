using Items;
using Players;
using UnityEngine;

namespace UI
{
    public class Hotbar : MonoBehaviour
    {
        public ItemSlot[] slots;
        public PlayerInventoryBehaviour playerInventory;
        
        private void Start()
        {
            playerInventory.hotbar.SlotChanged += UpdateHotbar;
        }

        private void OnDisable()
        {
            playerInventory.hotbar.SlotChanged -= UpdateHotbar;
        }

        private void UpdateHotbar(int index, ItemInstance item)
        {
            slots[index].Item = item;
        }
    }
}
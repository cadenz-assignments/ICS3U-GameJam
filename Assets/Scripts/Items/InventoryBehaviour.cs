using UnityEngine;

namespace Items
{
    public class InventoryBehaviour : MonoBehaviour
    {
        public Inventory items;

        protected virtual void Start()
        {
            items = new Inventory(15);
        }

        public virtual bool Add(ItemInstance item)
        {
            if (items.IsFull()) return false;
            items.Add(item);
            return true;
        }
    }
}
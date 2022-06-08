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
    }
}
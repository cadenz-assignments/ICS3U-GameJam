using Items;

namespace Players
{
    public class PlayerInventoryBehaviour : InventoryBehaviour
    {
        public Inventory hotbar;

        protected override void Start()
        {
            base.Start();
            hotbar = new Inventory(5);
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
    }
}
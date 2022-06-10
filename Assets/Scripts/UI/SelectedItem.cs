using Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SelectedItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI countText;
        [SerializeField] private InventoryBehaviour inventory;

        private ItemInstance _selected;
        private Image _image;
        private Camera _camera;
        
        private void Start()
        {
            _image = GetComponent<Image>();
            _selected = ItemInstance.Empty;
            transform.position = new Vector3(-1000000, -1000000, 0);
            _camera = Camera.main;
        }

        private void Update()
        {
            if (_selected != ItemInstance.Empty)
            {
                var p = _camera.ScreenToWorldPoint(Input.mousePosition);
                transform.position = new Vector3(p.x, p.y, 0);
            }
        }

        public void Select(ItemInstance itemInstance)
        {
            _selected = itemInstance;
            _image.sprite = itemInstance.item.sprite;
            countText.text = itemInstance.count.ToString();
        }

        public ItemInstance Unselect()
        {
            transform.position = new Vector3(-1000000, -1000000, 0);
            var item = new ItemInstance(_selected);
            _selected = ItemInstance.Empty;
            return item;
        }

        public ItemInstance CurrentItem()
        {
            return _selected;
        }
    }
}
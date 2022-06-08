using Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private ItemInstance item;
        public ItemInstance Item
        {
            get => item;
            set
            {
                item = value;
                OnItemChanged();
            }
        }
        
        public Image itemImage;
        public TextMeshProUGUI itemCount;

        private Image _thisImage;
        
        private void OnItemChanged()
        {
            item ??= ItemInstance.Empty;
            
            if (item.item != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.item.sprite;
            }
            else
            {
                itemImage.gameObject.SetActive(false);
            }

            if (item.count != 0)
            {
                itemCount.gameObject.SetActive(true);
                itemCount.text = item.count.ToString();
            }
            else
            {
                itemCount.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            _thisImage = GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.value(gameObject, c => _thisImage.color = c, Color.white, new Color(0.6f, 0.6f, 0.6f, 1), 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LeanTween.value(gameObject, c => _thisImage.color = c, new Color(0.6f, 0.6f, 0.6f, 1), Color.white, 0.1f);
        }
    }
}
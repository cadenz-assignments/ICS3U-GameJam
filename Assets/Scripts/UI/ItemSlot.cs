using System;
using Items;
using Players;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private ItemInstance item;

        public ItemInstance Item
        {
            set
            {
                item = value;
                OnItemChanged();
            }
        }

        public Image itemImage;
        public TextMeshProUGUI itemCount;

        public int thisIndex;
        public PlayerInventoryBehaviour inventory;
        
        private Image _thisImage;
        private SelectedItem _selectedItem;

        private void OnItemChanged()
        {
            item ??= ItemInstance.Empty;

            if (item.item is not null)
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
            _selectedItem = FindObjectOfType<SelectedItem>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            LeanTween.value(gameObject, c => _thisImage.color = c, Color.white, new Color(0.6f, 0.6f, 0.6f, 1), 0.1f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            LeanTween.value(gameObject, c => _thisImage.color = c, new Color(0.6f, 0.6f, 0.6f, 1), Color.white, 0.1f);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left when item != ItemInstance.Empty:
                    if (_selectedItem.CurrentItem() == ItemInstance.Empty)
                    {
                        _selectedItem.Select(item);
                        Item = ItemInstance.Empty;
                        inventory[thisIndex] = item;
                    }
                    else
                    {
                        SwapOrMerge();
                    }

                    break;
                case PointerEventData.InputButton.Right when item != ItemInstance.Empty:
                    if (_selectedItem.CurrentItem() == ItemInstance.Empty)
                    {
                        var selectedCount = item.count / 2;
                        _selectedItem.Select(new ItemInstance(item.item, selectedCount));
                        item.count -= selectedCount;
                        OnItemChanged();
                        inventory[thisIndex] = item;
                    }
                    else
                    {
                        SwapOrMerge();
                    }
                    break;
                default:
                    SwapOrMerge();
                    break;
            }
        }

        private void SwapOrMerge()
        {
            var sii = _selectedItem.Unselect();
            var si = sii.item;
            if (si == item.item)
            {
                // basically both have no items
                if (si is null)
                {
                    return;
                }
                
                var selectedCount = sii.count;
                if (selectedCount + item.count < si.maxStackCount)
                {
                    item.count += selectedCount;
                    inventory[thisIndex] = item;
                    OnItemChanged();
                    return;
                }
            }

            if (item != ItemInstance.Empty)
            {
                _selectedItem.Select(item);
            }
            Item = sii;
            inventory[thisIndex] = item;
        }
    }
}
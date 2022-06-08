using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Players;
using UnityEngine;

namespace UI
{
    public class PlayerInventoryUI : MonoBehaviour
    {
        public ItemSlot[] slots;
        public PlayerInventoryBehaviour playerInventory;

        public CanvasGroup inventory;
        
        
        private void Start()
        {
            playerInventory.InventoryChanged += UpdateSlots;
            inventory.alpha = 0;
            inventory.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            playerInventory.InventoryChanged -= UpdateSlots;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(ToggleInventory());
            }
        }

        private IEnumerator ToggleInventory()
        {
            var active = !inventory.gameObject.activeSelf;
            if (active)
            {
                inventory.gameObject.SetActive(true);
                LeanTween.value(gameObject, f => inventory.alpha = f, 0f, 1f, 0.1f);
            }
            else
            {
                LeanTween.value(gameObject, f => inventory.alpha = f, 1f, 0f, 0.1f);
                yield return new WaitForSeconds(0.1f);
                inventory.gameObject.SetActive(false);
            }
        }
        
        private void UpdateSlots(int index, ItemInstance item)
        {
            slots[index].Item = item;
        }
    }
}
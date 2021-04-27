using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace LV
{
	public class EquipmentSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
        public EItemType allowedItemType;

        public ItemSlot equippedItem;

        private Color baseColor;
        private Color highlightedColor;

        public InventoryUI ui;

        public Image slotIcon;
        public Image overlay;

        public Rect rect;

        public void Init(InventoryUI ui)
        {
            this.ui = ui;

            rect = ui.RectTransformToScreenSpace(GetComponent<RectTransform>());

            GameObject ovTmp = Utilities.FindChildRecursive(transform, "Overlay");

            if(ovTmp)
            {
                overlay = ovTmp.GetComponent<Image>();

                baseColor = overlay.color;
                highlightedColor = overlay.color;
                highlightedColor.a = 1;
            }

            GameObject slotTmp = Utilities.FindChildRecursive(transform, "Icon");

            if(slotTmp)
            {
                slotIcon = slotTmp.GetComponent<Image>();
            }
        }

        public void EquipItem(ItemSlot item)
        {
            slotIcon.sprite = item.heldItem.inspectSprite;
            item.rect.sizeDelta = rect.size;
            item.ToggleSlot(false);
            ToggleSlotImage(true);

            OnEquip(item.heldItem);
        }

        public void UnequipItem()
        {
            if(equippedItem)
            {
                equippedItem.ToggleSlot(true);

                equippedItem.equipped = null;

                if (equippedItem.heldItem.onlyEquip)
                {
                    equippedItem.DropItem();
                }
                else
                {
                    equippedItem.transform.SetParent(ui.inventoryContent);

                    equippedItem.origin = ui.inventoryContent;

                    equippedItem.ToggleTexts(true);

                    equippedItem.OnRelease();
                }

                ToggleSlotImage(false);

                slotIcon.sprite = null;
                equippedItem = null;

                OnUnequip();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            overlay.color = highlightedColor;
            ui.selectedEqSlot = this;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            overlay.color = baseColor;

            if(ui.selectedEqSlot == this)
            {
                ui.selectedEqSlot = null;
            }
        }

        public void ToggleSlotImage(bool b)
        {
            Color newC = slotIcon.color;

            newC.a = b ? 1 : 0;

            slotIcon.color = newC;
        }

        public virtual void OnEquip(Item item)
        {

        }

        public virtual void OnUnequip()
        {

        }
    }
}
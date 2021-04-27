using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace LV
{
    public class ItemSlot : InventorySlot
    {
        protected PlayerInventory inv;
        public CanvasGroup display;

        public Item heldItem;
        public int amount;

        public EquipmentSlot equipped;

        public void Init(Item it, int amount, PlayerInventory inv)
        {
            this.inv = inv;

            display = GetComponent<CanvasGroup>();
            rect = GetComponent<RectTransform>();
            ui = GetComponentInParent<InventoryUI>();

            origin = transform.parent;

            icon.sprite = it.thumbnailSprite;

            heldItem = it;
            this.amount = amount;

            nameTxt.text = it.displayName;

            if (it.stackable)
            {
                amountTxt.text = amount.ToString();
            }
            else
            {
                amountTxt.text = "";
            }
        }

        public void ToggleSlot(bool b)
        {
            display.alpha = b ? 1 : 0;
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == 0)
            {
                Vector3 mPos = Input.mousePosition;
                mPos.y = -(mPos.y - Screen.height);

                if (equipped)
                {
                    if (ui.RectTransformToScreenSpace(ui.inventoryRect).Contains(mPos))
                    {
                        if (!heldItem.onlyEquip)
                        {
                            equipped.UnequipItem();
                        }
                    }
                }

                if (ui.RectTransformToScreenSpace(ui.vicinityRect).Contains(mPos))
                {
                    if(equipped)
                    {
                        equipped.UnequipItem();
                    }

                    inv.DropItem(this);
                }
                else
                {
                    if(ui.selectedEqSlot)
                    {
                        EquipmentSlot slot = ui.selectedEqSlot;
                        
                        if(equipped && slot == equipped)
                        {
                            Release();
                            return;
                        }

                        ui.EquipIfSlotValid(this, slot);
                    }
                }

                Release();
            }
        }

        public void DropItem()
        {
            inv.DropItem(this);
        }

        protected override void OnPick()
        {
            base.OnPick();

            ToggleSlot(true);
        }

        public override void OnRelease()
        {
            base.OnRelease();

            if(equipped)
            {
                ToggleTexts(false);
                ToggleSlot(false);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace LV
{
	public class VicinitySlot : InventorySlot 
	{
        public ItemEntity entity;

        public void Init(ItemEntity ent)
        {
            entity = ent;

            rect = GetComponent<RectTransform>();
            ui = GetComponentInParent<InventoryUI>();

            icon.sprite = ent.item.thumbnailSprite;

            origin = transform.parent;
            
            nameTxt.text = ent.item.displayName;

            if (ent.item.stackable)
            {
                amountTxt.text = ent.amount.ToString();
            }
            else
            {
                amountTxt.text = "";
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button == 0)
            {
                Vector3 mPos = Input.mousePosition;

                if (ui.RectTransformToScreenSpace(ui.inventoryRect).Contains(mPos))
                {
                    if (!entity.item.onlyEquip)
                    {
                        GameManager.instance.localPlayer.itManager.RequestPickUpItem(entity.identity);
                        Destroy(gameObject);
                    }
                    else
                    {
                        Release();
                    }
                }
                else if(ui.selectedEqSlot)
                {
                    if (ui.CheckIfSlotValid(entity.item, ui.selectedEqSlot))
                    {
                        if(ui.selectedEqSlot.equippedItem)
                        {
                            ui.selectedEqSlot.UnequipItem();
                        }

                        GameManager.instance.localPlayer.itManager.RequestPickUpItem(entity.identity, ui.selectedEqSlot);

                        Destroy(gameObject);
                    }
                    else
                    {
                        Release();
                    }
                }
                else
                {
                    Release();
                }
            }
        }
    }
}
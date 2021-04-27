using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class InventoryUI : MonoBehaviour 
	{
        private InGameGUI gui;

        public CanvasGroup display;

        public RectTransform inventoryRect;
        public RectTransform vicinityRect;

        public Transform inventoryContent;
        public Transform vicinityContent;
        
        public GameObject itemSlotPrefab;
        public GameObject vicinitySlotPrefab;

        bool isInit = false;

        public InventorySlot selectedSlot;

        public List<EquipmentSlot> Equipment;
        public EquipmentSlot selectedEqSlot;

        public bool isOpen = false;

        public void Init(InGameGUI g)
        {
            gui = g;

            display = GetComponent<CanvasGroup>();

            ToggleInventory(isOpen);

            Equipment = new List<EquipmentSlot>(GetComponentsInChildren<EquipmentSlot>());

            for(int i = 0; i < Equipment.Count; i++)
            {
                Equipment[i].Init(this);
            }

            isInit = true;
        }

        void Update()
        {
            if(selectedSlot)
            {
                selectedSlot.transform.position = Input.mousePosition + new Vector3(16, -16, 0);
            }
        }

        public void PickItem(InventorySlot slot)
        {
            selectedSlot = slot;
        }

        public void CloseInventory()
        {
            ToggleInventory(false);
        }

        public bool ToggleInventory()
        {
            return ToggleInventory(!isOpen);
        }

        public bool ToggleInventory(bool b)
        {
            display.alpha = b ? 1 : 0;
            display.blocksRaycasts = b;
            isOpen = b;

            if(isOpen)
            {
                gui.OnOpenMenus();
            }
            else
            {
                gui.OnCloseMenus();
            }

            return isOpen;
        }

        public Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
            rect.x -= (transform.pivot.x * size.x);
            rect.y -= ((1.0f - transform.pivot.y) * size.y);
            return rect;
        }

        public bool CheckIfSlotValid(Item item, EquipmentSlot slot)
        {
            if (slot.allowedItemType == item.itemType)
            {
                if (slot.allowedItemType == EItemType.WEAPON)
                {
                    WeaponSlot wSlot = (WeaponSlot)slot;
                    Weapon weapon = (Weapon)item;

                    int mask = (int)wSlot.allowedWeapons;
                    int val = (int)weapon.equipSlot;

                    if ((mask | (1 << val)) == mask)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public void EquipIfSlotValid(ItemSlot item, EquipmentSlot slot)
        {
            Item heldItem = item.heldItem;

            bool b = CheckIfSlotValid(heldItem, slot);

            if(b)
            {
                if(slot.equippedItem)
                {
                    slot.UnequipItem();
                }

                if (item.equipped)
                {
                    item.equipped.equippedItem = null;
                    item.equipped.ToggleSlotImage(false);
                }

                slot.equippedItem = item;

                item.origin = slot.transform;

                item.OnRelease();

                slot.EquipItem(item);
                item.equipped = slot;
            }
        }

        public EquipmentSlot FindSlotForItem(Item it)
        {
            List<EquipmentSlot> FoundSlots = new List<EquipmentSlot>();

            bool isWeapon = (it.itemType == EItemType.WEAPON);

            for(int i = 0; i < Equipment.Count; i++)
            {
                bool valid = CheckIfSlotValid(it, Equipment[i]);

                if(valid)
                {
                    if (isWeapon)
                    {
                        if (!Equipment[i].equippedItem)
                        {
                            return Equipment[i];
                        }
                        else
                        {
                            FoundSlots.Add(Equipment[i]);
                        }
                    }
                    else
                    {
                        return Equipment[i];
                    }
                }
            }

            if(FoundSlots.Count > 0)
            {
                for(int i = 0; i < FoundSlots.Count; i++)
                {
                    WeaponSlot slot = (WeaponSlot)FoundSlots[i];
                    if (GameManager.instance.localPlayer.weapons.currentSlot.id == slot.associatedSlotId)
                    {
                        return slot;
                    }
                }

                return FoundSlots[0];
            }

            return null;
        }
    }
}
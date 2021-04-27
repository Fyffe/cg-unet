using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class PlayerInventory : MonoBehaviour 
	{
        public PlayerRoot root;

        public ItemsDatabase db;

        public List<ItemSlot> Items = new List<ItemSlot>();
        public List<EquipmentSlot> Equipment;

        public InventoryUI ui;

        bool isInit;

        public void Init(PlayerRoot r)
        {
            root = r;
            db = ItemsDatabase.instance;

            ui = root.inGameGUI.inventoryUI;

            AddItem(0, 1);
            AddItem(1, 62);
            AddItem(1, 29);

            root.meta.onDeath += OnDeath;

            Equipment = new List<EquipmentSlot>(ui.GetComponentsInChildren<EquipmentSlot>());

            isInit = true;
        }

        void OnDisable()
        {
            if (!isInit)
            {
                return;
            }

            root.meta.onDeath -= OnDeath;
        }

        public void OnDeath()
        {
            for (int i = 0; i < Equipment.Count; i++)
            {
                Equipment[i].UnequipItem();
            }

            for (int i = 0; i < Items.Count; i++)
            {
                DropItem(Items[i]);
            }

            enabled = false;
        }

        public ItemSlot AddItem(int id, int amount)
        {
            Item it = db.FindItem(id);

            if (it == null)
            {
                return null;
            }

            return AddItem(it, amount);
        }

        public ItemSlot AddItem(Item it, int amount)
        {
            if (it == null)
            {
                return null;
            }

            ItemSlot retSlot = null;

            if(amount <= 0)
            {
                amount = 1;
            }

            if (it.onlyEquip)
            {
                root.itManager.RequestItemSpawn(it.id, amount, transform.position + Vector3.up);
            }
            else
            {
                if (it.stackable)
                {
                    if (amount > it.maxAmount)
                    {
                        amount = it.maxAmount;
                    }

                    ItemSlot slot = Instantiate(ui.itemSlotPrefab, ui.inventoryContent).GetComponent<ItemSlot>();
                    slot.Init(it, amount, this);

                    Items.Add(slot);

                    retSlot = slot;
                }
                else
                {
                    for (int i = 0; i < amount; i++)
                    {
                        ItemSlot slot = Instantiate(ui.itemSlotPrefab, ui.inventoryContent).GetComponent<ItemSlot>();
                        slot.Init(it, 1, this);

                        Items.Add(slot);

                        retSlot = slot;
                    }
                }
            }

            if (it.itemType == EItemType.AMMO)
            {
                root.weapons.UpdateAmmo();
            }

            return retSlot;
        }

        public void DropItem(ItemSlot slot)
        {
            if(Items.Contains(slot))
            {
                ItemSlot temp = slot;

                Items.Remove(slot);
                Destroy(slot.gameObject);

                root.itManager.RequestItemSpawn(temp.heldItem.id, temp.amount, transform.position + Vector3.up);

                if (temp.heldItem.itemType == EItemType.AMMO)
                {
                    root.weapons.UpdateAmmo();
                }
            }
            else
            {
                slot.Release();
            }
        }

        public void PickUpItem(ItemEntity ent)
        {
            if(ent && ent.item)
            {
                bool picked = false;
                int amt = ent.amount;
                Item it = ent.item;

                if (it.onlyEquip)
                {
                    EquipmentSlot slot = ui.FindSlotForItem(it);

                    if(slot)
                    {
                        PickUpItemAndEquip(ent, slot);
                        picked = true;
                    }
                }
                else
                {
                    AddItem(it, amt);
                    picked = true;
                }

                if(picked)
                {
                    ent.OnPickUp();
                }
            }
        }

        public void PickUpItemAndEquip(ItemEntity ent, EquipmentSlot slot)
        {
            if (ent && ent.item)
            {
                int amt = ent.amount;
                Item it = ent.item;

                ent.OnPickUp();

                ItemSlot itSlot = Instantiate(ui.itemSlotPrefab, ui.inventoryContent).GetComponent<ItemSlot>();
                itSlot.Init(it, amt, this);

                Items.Add(itSlot);

                ui.EquipIfSlotValid(itSlot, slot);
            }
        }

        public void OnEquipItem(EquipmentSlot slot)
        {
            Equipment.Add(slot);
        }

        public int GetItemCount(int id)
        {
            int count = 0;

            for(int i = 0; i < Items.Count; i++)
            {
                Item it = Items[i].heldItem;

                if(it.id == id)
                {
                    count += Items[i].amount;
                }
            }

            return count;
        }

        public void RemoveItemWithId(int id, int amount)
        {
            int amtLeft = amount;

            List<int> IdsToRemove = new List<int>();

            for (int i = 0; i < Items.Count; i++)
            {
                if (amtLeft > 0)
                {
                    ItemSlot slot = Items[i];
                    Item it = Items[i].heldItem;

                    if (it.id == id)
                    {
                        if (it.stackable)
                        {
                            if (slot.amount <= amtLeft)
                            {
                                amtLeft -= slot.amount;

                                Destroy(slot.gameObject);
                                IdsToRemove.Add(i);
                            }
                            else
                            {
                                slot.amount -= amtLeft;
                                slot.amountTxt.text = slot.amount.ToString();
                                amtLeft = 0;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            for(int i = 0; i < IdsToRemove.Count; i++)
            {
                Items.RemoveAt(IdsToRemove[i]);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class WeaponSlot : EquipmentSlot 
	{
        public EEquipSlot allowedWeapons;

        public int associatedSlotId;

        public override void OnEquip(Item item)
        {
            base.OnEquip(item);

            Weapon weapon = (Weapon)equippedItem.heldItem;

            GameManager.instance.localPlayer.weapons.EquipWeapon(associatedSlotId, weapon);
        }

        public override void OnUnequip()
        {
            base.OnUnequip();

            GameManager.instance.localPlayer.weapons.UnequipWeapon(associatedSlotId);
        }
    }
}
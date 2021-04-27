using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class CharacterSlot : EquipmentSlot 
	{
        public override void OnEquip(Item item)
        {
            base.OnEquip(item);

            GameManager.instance.localPlayer.itManager.RequestEquipItem(item.id);
        }
    }
}
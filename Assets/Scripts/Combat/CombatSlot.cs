using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [System.Serializable]
	public class CombatSlot
	{
        public int id;

        public WeaponEntity weapon;
        public WeaponHolder holder;

        public void Init(int i)
        {
            id = i;
        }

        public void SetWeapon(WeaponEntity ent)
        {
            weapon = ent;
        }

        public void UnsetWeapon()
        {
            weapon = null;
        }
	}
}
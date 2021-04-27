using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [CreateAssetMenu(menuName = "Items/Weapons/Base")]
	public class Weapon : Item
	{
        public EWeaponType weaponType;
        public EEquipSlot equipSlot;

        public int damage;
        public int range;

        public Vector3 vmEquipPos;
        public Vector3 vmEquipRot;

        public Vector3 vmAimPos;
        public Vector3 vmAimRot;

        public Vector3 rmEquipPos;
        public Vector3 rmEquipRot;

        public Vector3 rmAimPos;
        public Vector3 rmAimRot;

        public EEquipBones equipBone;
	}
}
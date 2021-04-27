using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [CreateAssetMenu(menuName = "Items/Weapons/Gun")]
	public class Gun : Weapon 
	{
        public float fireRate;
        public Ammunition ammoType;

        public float bulletSpeed;

        public int maxAmmoPerMag;

        public float aimFOV = 45f;

        public float recoilXMin;
        public float recoilXMax;
        public float recoilYMin;
        public float recoilYMax;
	}
}
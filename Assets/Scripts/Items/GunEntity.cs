using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class GunEntity : WeaponEntity
	{
        public int loadedAmmo;

        public Gun gun;

        public ParticleSystem[] shootEffects;

        public override void Init(Item it, int amt, int id)
        {
            base.Init(it, amt, id);

            gun = (Gun)weapon;
        }

        public void ShootEffects()
        {
            for(int i = 0; i < shootEffects.Length; i++)
            {
                shootEffects[i].Play();
            }
        }
    }
}
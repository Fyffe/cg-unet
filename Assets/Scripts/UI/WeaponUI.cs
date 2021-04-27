using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class WeaponUI : MonoBehaviour 
	{
        private PlayerInventory inv;

        public Text nameTxt;
        public Image circleFill;

        public WeaponEntity equippedWeapon;

        public GameObject gunDisplay;
        public GameObject meleeDisplay;

        public float maxAmmo;

        public Text ammoCurrentTxt;
        public Text ammoInventoryTxt;

        private bool isInit = false;

        public void Init(PlayerInventory inv)
        {
            this.inv = inv;

            isInit = true;
        }

        public void SetWeapon(WeaponEntity wep)
        {
            if(!isInit)
            {
                return;
            }

            circleFill.fillAmount = 1;
            gunDisplay.SetActive(false);
            meleeDisplay.SetActive(false);

            if (wep)
            {
                equippedWeapon = wep;

                nameTxt.text = wep.weapon.displayName;

                if (wep.weapon.weaponType == EWeaponType.GUN)
                {
                    gunDisplay.SetActive(true);
                }

                if(wep.weapon.weaponType == EWeaponType.MELEE)
                {
                    meleeDisplay.SetActive(true);
                }
            }
            else
            {
                meleeDisplay.SetActive(true);
                nameTxt.text = "Fists";
            }
        }

        public void SetAmmoCurrent(int count)
        {
            ammoCurrentTxt.text = count.ToString();
            circleFill.fillAmount = (float)count / maxAmmo;
        }

        public void SetAmmoInInventory(int count)
        {
            ammoInventoryTxt.text = count.ToString();
        }
	}
}
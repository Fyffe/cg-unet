using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class WeaponsController : MonoBehaviour 
	{
        public PlayerRoot root;

        public WeaponEntity currentWeapon;
        private WeaponEntity nextWeapon;

        private GunEntity currentGun;

        private WeaponUI wepDisplay;

        public List<CombatSlot> Slots = new List<CombatSlot>();
        public CombatSlot currentSlot;

        public Transform weaponHolder;

        public PlayerStates states;
        public PlayerCombatStates combatStates;
        public WeaponHolders holders;

        private float baseFOV;
        private float startFOV;
        private float targetFOV;

        private bool isInit = false;

        private Vector3 startPos;
        private Vector3 startRot;

        private Vector3 targetPos;
        private Vector3 targetRot;

        private float lerpTime;

        public float aimTime = .175f;
        public float holsterTime = .3f;
        public float timeSinceStart;

        private float lastAttackTime = 0f;

        public Vector3 holsterPosOffset = new Vector3(0, 0.25f, 0);
        public Vector3 holsterRotOffset = new Vector3(30, 0, 0);

        public Vector3 movementOffset;
        public Vector3 recoilOffset;

        private int currentAmmoId = -1;
        private int ammoInInventory = 0;

        private Vector3 weaponOffset;

        public void Init(PlayerRoot r)
        {
            root = r;
            states = root.states;

            holders = GetComponent<WeaponHolders>();
            holders.Init(root);

            wepDisplay = root.inGameGUI.weaponUI;
            wepDisplay.Init(root.inventory);

            baseFOV = root.cam.fieldOfView;
            startFOV = baseFOV;
            targetFOV = baseFOV;

            for(int i = 0; i < Slots.Count; i++)
            {
                for(int j = 0; j < holders.Holders.Count; j++)
                {
                    if(Slots[i].id == holders.Holders[j].id)
                    {
                        Slots[i].holder = holders.Holders[j];
                        continue;
                    }
                }
            }

            isInit = true;
        }

        void Update()
        {
            if(!isInit)
            {
                return;
            }

            HandleInputs();
            HandleSlots();
            HandleWeaponOffsets();

            if (currentWeapon)
            {
                switch (currentWeapon.weapon.weaponType)
                {
                    case EWeaponType.MELEE:
                        HandleMelee();
                        break;
                    case EWeaponType.GUN:
                        HandleGuns();
                        break;
                }
            }
        }

        void HandleInputs()
        {
            combatStates.canReload = currentGun && !states.inMenus && !combatStates.isEquipping && !combatStates.isHolstering && !combatStates.isReloading;
            combatStates.canAim = currentGun && !states.inMenus && !combatStates.isReloading && !combatStates.isEquipping && !combatStates.isHolstering && states.isGrounded;
            combatStates.canAttack = currentWeapon && !states.isSprinting && !combatStates.isReloading && !states.inMenus && !combatStates.isEquipping && !combatStates.isHolstering;

            bool aiming = Input.GetButton("Fire2");

            if(aiming && combatStates.canAim && !combatStates.isAiming)
            {
                combatStates.isAiming = true;
                combatStates.aimStartTime = Time.time;

                startPos = currentWeapon.weapon.vmEquipPos;
                startRot = currentWeapon.weapon.vmEquipRot;

                targetPos = currentWeapon.weapon.vmAimPos;
                targetRot = currentWeapon.weapon.vmAimRot;

                timeSinceStart = combatStates.aimStartTime;

                if (currentGun)
                {
                    startFOV = root.cam.fieldOfView;
                    targetFOV = currentGun.gun.aimFOV;
                }

                lerpTime = aimTime;
            }

            if(combatStates.isAiming && (!aiming || !combatStates.canAim))
            {
                combatStates.isAiming = false;
                combatStates.aimStopTime = Time.time;

                startPos = currentWeapon.weapon.vmAimPos;
                startRot = currentWeapon.weapon.vmAimRot;

                targetPos = currentWeapon.weapon.vmEquipPos;
                targetRot = currentWeapon.weapon.vmEquipRot;

                timeSinceStart = combatStates.aimStopTime;

                if (currentGun)
                {
                    startFOV = root.cam.fieldOfView;
                    targetFOV = baseFOV;
                }

                lerpTime = aimTime;
            }

            if(combatStates.canAttack && Input.GetButton("Fire1"))
            {
                if(currentGun)
                {
                    if(currentGun.loadedAmmo > 0 && Time.time - lastAttackTime > (60 / currentGun.gun.fireRate))
                    {
                        Attack();
                    }
                }
            }

            if(combatStates.canReload && Input.GetButtonDown("Reload"))
            {
                if(currentGun && currentGun.loadedAmmo < currentGun.gun.maxAmmoPerMag && ammoInInventory > 0)
                {
                    Reload();
                }
            }
        }

        void HandleSlots()
        {
            string input = Input.inputString;
            int key = -1;

            if (int.TryParse(input, out key))
            {
                if(key > 0 && key <= Slots.Count)
                {
                    SelectSlot(key - 1);
                }
            }
        }

        public void SelectSlot(int id)
        {
            if(currentSlot == Slots[id])
            {
                return;
            }

            if (Slots[id].weapon)
            {
                currentSlot = Slots[id];

                SwitchWeapon(id);
            }
        }

        public void SwitchWeapon(int id)
        {
            if(currentWeapon)
            {
                nextWeapon = currentSlot.weapon;
                HolsterWeapon();
            }
            else
            {
                currentWeapon = currentSlot.weapon;

                if(currentWeapon.weapon.weaponType == EWeaponType.GUN)
                {
                    currentGun = (GunEntity)currentWeapon;
                }

                UnholsterWeapon();
            }
        }

        void HandleWeaponOffsets()
        {
            float multiplier = 5;

            float y = 0;

            if(Mathf.Abs(root.controller.rb.velocity.y) > 0.5f)
            {
                y = -root.controller.rb.velocity.y;

                y = Mathf.Clamp(y, -.75f, .75f) * 0.05f;
            }

            movementOffset = new Vector3(-root.controller.horizontal, 0, -root.controller.vertical) * 0.05f;

            if(states.isSprinting)
            {
                movementOffset *= 2f;
            }

            if(combatStates.isAiming)
            {
                movementOffset *= 0.1f;
            }

            movementOffset.y = y;

            weaponOffset = movementOffset + recoilOffset;

            weaponHolder.localPosition = Vector3.Lerp(weaponHolder.localPosition, weaponOffset, Time.deltaTime * multiplier);
        }

        void HandleMelee()
        {
        }

        void HandleGuns()
        {
            float perc = (Time.time - timeSinceStart) / lerpTime;

            currentWeapon.transform.localPosition = Vector3.Lerp(startPos, targetPos, perc);
            currentWeapon.transform.localEulerAngles = Vector3.Lerp(startRot, targetRot, perc);
            root.cam.fieldOfView = Mathf.Lerp(startFOV, targetFOV, perc);

            if (combatStates.isHolstering)
            {
                if (perc >= 1)
                {
                    combatStates.isHolstering = false;
                    currentWeapon.gameObject.SetActive(false);

                    OnHolster();

                    if (nextWeapon)
                    {
                        currentWeapon = nextWeapon;

                        if(currentWeapon.weapon.weaponType == EWeaponType.GUN)
                        {
                            currentGun = (GunEntity)currentWeapon;
                        }

                        UnholsterWeapon();
                    }
                }
            }
            else if (combatStates.isEquipping)
            {
                if (perc >= 1)
                {
                    combatStates.isEquipping = false;
                    nextWeapon = null;

                    OnUnholster();
                }
            }
        }

        void Reload()
        {
            currentWeapon.anim.SetTrigger("Reload");
            combatStates.isReloading = true;
        }

        public void OnReload()
        {
            if (currentGun)
            {
                int requiredAmmo = currentGun.gun.maxAmmoPerMag - currentGun.loadedAmmo;

                if (ammoInInventory >= requiredAmmo)
                {
                    root.inventory.RemoveItemWithId(currentAmmoId, requiredAmmo);

                    currentGun.loadedAmmo += requiredAmmo;
                    ammoInInventory -= requiredAmmo;
                }
                else
                {
                    root.inventory.RemoveItemWithId(currentAmmoId, ammoInInventory);

                    currentGun.loadedAmmo += ammoInInventory;
                    ammoInInventory = 0;
                }

                wepDisplay.SetAmmoCurrent(currentGun.loadedAmmo);
                wepDisplay.SetAmmoInInventory(ammoInInventory);
            }

            combatStates.isReloading = false;
        }

        public void UpdateAmmo()
        {
            ammoInInventory = root.inventory.GetItemCount(currentAmmoId);

            wepDisplay.SetAmmoInInventory(ammoInInventory);
        }

        void Attack()
        {
            if(!currentWeapon)
            {
                return;
            }

            lastAttackTime = Time.time;

            if (currentGun)
            {
                float recMulti = 1;
                
                if(combatStates.isAiming)
                {
                    recMulti = 0.35f;
                }

                if(states.isMoving)
                {
                    recMulti += 0.25f;
                }

                float recX = Random.Range(currentGun.gun.recoilXMin, currentGun.gun.recoilXMax) * recMulti;
                float recY = Random.Range(currentGun.gun.recoilYMin, currentGun.gun.recoilYMax) * recMulti;

                root.controller.AddRecoil(recX, recY);

                currentWeapon.anim.SetTrigger("Fire");

                currentGun.loadedAmmo--;
                currentGun.ShootEffects();
                wepDisplay.SetAmmoCurrent(currentGun.loadedAmmo);

                GameObject bulletPrefab = currentGun.gun.ammoType.bulletPrefab;

                Projectile bullet = Instantiate(bulletPrefab, root.cam.transform.position, root.cam.transform.rotation).GetComponent<Projectile>();
                bullet.Init(currentGun.gun.bulletSpeed, currentGun.gun.range, currentGun.gun.damage, "Player", currentGun.gun.displayName);
            }
        }

        void UnholsterWeapon()
        {
            currentWeapon.gameObject.SetActive(true);

            root.viewmodelIK.EquipWeapon(currentWeapon);

            combatStates.isEquipping = true;

            startPos = currentWeapon.weapon.vmEquipPos - holsterPosOffset;
            startRot = currentWeapon.weapon.vmEquipRot + holsterRotOffset;

            currentWeapon.transform.localPosition = startPos;
            currentWeapon.transform.localEulerAngles = startRot;

            targetPos = currentWeapon.weapon.vmEquipPos;
            targetRot = currentWeapon.weapon.vmEquipRot;

            lerpTime = holsterTime;
            
            timeSinceStart = Time.time;
        }

        void OnUnholster()
        {
            wepDisplay.SetWeapon(currentWeapon);

            if (currentWeapon.weapon.weaponType == EWeaponType.GUN)
            {
                currentGun = (GunEntity)currentWeapon;

                currentAmmoId = currentGun.gun.ammoType.id;
                wepDisplay.maxAmmo = currentGun.gun.maxAmmoPerMag;

                ammoInInventory = root.inventory.GetItemCount(currentAmmoId);
                
                wepDisplay.SetAmmoCurrent(currentGun.loadedAmmo);
                wepDisplay.SetAmmoInInventory(ammoInInventory);

                for(int i = 0; i < currentSlot.holder.holder.childCount; i++)
                {
                    ItemEntity ent = currentSlot.holder.holder.GetChild(i).GetComponent<ItemEntity>();

                    if(ent)
                    {
                        root.itManager.RequestUnholsterWeapon(ent.identity);
                    }
                }
            }
        }

        void HolsterWeapon()
        {
            if(currentWeapon)
            {
                combatStates.isHolstering = true;

                startPos = currentWeapon.weapon.vmEquipPos;
                startRot = currentWeapon.weapon.vmEquipRot;

                targetPos = currentWeapon.weapon.vmEquipPos - holsterPosOffset;
                targetRot = currentWeapon.weapon.vmEquipRot + holsterRotOffset;

                lerpTime = holsterTime;

                timeSinceStart = Time.time;

                currentGun = null;
            }
        }

        void OnHolster()
        {
            currentAmmoId = -1;
            ammoInInventory = 0;

            wepDisplay.SetWeapon(null);
        }

        public void EquipWeapon(int slot, Weapon weapon)
        {
            if (slot >= 0 && slot < Slots.Count)
            {
                if (weapon)
                {
                    WeaponEntity ent = Instantiate(weapon.prefab, weaponHolder).GetComponent<WeaponEntity>();

                    ent.Init(weapon, 0, -1);

                    Slots[slot].SetWeapon(ent);

                    Utilities.SetLayerRecursive(ent.gameObject, 11);
                    
                    root.itManager.RequestEquipWeapon(weapon.id, slot);

                    if(Slots[slot] != currentSlot)
                    {
                        ent.gameObject.SetActive(false);
                    }
                    else
                    {
                        SwitchWeapon(currentSlot.id);
                    }
                }
            }
        }

        public void UnequipWeapon(int slot)
        {
            if(Slots[slot].weapon)
            {
                if(Slots[slot].weapon.weapon.weaponType == EWeaponType.GUN)
                {
                    GunEntity ent = (GunEntity)Slots[slot].weapon;

                    int id = ent.gun.ammoType.id;
                    int amt = ent.loadedAmmo;

                    if(amt > 0)
                    {
                        root.inventory.AddItem(id, amt);
                    }
                }

                if(Slots[slot] == currentSlot)
                {
                    currentWeapon = null;
                    currentGun = null;
                    currentSlot = null;

                    root.viewmodelIK.UnequipWeapon();

                    wepDisplay.SetWeapon(null);
                }

                Destroy(Slots[slot].weapon.gameObject);
            }
        }
    }
}
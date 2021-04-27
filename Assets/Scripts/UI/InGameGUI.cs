using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class InGameGUI : MonoBehaviour 
	{
        public InGameMenu menuUI;
        public InventoryUI inventoryUI;
        public PickerUI picker;
        public WeaponUI weaponUI;
        public MetabolismUI metaUI;

        public delegate void OnOpen();
        public event OnOpen onOpen;

        public delegate void OnClose();
        public event OnClose onClose;

        void Awake()
        {
            menuUI = GetComponentInChildren<InGameMenu>();
            menuUI.Init(this);

            inventoryUI = GetComponentInChildren<InventoryUI>();
            inventoryUI.Init(this);

            picker = GetComponentInChildren<PickerUI>();

            weaponUI = GetComponentInChildren<WeaponUI>();

            metaUI = GetComponentInChildren<MetabolismUI>();
        }

        void Update()
        {
            if(!menuUI.isOpen && Input.GetButtonDown("Inventory"))
            {
                inventoryUI.ToggleInventory();
            }

            if(!inventoryUI.isOpen && Input.GetButtonDown("In Game Menu"))
            {
                menuUI.ToggleDisplay(!menuUI.isOpen);
            }

            if(inventoryUI.isOpen && Input.GetButtonDown("Cancel"))
            {
                inventoryUI.CloseInventory();
            }
        }

        public void OnOpenMenus()
        {
            if (onOpen != null)
            {
                onOpen();
            }
        }

        public void OnCloseMenus()
        {
            if (onClose != null)
            {
                onClose();
            }
        }
	}
}
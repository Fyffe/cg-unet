using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
	public class PlayerRoot : NetworkBehaviour 
	{
        public NetworkIdentity identity;
        public PlayerController controller;
        public PlayerInventory inventory;
        public PlayerMetabolism meta;
        public EquipmentController eqController;
        public InGameGUI inGameGUI;
        public InventoryUI inventoryUI;
        public WeaponsController weapons;
        public Transform camHolder;
        public Camera cam;
        public ItemsManager itManager;
        public PlayerStates states;
        public VicinityController vicinity;
        public PlayerRagdoll ragdoll;

        public PlayerModelIK modelIK;
        public PlayerViewmodelIK viewmodelIK;

        public Animator remoteAnim;

        public PlayerMovementSync moveSync;

        public GameObject viewmodel;
        public GameObject model;

        public bool offline;

        private bool isInit;

        void Start()
        {
            Init();
        }

        public void Init()
        {
            identity = GetComponent<NetworkIdentity>();
            itManager = GetComponent<ItemsManager>();

            itManager.Init(this);

            eqController = GetComponent<EquipmentController>();

            ragdoll = GetComponent<PlayerRagdoll>();
            modelIK = GetComponentInChildren<PlayerModelIK>();
            
            moveSync = GetComponent<PlayerMovementSync>();

            model = transform.Find("Model").gameObject;

            remoteAnim = model.GetComponent<Animator>();

            if (identity.isLocalPlayer || offline)
            {
                camHolder = transform.Find("Camera Holder");
                cam = camHolder.Find("Camera").GetComponent<Camera>();

                viewmodel = cam.transform.Find("Viewmodel").gameObject;

                LevelController.instance.ToggleLevelCamera(false);
                camHolder.gameObject.SetActive(true);

                controller = GetComponent<PlayerController>();
                inventory = GetComponent<PlayerInventory>();
                meta = GetComponent<PlayerMetabolism>();
                inGameGUI = GameManager.instance.inGameGUI;
                inventoryUI = inGameGUI.inventoryUI;
                weapons = GetComponent<WeaponsController>();
                states = controller.states;
                vicinity = GetComponentInChildren<VicinityController>(true);
                viewmodelIK = GetComponentInChildren<PlayerViewmodelIK>(true);

                controller.Init(this);
                weapons.Init(this);
                inventory.Init(this);
                meta.Init(this);
                vicinity.Init(this);
                viewmodelIK.Init(this);

                meta.onDeath += OnDeath;

                controller.rb.isKinematic = false;

                GameManager.instance.localPlayer = this;
            }
            else
            {
                Utilities.SetLayerRecursive(model, 10);
            }

            eqController.Init(this);
            ragdoll.Init(this);
            modelIK.Init(this);
            moveSync.Init(this);

            isInit = true;
        }

        public void OnDeath()
        {
            camHolder.transform.SetParent(model.GetComponent<Animator>().GetBoneTransform(HumanBodyBones.Head));
            camHolder.transform.localPosition = new Vector3(-0.1f, 0, 0);

            for(int i = 0; i < cam.transform.childCount; i++)
            {
                Destroy(cam.transform.GetChild(i).gameObject);
            }

            Destroy(modelIK);

            model.transform.SetParent(null);
            model.name = "Corpse";
        }
	}
}
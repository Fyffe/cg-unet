using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
	public class ItemsManager : NetworkBehaviour 
	{
        private PlayerRoot root;
        private ItemsDatabase itemDb;

        public List<ItemEntity> SpawnedItems = new List<ItemEntity>();

        private int lastId = -1;

        private bool isInit = false;

        private EquipmentSlot requestedSlot;

        public void Init(PlayerRoot r)
        {
            root = r;
            itemDb = ItemsDatabase.instance;

            isInit = true;
        }

        public void RequestItemSpawn(string slug, int amount, Vector3 pos)
        {
            if(!isInit)
            {
                return;
            }

            CmdTrySpawnItemSlug(slug, amount, pos);
        }

        public void RequestItemSpawn(int id, int amount, Vector3 pos)
        {
            if (!isInit)
            {
                return;
            }

            CmdTrySpawnItemId(id, amount, pos);
        }

        [Command]
        void CmdTrySpawnItemSlug(string slug, int amount, Vector3 pos)
        {
            if (slug.Length <= 0)
            {
                return;
            }

            Item it = itemDb.FindItem(slug);

            if (!it)
            {
                return;
            }

            lastId++;

            RpcSpawnItem(it.id, amount, lastId, pos);
        }

        [Command]
        void CmdTrySpawnItemId(int id, int amount, Vector3 pos)
        {
            if(id < 0)
            {
                return;
            }

            Item it = itemDb.FindItem(id);

            if (!it)
            {
                return;
            }

            lastId++;

            RpcSpawnItem(it.id, amount, lastId, pos);
        }

        [ClientRpc]
        void RpcSpawnItem(int id, int amount, int index, Vector3 pos)
        {
            Item it = ItemsDatabase.instance.FindItem(id);

            if (!it)
            {
                Debug.LogWarning("Couldn't find and spawn item with ID of " + id);
                return;
            }

            ItemEntity spawned = Instantiate(it.prefab, pos, Quaternion.identity).GetComponent<ItemEntity>();

            spawned.Init(it, amount, index);

            spawned.rb.isKinematic = false;
            spawned.col.isTrigger = false;

            SpawnedItems.Add(spawned);
        }

        public void RequestPickUpItem(int id, EquipmentSlot slot = null)
        {
            if(id < 0)
            {
                return;
            }

            if(slot)
            {
                requestedSlot = slot;
            }

            CmdTryPickUpItem(id);
        }

        [Command]
        void CmdTryPickUpItem(int id)
        {
            if(id < 0)
            {
                return;
            }

            ItemEntity ent = FindItemEntity(id);

            if(ent)
            {
                TargetPickUpItem(connectionToClient, id);
            }
        }

        [TargetRpc]
        public void TargetPickUpItem(NetworkConnection conn, int id)
        {
            ItemEntity ent = FindItemEntity(id);

            if(ent)
            {
                if (requestedSlot)
                {
                    root.inventory.PickUpItemAndEquip(ent, requestedSlot);
                    requestedSlot = null;
                }
                else
                {
                    root.inventory.PickUpItem(ent);
                }

                CmdRemoveItem(id);
            }
        }

        [Command]
        public void CmdRemoveItem(int id)
        {
            ItemEntity ent = FindItemEntity(id);

            if(ent)
            {
                RpcRemoveItem(id);
            }
        }

        [ClientRpc]
        public void RpcRemoveItem(int id)
        {
            int i = FindItemEntityIndex(id);
            ItemEntity ent = SpawnedItems[i];

            if(ent)
            {
                Destroy(ent.gameObject);
                SpawnedItems.RemoveAt(i);
            }
        }

        public void RequestEquipItem(int id)
        {
            if (id < 0)
            {
                return;
            }

            CmdSpawnEquipItem(id);
        }

        public void RequestEquipWeapon(int id, int slotId)
        {
            if (id < 0)
            {
                return;
            }

            CmdSpawnEquipWeapon(id, slotId);
        }

        [Command]
        public void CmdSpawnEquipWeapon(int id, int slotId)
        {
            Item it = ItemsDatabase.instance.FindItem(id);

            if (!it)
            {
                Debug.LogWarning("Couldn't find and spawn item with ID of " + id);
                return;
            }

            lastId++;

            RpcSpawnEquipWeapon(id, slotId, lastId);
        }

        [ClientRpc]
        public void RpcSpawnEquipWeapon(int id, int slotId, int index)
        {
            Item it = ItemsDatabase.instance.FindItem(id);

            if (!it)
            {
                Debug.LogWarning("Couldn't find and spawn item with ID of " + id);
                return;
            }

            ItemEntity spawned = Instantiate(it.prefab).GetComponent<ItemEntity>();

            spawned.Init(it, 1, index);

            Transform parent = root.weapons.Slots[slotId].holder.holder;

            spawned.transform.SetParent(parent, true);
            spawned.transform.localPosition = Vector3.zero;
            spawned.transform.localEulerAngles = Vector3.zero;

            if (isLocalPlayer)
            {
                Utilities.SetLayerRecursive(spawned.gameObject, 9);
            }
            else
            {
                Utilities.SetLayerRecursive(spawned.gameObject, 10);
            }

            SpawnedItems.Add(spawned);
        }

        [Command]
        public void CmdSpawnEquipItem(int id)
        {
            Item it = ItemsDatabase.instance.FindItem(id);

            if(!it)
            {
                Debug.LogWarning("Couldn't find and spawn item with ID of " + id);
                return;
            }

            lastId++;

            RpcSpawnEquipItem(id, lastId);
        }

        [ClientRpc]
        public void RpcSpawnEquipItem(int id, int index)
        {
            Item it = ItemsDatabase.instance.FindItem(id);

            if (!it)
            {
                Debug.LogWarning("Couldn't find and spawn item with ID of " + id);
                return;
            }

            ItemEntity spawned = Instantiate(it.prefab).GetComponent<ItemEntity>();

            spawned.Init(it, 1, index);
            
            root.eqController.EquipItem(spawned);

            SpawnedItems.Add(spawned);
        }

        public void RequestUnholsterWeapon(int id)
        {
            if(id < 0 && id >= SpawnedItems.Count)
            {
                return;
            }

            ItemEntity ent = FindItemEntity(id);

            if(!ent || (ent && ent.item.itemType != EItemType.WEAPON))
            {
                return;
            }

            CmdUnholsterWeapon(id);
        }

        [Command]
        public void CmdUnholsterWeapon(int id)
        {
            RpcUnholsterWeapon(id);
        }

        [ClientRpc]
        public void RpcUnholsterWeapon(int id)
        {
            WeaponEntity ent = (WeaponEntity)FindItemEntity(id);

            if (!ent)
            {
                return;
            }

            Transform parent = null;

            switch (ent.weapon.equipBone)
            {
                case EEquipBones.LEFT_HAND:
                    parent = root.remoteAnim.GetBoneTransform(HumanBodyBones.LeftHand);
                    break;
                case EEquipBones.RIGHT_HAND:
                    parent = root.remoteAnim.GetBoneTransform(HumanBodyBones.RightHand);
                    break;
            }

            if (!parent)
            {
                return;
            }

            ent.transform.SetParent(parent, true);
            ent.transform.localPosition = ent.weapon.rmEquipPos;
            ent.transform.localEulerAngles = ent.weapon.rmEquipRot;
        }

        public ItemEntity FindItemEntity(int id)
        {
            ItemEntity ent = null;

            for (int i = 0; i < SpawnedItems.Count; i++)
            {
                ItemEntity it = SpawnedItems[i];

                if (it.identity == id)
                {
                    return it;
                }
            }

            return ent;
        }

        public int FindItemEntityIndex(int id)
        {
            int ent = -1;

            for (int i = 0; i < SpawnedItems.Count; i++)
            {
                ItemEntity it = SpawnedItems[i];

                if (it.identity == id)
                {
                    return i;
                }
            }

            return ent;
        }
    }
}
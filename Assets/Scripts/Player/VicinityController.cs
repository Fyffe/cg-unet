using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    public class VicinityController : MonoBehaviour
    {
        public PlayerRoot root;
        public InventoryUI inv;

        public LayerMask itemsMask;
        public LayerMask obstaclesMask;

        public List<ItemEntity> InVicinity = new List<ItemEntity>();

        public float range = 1f;

        bool isInit = false;

        private float cycle;

        public void Init(PlayerRoot r)
        {
            root = r;
            inv = root.inventoryUI;

            isInit = true;
        }

        public void DetectItemsInVicinity()
        {
            if (!isInit)
            {
                return;
            }

            Collider[] items = Physics.OverlapCapsule(root.transform.position, root.transform.position + Vector3.up * 1.8f, range, itemsMask);

            for (int i = 0; i < items.Length; i++)
            {
                ItemEntity ent = items[i].GetComponent<ItemEntity>();

                if (ent)
                {
                    AddItemToVicinity(ent);
                }
            }
        }

        public void ClearItems()
        {
            for(int i = 0; i < InVicinity.Count; i++)
            {
                RemoveFromVicinity(InVicinity[i]);
            }
        }

        public void AddItemToVicinity(ItemEntity ent)
        {
            if(ent)
            {
                if(InVicinity.Contains(ent))
                {
                    return;
                }

                bool blocked = CheckForObstacles(ent);

                if (!blocked)
                {
                    InVicinity.Add(ent);
                    AddItemToUI(ent);
                }
            }
        }

        bool CheckForObstacles(ItemEntity ent)
        {
            if (ent.transform.position.y < root.transform.position.y - 0.05f)
            {
                return true;
            }

            RaycastHit hit;

            Vector3 origin = root.transform.position + Vector3.up * 1.8f;
            Vector3 dir = ent.transform.position - origin;
            float dist = Vector3.Distance(origin, ent.transform.position);

            bool blocked = false;

            if (Physics.Raycast(origin, dir, out hit, dist, obstaclesMask))
            {
                if (hit.collider)
                {
                    blocked = true;
                }
            }

            return blocked;
        }

        public void RemoveFromVicinity(ItemEntity ent)
        {
            if (ent)
            {
                if (InVicinity.Contains(ent))
                {
                    RemoveFromUI(ent);
                    InVicinity.Remove(ent);
                }
            }
        }

        void AddItemToUI(ItemEntity ent)
        {
            VicinitySlot slot = Instantiate(inv.vicinitySlotPrefab, inv.vicinityContent).GetComponent<VicinitySlot>();

            slot.Init(ent);
        }

        void RemoveFromUI(ItemEntity ent)
        {
            VicinitySlot[] slots = inv.vicinityContent.GetComponentsInChildren<VicinitySlot>();

            for(int i = 0; i < slots.Length; i++)
            {
                if(slots[i].entity == ent)
                {
                    Destroy(slots[i].gameObject);
                    return;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if(!isInit || !inv.isOpen)
            {
                return;
            }

            ItemEntity ent = other.GetComponent<ItemEntity>();

            if(ent)
            {
                AddItemToVicinity(ent);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!isInit || !inv.isOpen)
            {
                return;
            }

            ItemEntity ent = other.GetComponent<ItemEntity>();

            if(ent)
            {
                RemoveFromVicinity(ent);
            }
        }
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class EquipmentController : MonoBehaviour 
	{
        private static HumanBodyBones backBone = HumanBodyBones.Chest;

        private PlayerRoot root;

        public BackpackEntity eqBackpack;

        private Animator anim;

        private bool isInit = false;

        public void Init(PlayerRoot r)
        {
            root = r;
            anim = root.model.GetComponentInChildren<Animator>();

            isInit = true;
        }

        public void EquipItem(ItemEntity it)
        {
            EItemType type = it.item.itemType;

            switch (type)
            {
                case EItemType.HEAD:
                    break;
                case EItemType.BACK:
                    BackpackEntity bp = (BackpackEntity)it;
                    EquipBack(bp);
                    break;
                case EItemType.VEST:
                    break;
            }
        }

        void EquipBack(BackpackEntity ent)
        {
            ent.transform.SetParent(anim.GetBoneTransform(backBone), true);

            ent.transform.localPosition = ent.backpack.equipPos;
            ent.transform.localEulerAngles = ent.backpack.equipRot;

            if (root.isLocalPlayer)
            {
                Utilities.SetLayerRecursive(ent.gameObject, 9);
            }
            else
            {
                Utilities.SetLayerRecursive(ent.gameObject, 10);
            }
        }
    }
}
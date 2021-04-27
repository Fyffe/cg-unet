using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class PlayerViewmodelIK : MonoBehaviour 
	{
        PlayerRoot root;
        Animator anim;
        PlayerController controller;

        [Range(0,1)]
        public float mainHandWeight;
        [Range(0, 1)]
        public float offHandWeight;

        public Transform rightHandTarget;
        public Transform leftHandTarget;

        public WeaponEntity weapon;

        Transform shoulder;
        Transform aimPivot;

        bool isInit = false;

        public void Init(PlayerRoot r)
        {
            root = r;
            anim = GetComponent<Animator>();
            controller = root.controller;

            shoulder = anim.GetBoneTransform(HumanBodyBones.RightShoulder).transform;
            aimPivot = new GameObject().transform;
            aimPivot.name = "Aim Pivot";
            aimPivot.transform.parent = controller.transform;

            isInit = true;
        }

        void OnAnimatorMove()
        {
            if(!isInit)
            {
                return;
            }

            HandleShoulder();
        }

        void HandleShoulder()
        {
            HandleShoulderPosition();
            HandleShoulderRotation();
        }

        void HandleShoulderPosition()
        {
            aimPivot.position = shoulder.position;
        }

        void HandleShoulderRotation()
        {
            Vector3 targetDir = transform.forward;

            if (targetDir == Vector3.zero)
            {
                targetDir = aimPivot.forward;
            }

            Quaternion targetLookDir = Quaternion.LookRotation(targetDir);

            aimPivot.rotation = Quaternion.Slerp(aimPivot.rotation, targetLookDir, Time.deltaTime * 15f);
        }

        public void EquipWeapon(WeaponEntity ent)
        {
            weapon = ent;

            leftHandTarget = null;
            rightHandTarget = null;

            if(weapon.leftHandTarget)
            {
                leftHandTarget = weapon.leftHandTarget;
            }

            if(weapon.rightHandTarget)
            {
                rightHandTarget = weapon.rightHandTarget;
            }
        }

        public void UnequipWeapon()
        {
            weapon = null;

            leftHandTarget = null;
            rightHandTarget = null;
        }

        void OnAnimatorIK()
        {
            if(!isInit || !rightHandTarget || !leftHandTarget)
            {
                return;
            }

            if (leftHandTarget != null)
            {
                UpdateIK(AvatarIKGoal.LeftHand, leftHandTarget, offHandWeight);
            }

            UpdateIK(AvatarIKGoal.RightHand, rightHandTarget, mainHandWeight);
        }

        void UpdateIK(AvatarIKGoal goal, Transform t, float weight)
        {
            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKRotationWeight(goal, weight);
            anim.SetIKPosition(goal, t.position);
            anim.SetIKRotation(goal, t.rotation);
        }
    }
}
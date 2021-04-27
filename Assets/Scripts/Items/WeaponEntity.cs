using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class WeaponEntity : ItemEntity 
	{
        public Weapon weapon;
        public Animator anim;
        public Transform model;

        public Transform leftHandTarget;
        public Transform rightHandTarget;
        public Transform magTarget;
        public Transform ejectTarget;
        
        public override void Init(Item it, int amt, int id)
        {
            base.Init(it, amt, id);

            weapon = (Weapon)it;

            anim = GetComponentInChildren<Animator>();
            model = anim.transform;

            leftHandTarget = model.Find("LH Target");
            rightHandTarget = model.Find("RH Target");
            magTarget = Utilities.FindChildRecursive(model, "Magazine Target").transform;
            ejectTarget = Utilities.FindChildRecursive(model, "Ejection Target").transform;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class BackpackEntity : ItemEntity 
	{
        public Backpack backpack;

        public Transform model;

        public Transform slot1;
        public Transform slot2;

        public override void Init(Item it, int amt, int id)
        {
            base.Init(it, amt, id);

            backpack = (Backpack)it;

            model = transform.Find("Model");

            slot1 = transform.Find("Slot 1");
            slot2 = transform.Find("Slot 2");
        }
    }
}
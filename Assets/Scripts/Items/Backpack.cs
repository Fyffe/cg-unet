using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [CreateAssetMenu(menuName = "Items/Back/Backpack")]
    public class Backpack : Item
	{
        public Vector3 equipPos;
        public Vector3 equipRot;

        public int capacity;
    }
}
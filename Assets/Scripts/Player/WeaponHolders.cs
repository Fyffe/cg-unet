using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class WeaponHolders : MonoBehaviour 
	{
        public PlayerRoot root;

        public List<WeaponHolder> Holders = new List<WeaponHolder>();

        private bool isInit = false;

        public void Init(PlayerRoot r)
        {
            root = r;

            isInit = true;
        }
    }

    [System.Serializable]
    public class WeaponHolder
    {
        public int id;
        public Transform baseHolder;
        public Transform holder;

        public WeaponHolder(Transform tr)
        {
            baseHolder = tr;
        }
    }
}
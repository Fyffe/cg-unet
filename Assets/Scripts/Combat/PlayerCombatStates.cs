using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [System.Serializable]
	public class PlayerCombatStates 
	{
        public bool canAim;
        public bool canAttack;
        public bool canReload;

        public bool isReloading;
        public bool isHolstering;
        public bool isEquipping;
        public bool isAiming;

        public float aimStartTime;
        public float aimStopTime;
    }
}
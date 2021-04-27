using UnityEngine;

namespace LV
{
    [System.Serializable]
	public class PlayerStates 
	{
        public bool isSprinting;
        public bool isMoving;
        public bool isJumping;
        public bool isGrounded;
        public bool canMove = true;
        public bool inMenus;
	}
}
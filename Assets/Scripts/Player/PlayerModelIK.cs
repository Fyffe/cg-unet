using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class PlayerModelIK : MonoBehaviour 
	{
        PlayerRoot root;
        Animator anim;
        PlayerController controller;

        public Vector3 lookAt;

        bool isInit = false;

        public void Init(PlayerRoot r)
        {
            root = r;
            anim = GetComponent<Animator>();
            controller = root.controller;

            isInit = true;
        }

        void Update()
        {
            if(!isInit)
            {
                return;
            }

            if (root.identity.isLocalPlayer)
            {
                Ray r = new Ray(root.cam.transform.position, root.cam.transform.forward);
                lookAt = r.GetPoint(10);
            }
        }

        void OnAnimatorIK()
        {
            if(!isInit)
            {
                return;
            }
            
            anim.SetLookAtPosition(lookAt);
            anim.SetLookAtWeight(1, 0.7f, 1, 1, 1);
        }
    }
}
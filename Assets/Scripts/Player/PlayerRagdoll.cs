using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class PlayerRagdoll : MonoBehaviour 
	{
        public PlayerRoot root;
        public Transform body;

        private Animator anim;
        private List<Rigidbody> Rigids;
        private List<Collider> Cols;

        private bool isInit;

        public void Init(PlayerRoot r)
        {
            root = r;

            body = root.model.transform;

            anim = body.GetComponent<Animator>();

            Rigids = new List<Rigidbody>(body.GetComponentsInChildren<Rigidbody>());
            Cols = new List<Collider>(body.GetComponentsInChildren<Collider>());

            ToggleRagdoll(false);

            if (root.isLocalPlayer)
            {
                root.meta.onDeath += EnableRagdoll;
            }

            isInit = true;
        }

        public void EnableRagdoll()
        {
            ToggleRagdoll(true);
        }

        public void ToggleRagdoll(bool b)
        {
            anim.enabled = !b;

            for(int i = 0; i < Rigids.Count; i++)
            {
                Rigids[i].isKinematic = !b;
            }

            for(int i = 0; i < Cols.Count; i++)
            {
                Cols[i].isTrigger = !b;
            }
        }
	}
}
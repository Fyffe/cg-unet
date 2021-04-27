using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
    [NetworkSettings(channel = 1, sendInterval = 0.1f)]
	public class PlayerMovementSync : NetworkBehaviour
	{
        private bool isInit;

        private PlayerRoot root;

        [SyncVar]
        protected Vector3 lastPos;
        [SyncVar]
        protected Quaternion lastRot;
        [SyncVar]
        protected Vector3 lastLook;

        public float lerpSpeed = 8f;
        public float slerpSpeed = 15f;

	    public void Init(PlayerRoot r)
        {
            root = r;

            isInit = true;
        }

        void Update()
        {
            if(!isInit)
            {
                return;
            }

            if(isLocalPlayer)
            {
                float dist = Vector3.Distance(lastPos, transform.position);

                if (dist >= 0.1f)
                {
                    CmdUpdatePosition(transform.position);
                }

                float angle = Quaternion.Angle(lastRot, transform.rotation);

                if(angle >= 1)
                {
                    CmdUpdateRotation(transform.rotation);
                }

                float lookDiff = Vector3.Distance(lastLook, root.modelIK.lookAt);

                if(lookDiff > 0.25f)
                {
                    CmdUpdateLookPosition(root.modelIK.lookAt);
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, lastPos, Time.deltaTime * lerpSpeed);
                transform.rotation = Quaternion.Slerp(transform.rotation, lastRot, Time.deltaTime * slerpSpeed);

                root.modelIK.lookAt = Vector3.Lerp(root.modelIK.lookAt, lastLook, Time.deltaTime * slerpSpeed);
            }
        }

        [Command]
        void CmdUpdatePosition(Vector3 pos)
        {
            lastPos = pos;
        }

        [Command]
        void CmdUpdateRotation(Quaternion rot)
        {
            lastRot = rot;
        }

        [Command]
        void CmdUpdateLookPosition(Vector3 pos)
        {
            lastLook = pos;
        }
	}
}
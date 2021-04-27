using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class DecalManager : MonoBehaviour 
	{
        public static DecalManager instance;

        public int bulletholeCount = 50;
        public int bulletholeIndex;

        public GameObject bulletholePrefab;

        private List<GameObject> Bulletholes = new List<GameObject>();

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            CreateBulletholes();
        }

        void CreateBulletholes()
        {
            if(!bulletholePrefab)
            {
                return;
            }

            for(int i = 0; i < bulletholeCount; i++)
            {
                GameObject bHole = Instantiate(bulletholePrefab);
                bHole.transform.SetParent(transform);
                bHole.SetActive(false);

                Bulletholes.Add(bHole);
            }
        }

        public void SetDecalAt(EDecalType type, Vector3 pos, Quaternion rot, Transform parent = null)
        {
            GameObject selected = null;

            switch (type)
            {
                case EDecalType.BULLETHOLE:

                    if (bulletholeIndex + 1 == bulletholeCount)
                    {
                        bulletholeIndex = 0;
                    }
                    else
                    {
                        bulletholeIndex++;
                    }

                    selected = Bulletholes[bulletholeIndex];

                    break;
            }

            if(selected)
            {
                if(parent)
                {
                    selected.transform.SetParent(parent, true);
                }

                selected.transform.rotation = rot;
                selected.transform.position = pos + (selected.transform.up * 0.025f);
                selected.SetActive(true);
            }
        }
    }
}
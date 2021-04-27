using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
    [DefaultExecutionOrder(-100)]
	public class ItemsDatabase : MonoBehaviour 
	{
        public static ItemsDatabase instance;

        public List<Item> Items = new List<Item>();

        void Awake()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            Items = new List<Item>(Resources.LoadAll<Item>("Items"));
        }

        public Item FindItem(string slug)
        {
            for(int i = 0; i < Items.Count; i++)
            {
                if(Items[i].slug == slug)
                {
                    return Items[i];
                }
            }

            return null;
        }

        public Item FindItem(int id)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].id == id)
                {
                    return Items[i];
                }
            }

            return null;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public static class Utilities
	{
	    public static GameObject FindChildRecursive(Transform tr, string name)
        {
            if (tr.childCount > 0)
            {
                for (int i = 0; i < tr.childCount; i++)
                {
                    GameObject child = tr.GetChild(i).gameObject;

                    if (child.name == name)
                    {
                        return child;
                    }
                    else
                    {
                        GameObject temp = FindChildRecursive(child.transform, name);

                        if(temp)
                        {
                            return temp;
                        }
                    }
                }
            }

            return null;
        }

        public static void SetLayerRecursive(GameObject go, int id)
        {
            go.layer = id;

            if(go.transform.childCount > 0)
            {
                for(int i = 0; i < go.transform.childCount; i++)
                {
                    SetLayerRecursive(go.transform.GetChild(i).gameObject, id);
                }
            }
        }
	}
}
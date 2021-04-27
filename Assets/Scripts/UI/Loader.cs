using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class Loader : MonoBehaviour 
	{
        public GameObject loader;

        public void ToggleLoader(bool b)
        {
            loader.SetActive(b);
        }
	}
}
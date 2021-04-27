using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class LevelController : MonoBehaviour 
	{
        public static LevelController instance;

        public Camera levelCamera;
        public AudioListener levelListener;

        void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        public void ToggleLevelCamera(bool b)
        {
            Debug.Log("DOES IT EVEN WORK?");

            levelCamera.enabled = b;
            levelListener.enabled = b;

            Debug.Log("TOGGLED CAMERA TO " + b);
        }
	}
}
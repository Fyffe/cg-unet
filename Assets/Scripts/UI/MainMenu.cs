using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class MainMenu : MenuRoot 
	{
        public static MainMenu instance;
        public InputField nameIpt;

        protected void Awake()
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

        protected override void Start()
        {
            base.Start();

            LoadSavedData();
        }

        void LoadSavedData()
        {
            string pName = PlayerPrefs.GetString("Player_Name");

            if (pName.Length > 0)
            {
                nameIpt.text = pName;
            }
        }

        public void SaveName()
        {
            if (nameIpt.text.Length > 0)
            {
                PlayerPrefs.SetString("Player_Name", nameIpt.text);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
	}
}
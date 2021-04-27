using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LV
{
	public class MenuManager : MonoBehaviour 
	{
        public MainMenu mainMenu;
        public LobbyMenu lobbyMenu;

        void Awake()
        {
            mainMenu = GetComponentInChildren<MainMenu>();
            lobbyMenu = GetComponentInChildren<LobbyMenu>();
        }
	}
}
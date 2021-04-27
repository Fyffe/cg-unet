using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class LobbyMenu : MenuRoot 
	{
        public GameObject hostControls;
        public GameObject clientControls;

        public Text lobbyName;

        public Transform lobbyRoot;

        public void StartLobbyMenu(bool host)
        {
            ToggleRoot(true);

            ToggleMenu(SubMenus[0], true);

            if(host)
            {
                hostControls.SetActive(true);
                clientControls.SetActive(false);
            }
            else
            {
                hostControls.SetActive(false);
                clientControls.SetActive(true);
            }
        }

        public void Disconnect()
        {
            CGNetworkManager.instance.StopHost();
            CGLobby.instance.ClearLobby();
            ToggleRoot(false);
            manager.mainMenu.ToggleRoot(true);
            manager.mainMenu.OpenMenu(0);
        }
	}
}
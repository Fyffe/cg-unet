using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class HostMenu : MonoBehaviour 
	{
        [SerializeField]
        private bool debug;

        public MainMenu mainMenu;
        public LobbyMenu lobbyMenu;

        public InputField nameIpt;
        public InputField portIpt;

        public Button hostButton;
        public Button backButton;

        void Awake()
        {
            mainMenu = GetComponentInParent<MainMenu>();
            lobbyMenu = transform.root.GetComponentInChildren<LobbyMenu>(true);

            LoadSavedData();
        }

        void LoadSavedData()
        {
            string sName = PlayerPrefs.GetString("Server_Name");
            string sPort = PlayerPrefs.GetString("Port_Server");

            if(sName.Length > 0)
            {
                nameIpt.text = sName;
            }

            if(sPort.Length > 0)
            {
                portIpt.text = sPort;
            }
        }

        public void SaveName()
        {
            if(nameIpt.text.Length > 0)
            {
                PlayerPrefs.SetString("Server_Name", nameIpt.text);
            }
        }

        public void SavePort()
        {
            if (portIpt.text.Length > 0)
            {
                PlayerPrefs.SetString("Port_Server", portIpt.text);
            }
        }

        public void TryHost()
        {
            bool valid = true;
            bool success = false;

            hostButton.interactable = false;
            backButton.interactable = false;

            string serverName = nameIpt.text;

            if(serverName.Length <= 3)
            {
                valid = false;
            }

            string serverPort = portIpt.text;
            int serverPortNum = -1;

            if(!int.TryParse(serverPort, out serverPortNum))
            {
                valid = false;
            }

            if(serverPortNum <= 0)
            {
                valid = false;
            }

            if(debug)
            {
                valid = true;
            }
            
            if(valid)
            {
                CGNetworkManager.instance.localName = mainMenu.nameIpt.text;
                success = CGNetworkManager.instance.TryStartHost(serverName, serverPortNum);
            }
            
            if(success)
            {
                Debug.Log("Successfully hosted the server.");
                mainMenu.ToggleRoot(false);
                lobbyMenu.StartLobbyMenu(true);

                hostButton.interactable = true;
                backButton.interactable = true;

            }
            else
            {
                Debug.Log("Failed to host the server.");
                hostButton.interactable = true;
                backButton.interactable = true;
            }
        }
	}
}
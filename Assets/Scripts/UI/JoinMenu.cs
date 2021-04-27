using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class JoinMenu : MonoBehaviour 
	{
        [SerializeField]
        private bool debug;

        public MainMenu mainMenu;
        public LobbyMenu lobbyMenu;
        public Loader loader;

        public CanvasGroup group;
        
        public InputField addressIpt;
        public InputField portIpt;

        public Button joinButton;
        public Button backButton;

        public float connectTimeout = 8f;
        private float tryConnectTime;
        private bool tryingToConnect;

        void Awake()
        {
            mainMenu = GetComponentInParent<MainMenu>();
            lobbyMenu = transform.root.GetComponentInChildren<LobbyMenu>(true);
            loader = transform.root.GetComponentInChildren<Loader>(true);

            group = GetComponent<CanvasGroup>();

            LoadSavedData();
        }

        void LoadSavedData()
        {
            string jPort = PlayerPrefs.GetString("Port_Join");

            if (jPort.Length > 0)
            {
                portIpt.text = jPort;
            }

            string jAddress = PlayerPrefs.GetString("Address_Join");

            if (jAddress.Length > 0)
            {
                addressIpt.text = jAddress;
            }
        }

        void Update()
        {
            if (tryingToConnect)
            {
                if (Time.time - tryConnectTime > connectTimeout)
                {
                    tryingToConnect = false;
                    OnResponseFromServer(false);
                }
            }
        }

        public void TryJoin()
        {
            bool valid = true;

            string name = mainMenu.nameIpt.text;

            if(name.Length < 3)
            {
                valid = false;
            }

            string address = addressIpt.text;

            if(address.Length <= 0)
            {
                valid = false;
            }

            string port = portIpt.text;
            int portNum = -1;

            if(!int.TryParse(port, out portNum))
            {
                valid = false;
            }

            if (portNum <= 0)
            {
                valid = false;
            }

            if (debug)
            {
                valid = true;
            }

            if (valid)
            {
                CGNetworkManager.instance.localName = mainMenu.nameIpt.text;
                CGNetworkManager.instance.TryConnect(address, portNum, OnResponseFromServer);

                tryConnectTime = Time.time;
                tryingToConnect = true;

                ToggleLoader(true);
            }
        }

        public void SavePort()
        {
            if (portIpt.text.Length > 0)
            {
                PlayerPrefs.SetString("Port_Join", portIpt.text);
            }
        }

        public void SaveAddress()
        {
            if (addressIpt.text.Length > 0)
            {
                PlayerPrefs.SetString("Address_Join", addressIpt.text);
            }
        }

        public void OnResponseFromServer(bool success)
        {
            if (success)
            {
                Debug.Log("Successfully connected to the server.");

                mainMenu.ToggleRoot(false);
                lobbyMenu.StartLobbyMenu(false);
            }
            else
            {
                CGNetworkManager.instance.StopClient();
                Debug.Log("Failed to join the server.");
            }

            joinButton.interactable = true;
            backButton.interactable = true;

            tryingToConnect = false;

            ToggleLoader(false);
        }

        public void ToggleLoader(bool b)
        {
            loader.ToggleLoader(b);

            group.interactable = !b;
            group.alpha = b ? 0 : 1;
        }
    }
}
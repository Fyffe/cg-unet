using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
	public class CGLobby : MonoBehaviour 
	{
        public static CGLobby instance;

        public LobbyMenu ui;

        public List<LobbyPlayer> Players = new List<LobbyPlayer>();

        public GameObject lobbyPlayerPrefab;

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

        public void StartGame()
        {
            CGNetworkManager.instance.StartGame(1);
        }

        public void AddPlayer(int id, string name)
        {
            LobbyPlayer newPlayer = Instantiate(lobbyPlayerPrefab, ui.lobbyRoot).GetComponent<LobbyPlayer>();

            newPlayer.Init(this, id, name, CGNetworkManager.instance.isHost);

            Players.Add(newPlayer);
        }

        public void RemovePlayer(LobbyPlayer player)
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if(Players[i] == player)
                {
                    Destroy(Players[i].gameObject);
                    Players.RemoveAt(i);
                }
            }
        }

        public void SetLobbyName(string name)
        {
            ui.lobbyName.text = name;
        }

        public void ClearLobby()
        {
            for(int i = 0; i < Players.Count; i++)
            {
                Destroy(Players[i].gameObject);
            }

            Players.Clear();
        }

        public void KickPlayer(int id)
        {
            LobbyPlayer p = FindPlayer(id);

            if (p)
            {
                if (CGNetworkManager.instance.KickClient(id))
                {
                    RemovePlayer(p);
                }
            }
        }

        public LobbyPlayer FindPlayer(int id)
        {
            for(int i = 0; i < Players.Count; i++)
            {
                if(Players[i].connId == id)
                {
                    return Players[i];
                }
            }

            return null;
        }
	}
}
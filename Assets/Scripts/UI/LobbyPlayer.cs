using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LV
{
	public class LobbyPlayer : MonoBehaviour 
	{
        private CGLobby lobby;

        public Text nameTxt;
        public GameObject kickBtn;

        public int connId;

        public bool isInit;

        public void Init(CGLobby lobby, int connId, string name, bool host)
        {
            this.lobby = lobby;
            this.connId = connId;

            if(host && connId != 0)
            {
                kickBtn.SetActive(true);
            }

            transform.Find("Kick").GetComponentInChildren<Button>().onClick.AddListener(() => lobby.KickPlayer(connId));

            nameTxt.text = name;
            
            isInit = true;
        }
	}
}
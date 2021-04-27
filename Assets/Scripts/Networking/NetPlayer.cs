using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
    [System.Serializable]
	public class NetPlayer
	{
        public string name;

        public NetworkConnection conn;
        public NetworkIdentity avatar;
        public bool isHost;

        public NetPlayer(string name, NetworkConnection conn)
        {
            this.name = name;
            this.conn = conn;

            Players.Add(this);
        }

        public NetPlayer(string name, bool host)
        {
            this.name = name;
            this.conn = null;
            this.isHost = true;

            Players.Add(this);
        }

        public static List<NetPlayer> Players = new List<NetPlayer>();

        public static NetPlayer FindPlayer(NetworkInstanceId id)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].avatar)
                {
                    if (Players[i].avatar.netId == id)
                    {
                        return Players[i];
                    }
                }
            }

            return null;
        }

        public static NetPlayer FindPlayer(int id)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].conn != null)
                {
                    if (Players[i].conn.connectionId == id)
                    {
                        return Players[i];
                    }
                }
            }

            return null;
        }

        public static bool RemovePlayer(NetPlayer p)
        {
            if(Players.Contains(p))
            {
                Players.Remove(p);
                return true;
            }

            return false;
        }
    }
}
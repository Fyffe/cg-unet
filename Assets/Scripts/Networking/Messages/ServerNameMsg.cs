using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
	public class ServerNameMsg : MessageBase 
	{
        public string name = "Unknown";
        public bool started = false;
	}
}
using UnityEngine.Networking;

namespace LV
{
	public class HandshakeMsg : MessageBase
	{
        public int connId;
        public string name;
	}
}
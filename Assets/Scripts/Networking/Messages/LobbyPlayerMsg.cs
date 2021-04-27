using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace LV
{
	public class LobbyPlayerMsg : MessageBase
	{
        public int id;
        public string name;
	}
}
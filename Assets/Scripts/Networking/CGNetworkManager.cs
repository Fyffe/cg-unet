using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

namespace LV
{
	public class CGNetworkManager : NetworkManager 
	{
        public static CGNetworkManager instance;

        public string localName = "";
        public string serverName = "";

        public NetPlayer localPlayer;

        private int requestedSceneIndex = -1;
        private bool changeScene = false;

        public string serverPassword;

        private int connectedClients = 0;

        public delegate void OnResponseFromServer(bool b);
        private OnResponseFromServer onResponseFromServer;

        private bool inLobby = true;
        public bool isHost { get; private set; }

        public object manager { get; private set; }

        void Awake()
        {
            if(instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }

            offlineScene = "";
            onlineScene = "";

            SceneManager.sceneLoaded += OnLoadedScene;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLoadedScene;
        }

        public bool TryStartHost(string name, int port)
        {
            if(name.Length <= 0)
            {
                name = "Unknown Server";
            }

            if(port <= 0)
            {
                port = 7777;
            }

            networkPort = port;
            maxConnections = 8;

            CGLobby.instance.SetLobbyName(name);
            serverName = name;
            inLobby = true;

            StartHost();

            return isNetworkActive;
        }

        public void TryConnect(string ipAddress, int port, OnResponseFromServer callback)
        {
            networkAddress = ipAddress;

            if (port <= 0)
            {
                port = 7777;
            }

            networkPort = port;

            StartClient();

            onResponseFromServer = callback;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();

            RegisterServerHandlers();
            
            isHost = true;
        }

        public override void OnStopHost()
        {
            base.OnStopHost();

            isHost = false;
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            if(conn.hostId == -1)
            {
                return;
            }

            connectedClients++;

            StringMessage pwd = new StringMessage(serverPassword);
            ServerNameMsg srvName = new ServerNameMsg()
            {
                name = serverName,
                started = !inLobby
            };

            NetworkServer.SendToClient(conn.connectionId, (int)EMsgType.RECEIVE_PASSWORD, pwd);
            NetworkServer.SendToClient(conn.connectionId, (int)EMsgType.RECEIVE_SERVERNAME, srvName);

            if(!inLobby)
            {
                return;
            }

            for(int i = 0; i < NetPlayer.Players.Count; i++)
            {
                NetPlayer player = NetPlayer.Players[i];
                int id = 0;

                LobbyPlayerMsg p = new LobbyPlayerMsg();

                if(player.isHost)
                {
                    p.id = 0;
                }
                else
                {
                    p.id = player.conn.connectionId;
                    id = player.conn.connectionId;
                }

                p.name = player.name;

                NetworkServer.SendToClient(conn.connectionId, (int)EMsgType.RECEIVE_PLAYER, p);

                Debug.Log("Sending " + player.name + " with id " + id);
            }

            Debug.Log("Client with ID " + conn.connectionId + " connected!");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            Debug.Log("Player with ID " + conn.connectionId + " disconnected!");

            NetPlayer p = NetPlayer.FindPlayer(conn.connectionId);

            if (p == null)
            {
                return;
            }

            NetPlayer.RemovePlayer(p);
            Debug.Log("Found " + p);

            IntegerMessage msg = new IntegerMessage();
            msg.value = conn.connectionId;

            NetworkServer.SendToAll((int)EMsgType.RECEIVE_REMOVEPLAYER, msg);

            CGLobby.instance.RemovePlayer(CGLobby.instance.FindPlayer(conn.connectionId));
            
            connectedClients--;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            if (inLobby)
            {
                CGLobby.instance.ClearLobby();
                CGLobby.instance.ui.ToggleRoot(false);
                MainMenu.instance.ToggleRoot(true);
                MainMenu.instance.OpenMenu(0);
            }
            else
            {
                SceneManager.LoadSceneAsync(0);
            }

            connectedClients = 0;
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            
            if (inLobby)
            {
                CGLobby.instance.ClearLobby();
                CGLobby.instance.ui.ToggleRoot(false);
                MainMenu.instance.ToggleRoot(true);
                MainMenu.instance.OpenMenu(0);
            }
            else
            {
                if(SceneManager.GetActiveScene().buildIndex != 0)
                {
                    LevelController.instance.ToggleLevelCamera(true);
                }

                SceneManager.LoadSceneAsync(0);
            }
        }

        public override void OnStartClient(NetworkClient client)
        {
            base.OnStartClient(client);

            RegisterClientHandlers();
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            HandshakeMsg msg = new HandshakeMsg()
            {
                name = localName,
                connId = client.connection.connectionId
            };

            client.Send((int)EMsgType.RECEIVE_HANDSHAKE, msg);

            if (onResponseFromServer != null)
            {
                onResponseFromServer(true);
                onResponseFromServer = null;
            }
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            StopHost();
        }

        public void StartGame(int index)
        {
            IntegerMessage msg = new IntegerMessage();
            msg.value = index;

            requestedSceneIndex = index;
            changeScene = true;
            inLobby = false;

            NetworkServer.SendToAll((int)EMsgType.RECEIVE_SCENECHANGE, msg);
        }

        public void OnReceiveServerName(NetworkMessage msg)
        {
            ServerNameMsg decoded = msg.ReadMessage<ServerNameMsg>();
            string name = decoded.name;
            bool started = decoded.started;

            if(name.Length > 0)
            {
                CGLobby.instance.SetLobbyName(name);
            }

            if(started)
            {
                StopHost();
            }
        }

        public void OnReceivePassword(NetworkMessage msg)
        {
            var pwd = msg.ReadMessage<StringMessage>().value;

            if(pwd != serverPassword)
            {
                //Disconnect
            }
        }

        public void OnReceiveKick(NetworkMessage msg)
        {
            var val = msg.ReadMessage<EmptyMessage>();

            if (val != null)
            {
                StopHost();
            }
        }

        public void OnReceiveHandshake(NetworkMessage msg)
        {
            var val = msg.ReadMessage<HandshakeMsg>();
            Debug.Log("Handshake ID " + val.connId + " and name " + val.name);

            NetworkConnection conn = null;

            for(int i = 0; i < NetworkServer.connections.Count; i++)
            {
                NetworkConnection c = NetworkServer.connections[i];

                if (c != null)
                {
                    if (c.connectionId == val.connId)
                    {
                        conn = NetworkServer.connections[i];
                    }
                }
            }

            if (conn != null)
            {
                NetPlayer newPlayer = new NetPlayer(val.name, conn);

                Debug.Log("Client with ID " + conn.connectionId + " is called " + val.name + ".");
            }

            LobbyPlayerMsg outMsg = new LobbyPlayerMsg();
            outMsg.id = val.connId;
            outMsg.name = val.name;

            NetworkServer.SendToAll(1339, outMsg);
        }

        public void OnReceivePlayer(NetworkMessage msg)
        {
            var val = msg.ReadMessage<LobbyPlayerMsg>();

            int id = val.id;
            string name = val.name;

            Debug.Log("Received player with ID " + id + " and name " + name);

            CGLobby.instance.AddPlayer(id, name);
        }

        public void OnReceiveRemoveLobbyPlayer(NetworkMessage msg)
        {
            int id = msg.ReadMessage<IntegerMessage>().value;

            if(id >= 0)
            {
                LobbyPlayer p = CGLobby.instance.FindPlayer(id);

                if(p != null)
                {
                    CGLobby.instance.RemovePlayer(p);
                }
            }
        }

        public void OnReceiveClientLoaded(NetworkMessage msg)
        {
            int id = msg.ReadMessage<IntegerMessage>().value;

            if(id >= 0)
            {
                Debug.Log("Client with ID " + id + " succesfully loaded!");
            }
        }

        public void OnReceiveSceneChange(NetworkMessage msg)
        {
            int sceneIndex = msg.ReadMessage<IntegerMessage>().value;

            if(sceneIndex >= 0)
            {
                requestedSceneIndex = sceneIndex;
                changeScene = true;

                SceneManager.LoadSceneAsync(sceneIndex);
            }
        }

        public void OnLoadedScene(Scene scene, LoadSceneMode mode)
        {
            if (scene.buildIndex == requestedSceneIndex)
            {
                IntegerMessage msg = new IntegerMessage();
                msg.value = client.connection.connectionId;

                client.Send(1342, msg);
                
                changeScene = false;

                GameManager.instance.OnLoadMap();

                ClientScene.AddPlayer(0);
            }
        }

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {
            GameObject player = Instantiate(playerPrefab);
            Vector3 pos = Vector3.zero;
            pos.x += connectedClients - 1;
            pos.y = 2;

            player.transform.position = pos;

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        void RegisterServerHandlers()
        {
            NetworkServer.RegisterHandler((short)EMsgType.RECEIVE_PASSWORD, OnReceivePassword);
            NetworkServer.RegisterHandler((short)EMsgType.RECEIVE_HANDSHAKE, OnReceiveHandshake);
            NetworkServer.RegisterHandler((short)EMsgType.RECEIVE_CLIENTLOADED, OnReceiveClientLoaded);
        }

        void RegisterClientHandlers()
        {
            client.RegisterHandler((short)EMsgType.RECEIVE_PASSWORD, OnReceivePassword);
            client.RegisterHandler((short)EMsgType.RECEIVE_SERVERNAME, OnReceiveServerName);
            client.RegisterHandler((short)EMsgType.RECEIVE_KICK, OnReceiveKick);
            client.RegisterHandler((short)EMsgType.RECEIVE_PLAYER, OnReceivePlayer);
            client.RegisterHandler((short)EMsgType.RECEIVE_REMOVEPLAYER, OnReceiveRemoveLobbyPlayer);
            client.RegisterHandler((short)EMsgType.RECEIVE_SCENECHANGE, OnReceiveSceneChange);
        }

        public bool KickClient(int id)
        {
            if (isHost)
            {
                Debug.Log("Trying to kick player with ID " + id);

                NetPlayer p = NetPlayer.FindPlayer(id);

                if (p == null)
                {
                    return false;
                }

                Debug.Log("Found player " + p.name + " with ID " + p.conn.connectionId);

                NetworkServer.SendToClient(p.conn.connectionId, (int)EMsgType.RECEIVE_KICK, new EmptyMessage());

                Debug.Log("Kicked player " + p.name);

                return true;
            }

            return false;
        }
    }
}
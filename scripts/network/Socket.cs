using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Godot;
using LiteNetLib;

namespace Bonebreaker.Net
{
    /// <summary>
    /// A class responsible for any low level networking
    /// For something more high level see Network.cs
    /// </summary>
    public class Socket : INetEventListener
    {
        private const string KEY = "bb_a_v0.07";
        public int HostingPort => client.LocalPort;

        public NetManager client;

        public NetPeer Lobbyer;
        public List<NetPeer> Peers = new List<NetPeer>();

        public delegate void ReceiveAccountLoginResult (AccountLoginResult packet);
        public delegate void ReceiveOperationResult (EmptyOperationResult result);
        
        public event ReceiveAccountLoginResult OnAccountLoginResultReceived;
        public event ReceiveOperationResult OnOperationResultReceived;
        
        public TaskCompletionSource<bool> ConnectedToLobbyer = new TaskCompletionSource<bool>();
        
        public Socket ()
        {
            client = new NetManager(this);
            client.Start();
            
            ConnectToLobbyer();
        }

        public void Poll ()
        {
            client.PollEvents();
        }

        private void ConnectToLobbyer ()
        {
            GD.Print("connecting to lobbyer...");
            Lobbyer = client.Connect("127.0.0.1", 3456, KEY);
        }

        public void OnPeerConnected (NetPeer peer)
        {
            if (peer == Lobbyer)
            {
                GD.Print("connected to lobbyer !");
                ConnectedToLobbyer.TrySetResult(true);
            }
        }

        public void OnPeerDisconnected (NetPeer peer, DisconnectInfo disconnectInfo)
        {
            
        }

        public void OnNetworkError (IPEndPoint endPoint, SocketError socketError)
        {
            
        }

        public void OnNetworkReceive (NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            // get packet header
            PacketsList header = (PacketsList)reader.GetInt();

            switch (header)
            {
                case PacketsList.L_ACCOUNT_LOGIN_RESULT:
                    AccountLoginResult response = new AccountLoginResult();
                    response.Deserialize(reader);
                    
                    OnAccountLoginResultReceived.Invoke(response);
                    break;
                case PacketsList.L_OPERATION_RESULT:
                    EmptyOperationResult result = new EmptyOperationResult();
                    result.Deserialize(reader);
                    
                    OnOperationResultReceived.Invoke(result);
                    break;
            }
            
            reader.Recycle();
        }

        public void OnNetworkReceiveUnconnected (IPEndPoint remoteEndPoint, NetPacketReader reader,
            UnconnectedMessageType messageType)
        {
            
        }

        public void OnNetworkLatencyUpdate (NetPeer peer, int latency)
        {
            
        }

        public void OnConnectionRequest (ConnectionRequest request)
        {
            // only accept connection if it's from the same version
            request.AcceptIfKey(KEY);
        }
    }
}
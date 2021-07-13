using Godot;
using System;
using Nakama;

namespace Bonebreaker
{
    // TODO add try catch
    public class Nakama : Node
    {
        public static Nakama singleton;
        
        private const string NAKAMA_SERVER_KEY = "defaultkey";
        private const string NAKAMA_HOST = "20.101.88.88";
        private const int NAKAMA_PORT = 7350;
        private const string NAKAMA_SCHEME = "HTTP";

        private bool _isSocketConnecting;
        
        private IClient _clientNakama;
        private ISession _sessionNakama;
        private ISocket _socketNakama;

        [Signal] public delegate void SessionChanged ();
        [Signal] public delegate void SessionConnected ();
        [Signal] public delegate void SocketConnected ();

        public override void _Ready ()
        {
            singleton = this;
        }

        public IClient GetNakamaClient ()
        {
            if (_clientNakama == null)
            {
                _clientNakama = new Client(NAKAMA_SCHEME, NAKAMA_HOST, NAKAMA_PORT, NAKAMA_SERVER_KEY);
            }
            return _clientNakama;
        }

        public ISession GetNakamaSession () => _sessionNakama;

        public async void SetNakamaSession (ISession session)
        {
            if (_socketNakama != null)
            {
                await _socketNakama.CloseAsync();
                _socketNakama = null;
            }


            _sessionNakama = session;
            EmitSignal(nameof(SessionChanged));
            
            if(_sessionNakama != null && !_sessionNakama.IsExpired)
                EmitSignal(nameof(SessionConnected));
        }

        public async void ConnectNakamaSocket ()
        {
            if (_socketNakama != null) return;
            if (_isSocketConnecting) return;

            _isSocketConnecting = true;

            ISocket newSocket = Socket.From(_clientNakama);
            await newSocket.ConnectAsync(_sessionNakama);

            _socketNakama = newSocket;
            _isSocketConnecting = false;

            EmitSignal(nameof(SocketConnected));
        }

        public bool IsNakamaSocketConnected ()
            => _socketNakama is {IsConnected: true};
    }
}
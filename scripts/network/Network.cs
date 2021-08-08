using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Bonebreaker.Net
{
    public class AwaitingOperation
    {
        public int ID;
        public int TimeOut;
        
        public TaskCompletionSource<object> Task;
        public uint StartedAt;

        public AwaitingOperation (int timeout)
        {
            TimeOut = timeout;
            StartedAt = OS.GetTicksMsec();
            Task = new TaskCompletionSource<object>();
        }

        public void Finish (object param)
        {
            Task.TrySetResult(param);
            Delete();
        }

        public void CheckIfTimeout (uint time)
        {
            if (time - StartedAt > TimeOut)
            {
                Task.SetCanceled();
                Delete();
            }
        }

        private void Delete ()
        {
            AwaitingOperation op;
            Network.sg.AwaitingOperations.TryRemove(ID, out op);

            if (op != this)
            {
                Debug.Fail("Removed a duplicated operation !");
            }
        }
    }
    
    public class Network : Node
    {
        public static Network sg;

        private Socket socket;

        public ConcurrentDictionary<int, AwaitingOperation> AwaitingOperations =
            new ConcurrentDictionary<int, AwaitingOperation>();

        public override async void _Ready ()
        {
            socket = new Socket();
            
            socket.OnOperationResultReceived += OnOperationResultReceived;
            socket.OnAccountLoginResultReceived += OnAccountLoginResultReceived;
            
            sg = this;

            await socket.ConnectedToLobbyer.Task;
        }

        public override void _Process (float delta)
        {
            socket?.Poll();
            PollOperations();
        }

        private void PollOperations ()
        {
            foreach (AwaitingOperation operation in AwaitingOperations.Values)
            {
                operation.CheckIfTimeout(OS.GetTicksMsec());
            }
        }

        public static void SendToLobbyer (INetSerializable packet)
        {
            NetDataWriter writer = new NetDataWriter();
            packet.Serialize(writer);
            sg.socket.Lobbyer.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        private void OnOperationResultReceived (EmptyOperationResult result)
        {
            AwaitingOperations[result.OperationID].Task.SetResult(result);
        }

        private void OnAccountLoginResultReceived (AccountLoginResult result)
        {
            AwaitingOperations[result.OperationID].Task.SetResult(result);
        }
        
        #region Account Creation

        public static async Task<int> CreateAccount (Account account)
        {
            CreateAccount packet = new CreateAccount();
            int ticket = sg.RegisterNewOperation(4000);
            
            packet.OperationID = ticket;
            packet.Account = account;

            SendToLobbyer(packet);
            
            GD.Print("trying to register an account");
            
            // try to await a response
            try
            {
                object result = await sg.AwaitingOperations[ticket].Task.Task;
                EmptyOperationResult operationResult = result as EmptyOperationResult;
                
                GD.Print(operationResult.ErrorCode);
                return operationResult.ErrorCode;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e);
                return -1;
            }
        }
        #endregion
        
        #region Account Login
        
        public static async Task<AccountLoginResult> TryLogin (string email, string pwd)
        {
            LoginAccount packet = new LoginAccount();
            int ticket = sg.RegisterNewOperation(4000);
            
            packet.OperationID = ticket;
            packet.Credential = email;
            packet.Password = pwd;

            SendToLobbyer(packet);
            
            // try to await a response
            try
            {
                object result = await sg.AwaitingOperations[ticket].Task.Task;
                AccountLoginResult operationResult = result as AccountLoginResult;
                
                GD.Print(operationResult.ErrorCode);
                return operationResult;
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
        
        #endregion

        private int RegisterNewOperation (int timeout)
        {
            int operationID = 1;
            if (AwaitingOperations.Count != 0)
            {
                operationID = AwaitingOperations.Last().Key + 1;
            }

            RegisterOperation(operationID, timeout);

            return operationID;
        }

        private void RegisterOperation (int operationID, int timeout)
        {
            AwaitingOperations[operationID] = new AwaitingOperation(timeout) { ID = operationID };
        }
    }
}
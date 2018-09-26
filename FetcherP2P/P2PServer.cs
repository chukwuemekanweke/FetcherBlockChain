using System;
using System.Collections.Generic;
using System.Net;
using WebSocketSharp.Server;
using WebSocketSharp;
using Newtonsoft.Json;
using Models;

namespace FetcherP2P
{
    public class P2PServer
    {
        public int PORT = 55070;
        public string IP = "127.0.0.1";
        public static List<WebSocket> Clients = new List<WebSocket>();
        public static List<string> Peers = new List<string>();
        private WebSocketServer websocketServer;

        public BlockChain blockchain { get; set; }
        public SocketCommunicator socketCommunicator { get; set; }

        public P2PServer(BlockChain _blockChain, SocketCommunicator _socketCommunicator)
        {
            blockchain = _blockChain;
            socketCommunicator = _socketCommunicator;
        }


        public void SyncChains()
        {
            foreach (var peer in Clients)
            {

            }
        }


        public void PopulatePeers(string peersCSV)
        {
            if (!string.IsNullOrWhiteSpace(peersCSV))
            {
                string[] _peers = peersCSV.Split(new char[',']);
                for (int i = 0; i < _peers.Length; i++)
                {
                    Peers.Add(_peers[i]);
                }
            }
        }

        public void Listen()
        {


            websocketServer = new WebSocketServer(
                //IPAddress.Parse(IP),
                PORT);
            websocketServer.AddWebSocketService<ServerBehaviour>("/", () => new ServerBehaviour(blockchain, socketCommunicator));
            websocketServer.Start();
            if (websocketServer.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", websocketServer.Port);
                foreach (var path in websocketServer.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }


        }

        public void StopServer()
        {
            websocketServer.Stop();
        }



        public void CoonnectToPeers()
        {
            Peers.ForEach(peer =>
            {
                var ipAndPort = peer.Split(new char[] { ':' });

                var ws = new WebSocketSharp.WebSocket($"ws://{peer}/");

                ws.OnOpen += (s, e) =>
                {
                    var _ws = s as WebSocketSharp.WebSocket;
                    Console.WriteLine($"Connection With Peer {peer} Established Successfully");
                    Clients.Add(ws);
                    _ws.Send(JsonConvert.SerializeObject(blockchain.Chain));
                };

                ws.OnClose += (s, e) =>
                {
                    Console.WriteLine($"Connection With Peer {peer} Ended Successfully");
                    Clients.Remove(ws);

                };

                ws.OnMessage += (s, e) =>
                {
                    var _ws = s as WebSocketSharp.WebSocket;

                    socketCommunicator.MessageHandler(_ws, e.Data);

                };

                ws.OnError += (s, e) =>
                {
                    Console.WriteLine($"Error has happened");


                };

                ws.Connect();
            });
            //Console.ReadKey(true);

        }





        //public void Listen()
        //{

        //    var aServer = new WebSocketServer(8100, IPAddress.Any)
        //    {
        //        OnReceive = (UserContext context) =>
        //        {
        //            Console.WriteLine("Received Data From :" + context.ClientAddress);

        //            try
        //            {
        //                var json = context.DataFrame.ToString();

        //                // <3 dynamics
        //                dynamic obj = JsonConvert.DeserializeObject(json);

        //                switch ((int)obj.Type)
        //                {
        //                    case (int)CommandType.Register:
        //                        Register(obj.Name.Value, context);
        //                        break;
        //                    case (int)CommandType.Message:
        //                        ChatMessage(obj.Message.Value, context);
        //                        break;
        //                    case (int)CommandType.NameChange:
        //                        NameChange(obj.Name.Value, context);
        //                        break;
        //                }
        //            }
        //            catch (Exception e) // Bad JSON! For shame.
        //            {
        //                var r = new Response { Type = ResponseType.Error, Data = new { e.Message } };

        //                context.Send(JsonConvert.SerializeObject(r));
        //            }
        //        },
        //        OnSend = (UserContext context) =>
        //        {
        //            Console.WriteLine("Data Send To : " + context.ClientAddress);

        //        },
        //        OnConnect = (UserContext context) =>
        //        {
        //            Console.WriteLine("Client Connection From : " + context.ClientAddress);

        //            var me = new User { Context = context };

        //            OnlineUsers.TryAdd(me, String.Empty);
        //        },
        //        OnDisconnect = (UserContext context) =>
        //        {
        //            Console.WriteLine("Client Disconnected : " + context.ClientAddress);
        //            var user = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();

        //            string trash; // Concurrent dictionaries make things weird

        //            OnlineUsers.TryRemove(user, out trash);

        //            if (!String.IsNullOrEmpty(user.Name))
        //            {
        //                var r = new Response { Type = ResponseType.Disconnect, Data = new { user.Name } };

        //                Broadcast(JsonConvert.SerializeObject(r));
        //            }

        //            BroadcastNameList();

        //        },
        //        TimeOut = new TimeSpan(0, 5, 0)
        //    };

        //    aServer.Start();
        //    var command = string.Empty;
        //    while (command != "exit")
        //    {
        //        command = Console.ReadLine();
        //    }

        //    aServer.Stop();

        //}

        //private static void Register(string name, UserContext context)
        //{
        //    var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
        //    var r = new Response();

        //    if (ValidateName(name))
        //    {
        //        u.Name = name;

        //        r.Type = ResponseType.Connection;
        //        r.Data = new { u.Name };

        //        Broadcast(JsonConvert.SerializeObject(r));

        //        BroadcastNameList();
        //        OnlineUsers[u] = name;
        //    }
        //    else
        //    {
        //        SendError("Name is of incorrect length.", context);
        //    }
        //}


        //private static void ChatMessage(string message, UserContext context)
        //{
        //    var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == context.ClientAddress).Single();
        //    var r = new Response { Type = ResponseType.Message, Data = new { u.Name, Message = message } };

        //    Broadcast(JsonConvert.SerializeObject(r));

        //}


        //private static void NameChange(string name, UserContext aContext)
        //{
        //    var u = OnlineUsers.Keys.Where(o => o.Context.ClientAddress == aContext.ClientAddress).Single();

        //    if (ValidateName(name))
        //    {
        //        var r = new Response
        //        {
        //            Type = ResponseType.NameChange,
        //            Data = new { Message = u.Name + " is now known as " + name }
        //        };
        //        Broadcast(JsonConvert.SerializeObject(r));

        //        u.Name = name;
        //        OnlineUsers[u] = name;

        //        BroadcastNameList();
        //    }
        //    else
        //    {
        //        SendError("Name is of incorrect length.", aContext);
        //    }
        //}

        //private static void SendError(string errorMessage, UserContext context)
        //{
        //    var r = new Response { Type = ResponseType.Error, Data = new { Message = errorMessage } };

        //    context.Send(JsonConvert.SerializeObject(r));
        //}

        //private static void BroadcastNameList()
        //{
        //    var r = new Response
        //    {
        //        Type = ResponseType.UserCount,
        //        Data = new { Users = OnlineUsers.Values.Where(o => !String.IsNullOrEmpty(o)).ToArray() }
        //    };
        //    Broadcast(JsonConvert.SerializeObject(r));
        //}

        //private static void Broadcast(string message, ICollection<User> users = null)
        //{
        //    if (users == null)
        //    {
        //        foreach (var u in OnlineUsers.Keys)
        //        {
        //            u.Context.Send(message);
        //        }
        //    }
        //    else
        //    {
        //        foreach (var u in OnlineUsers.Keys.Where(users.Contains))
        //        {
        //            u.Context.Send(message);
        //        }
        //    }
        //}


        //private static bool ValidateName(string name)
        //{
        //    var isValid = false;
        //    if (name.Length > 3 && name.Length < 25)
        //    {
        //        isValid = true;
        //    }

        //    return isValid;
        //}



    }



    class ServerBehaviour : WebSocketBehavior
    {
        BlockChain blockchain { get; set; }
        SocketCommunicator socketCommunicator { get; set; }
        public ServerBehaviour(BlockChain _blockChain, SocketCommunicator _socketCommunicator)
        {
            blockchain = _blockChain;
            socketCommunicator = _socketCommunicator;

        }

        protected override void OnOpen()
        {

            Uri clientURI = Context.WebSocket.Url;
            IPEndPoint ipEndPoint = Context.UserEndPoint;
            P2PServer.Clients.Add(Context.WebSocket);

            Console.WriteLine($"Client Connected on {ipEndPoint.Address}:{ipEndPoint.Port}");

            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {

            P2PServer.Clients.Remove(Context.WebSocket);
            base.OnClose(e);
        }


        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            socketCommunicator.MessageHandler(Context.WebSocket, e.Data);
            base.OnMessage(e);
        }
    }




    //public enum ResponseType
    //{
    //    Connection = 0,
    //    Disconnect = 1,
    //    Message = 2,
    //    NameChange = 3,
    //    UserCount = 4,
    //    Error = 255
    //}


    //public class Response
    //{
    //    public ResponseType Type { get; set; }
    //    public dynamic Data { get; set; }
    //}


    //public class User
    //{
    //    public string Name = String.Empty;
    //    public UserContext Context { get; set; }
    //}


    //public enum CommandType
    //{
    //    Register = 0,
    //    Message,
    //    NameChange
    //}
}

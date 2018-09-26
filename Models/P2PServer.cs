using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Linq;
using WebSocketSharp.Server;
using WebSocketSharp;


namespace Model
{
    public class P2PServer
    {
        public const int PORT = 5050;
        public const string IP = "127.0.0.1";
        public static List<IPEndPoint> Clients = new List<IPEndPoint>();
        public static List<string> Peers = new List<string>();


        public void PopulatePeers(string peersCSV)
        {
            string[] _peers = peersCSV.Split(new char[',']);
            for (int i = 0; i < _peers.Length; i++)
            {
                Peers.Add(_peers[i]);
            }
        }

        public void Listen()
        {


            var websocketServer = new WebSocketServer(PORT);
            websocketServer.AddWebSocketService<ServerBehaviour>("/");
            websocketServer.Start();
            if (websocketServer.IsListening)
            {
                Console.WriteLine("Listening on port {0}, and providing WebSocket services:", websocketServer.Port);
                foreach (var path in websocketServer.WebSocketServices.Paths)
                    Console.WriteLine("- {0}", path);
            }

            //Console.WriteLine("\nPress Enter key to stop the server...");
            //Console.ReadLine();
            websocketServer.Stop();



        }



        public void CoonnectToPeers()
        {
            Peers.ForEach(peer =>
            {
                var ipAndPort = peer.Split(new char[] { ':' });

                var ws = new WebSocketSharp.WebSocket($"ws://{peer}");

                ws.OnOpen += (s, e) =>
                {
                    Console.WriteLine($"Connection With Peer {peer} Established Successfully");
                    Clients.Add(new IPEndPoint(IPAddress.Parse(ipAndPort[0]), int.Parse(ipAndPort[1])));
                };

                ws.OnClose += Ws_OnClose;

                ws.OnMessage += Ws_OnMessage;

                ws.OnError += Ws_OnError;

                ws.Connect();
            });
            //Console.ReadKey(true);

        }

        private void Ws_OnError(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Ws_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Peer Says: " + e.Data);

        }

        private void Ws_OnClose(object sender, CloseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Ws_OnOpen(object sender, EventArgs e)
        {

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



    public class ServerBehaviour : WebSocketBehavior
    {
        public ServerBehaviour()
        {
        }

        protected override void OnOpen()
        {

            Uri clientURI = Context.WebSocket.Url;
            IPEndPoint ipEndPoint = Context.UserEndPoint;
            P2PServer.Clients.Add(ipEndPoint);



            Console.WriteLine($"Client Connected on {ipEndPoint.Address}:{ipEndPoint.Port}");

            base.OnOpen();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Uri clientURI = Context.WebSocket.Url;
            IPEndPoint ipEndPoint = Context.UserEndPoint;
            P2PServer.Clients.Remove(ipEndPoint);
            base.OnClose(e);
        }


        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
        }

        protected override void OnMessage(MessageEventArgs e)
        {

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

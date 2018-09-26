using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebSocketSharp;

namespace FetcherP2P
{
    public class SocketCommunicator
    {
        public BlockChain blockChain { get; set; }
        public SocketCommunicator(BlockChain _blockchain)
        {
            blockChain = _blockchain;
        }

        public void SendMessage(WebSocket socket, List<Block> blocks)
        {
            var stringifiedData = JsonConvert.SerializeObject(blocks);
            socket.Send(stringifiedData);
        }

        public void MessageHandler(WebSocket socket, string stringifiedData)
        {
            List<Block> chain = JsonConvert.DeserializeObject<List<Block>>(stringifiedData);


            Uri clientURI = socket.Url;

            Console.WriteLine($"Got Data From Client {clientURI} ---  ");

            foreach (var block in chain)
            {
                Console.WriteLine($"{block}");

            }
            blockChain.ReplaceChain(chain);


        }


    }
}

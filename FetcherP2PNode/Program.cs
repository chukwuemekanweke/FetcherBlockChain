using FetcherP2P;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.IO;

namespace FetcherP2PNode
{
    [Command(Name = "di", Description = "Dependency Injection sample project")]
    [HelpOption]
    class Program
    {
        public static int Main(string[] args)
        {

            //P2PServer p2p = new P2PServer();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);



            var serviceProvider = new ServiceCollection()
            .AddSingleton<BlockChain>()
            .AddSingleton<P2PServer>()
            .AddSingleton<IConfiguration>(builder.Build())
               .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(serviceProvider);

            return app.Execute(args);



        }

        private readonly BlockChain blockchain;
        private readonly P2PServer p2pServer;

        public Program(BlockChain _blcokchain, P2PServer _p2pServer, IConfiguration Configuration)
        {
            int PORT = 55051;
            string IP = "127.0.0.1";
            blockchain = _blcokchain;
            p2pServer = _p2pServer;

            p2pServer.IP = IP;
            p2pServer.PORT = PORT;
            string csvPeers = Configuration["Peers"];
            p2pServer.PopulatePeers(csvPeers);
        }

        private void OnExecute()
        {
            p2pServer.Listen();
            p2pServer.CoonnectToPeers();
            Console.ReadLine();
            p2pServer.StopServer();

        }

    }
}

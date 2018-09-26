using FetcherP2P;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FetcherP2PNode
{
    public class Startup
    {
        public static ServiceProvider ServiceProvider { get; set; }



        public static void ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton<BlockChain>()
                .AddSingleton<P2PServer>();

            // create a service provider from the service collection
            ServiceProvider = services.BuildServiceProvider();

            //// resolve the dependency graph
            //var appService = serviceProvider.GetService<IAppService>();

            //// run the application
            //appService.RunAsync().Wait();



        }




    }
}

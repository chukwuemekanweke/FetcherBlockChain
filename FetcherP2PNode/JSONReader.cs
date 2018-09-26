using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FetcherP2PNode
{
    public class JSONReader
    {

        public string Read()
        {

            var builder = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .AddUserSecrets<Program>()
                   .AddEnvironmentVariables();

            IConfigurationRoot configuration = builder.Build();
            return configuration["Peers"];



        }


    }
}

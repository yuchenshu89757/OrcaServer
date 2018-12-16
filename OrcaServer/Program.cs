using System;
using System.Collections.Generic;
using OrcaServer.View;
using OrcaServer.Model.Database;
using Grpc.Core;
using Com.Xiaoman;

namespace OrcaServer
{
    internal class MainClass
    {
        private static readonly string conf_path = "./Conf";
        private static readonly string conf_file = "program.conf";

        public static void Main(string[] args)
        {
            Console.WriteLine("Start to load configure....");
            Configure conf = new Configure();
            if (!conf.Load(conf_path, conf_file))
            {
                Console.WriteLine("Load conf failed.");
                return;
            }

            Console.WriteLine("Start to Init DatabaseConf...");
            if (!DatabaseConf.Init(conf))
            {
                Console.WriteLine("Init DatabaseConf failed.");
                return;
            }

            Console.WriteLine("Start to Init AdvManager...");
            if (!AdvManager.Init(conf))
            {
                Console.WriteLine("Init AdvManager failed.");
                return;
            }

            Console.WriteLine("Start to Init Server...");
            string host = conf["SERVER.IP"];
            int port = int.Parse(conf["SERVER.PORT"]);

            var options = new List<ChannelOption> { 
               new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue),
               new ChannelOption(ChannelOptions.MaxSendMessageLength, int.MaxValue)
            };
            Server orcaServer = new Server(options)
            {
                Services = { gRPC.BindService(new GRPCImpl()) },
                Ports = {new ServerPort(host, port, ServerCredentials.Insecure)}
            };

            Console.WriteLine("gRPC server listening on {0}:{1}", host, port);
            orcaServer.Start();
            while (true) {}

            //orcaServer.ShutdownAsync().Wait();
        }
    }
}

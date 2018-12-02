using System;
using OrcaServer.View;

namespace OrcaServer
{
    class MainClass
    {
        static readonly string conf_path = "./conf";
        static readonly string conf_file = "program.conf";

        public static void Main(string[] args)
        {
            Server orcaServer = new Server();

            Console.WriteLine("Start to load configure....");
            Configure conf = new Configure();
            if (!conf.Load(conf_path, conf_file))
            {
                Console.WriteLine("Load conf failed.");
                return;
            }
            Console.WriteLine("Start to Init Server...");
            if (!orcaServer.Init(conf)) {
                Console.WriteLine("Init Server failed.");
                return;
            }
            orcaServer.Run();
        }
    }
}

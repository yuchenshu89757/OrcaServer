using System;
using OrcaServer.View;

namespace OrcaServer.Model.Database
{
    public static class DatabaseConf
    {
        public static bool Init(Configure conf) 
        {
            try {
                Server = conf["DATABASE.SERVER"];
                Database = conf["DATABASE.DATABASE"];
                Username = conf["DATABASE.USERNAME"];
                Password = conf["DATABASE.PASSWORD"];
                Port = conf["DATABASE.PORT"];
                Backup = conf["DATABASE.BACKUP"];
                AdvTable = conf["DATABASE.ADV_TABLE"];
                ShowLogTable = conf["DATABASE.SHOWLOG_TABLE"];
                PrintLogTable = conf["DATABASE.PRINTLOG_TABLE"];
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        public static string Server { get; private set; }
        public static string Database { get; private set; }
        public static string Username { get; private set; }
        public static string Password { get; private set; }
        public static string Port { get; private set; }
        public static string Backup { get; private set; }
        public static string AdvTable { get; private set; }
        public static string ShowLogTable { get; private set; }
        public static string PrintLogTable { get; private set; }
    }
}

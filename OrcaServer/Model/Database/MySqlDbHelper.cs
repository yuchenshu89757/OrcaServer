using System;
using System.Text;
using System.Data;
using System.IO;
using System.Diagnostics;
using MySql.Data.MySqlClient;

namespace OrcaServer.Model.Database
{
    class MySqlDbHelper : IDatabaseHelper
    {
        readonly string m_DbServer;
        readonly string m_DbUserId;
        readonly string m_DbPwd;
        readonly string m_DbPort;
        readonly string m_DbDatabase;
        readonly string m_DbBackupRoot;
        readonly MySqlConnection m_DbConn;

        public MySqlDbHelper()
        {
            m_DbServer = DatabaseConf.Server;
            m_DbDatabase = DatabaseConf.Database;
            m_DbUserId = DatabaseConf.Username;
            m_DbPwd = DatabaseConf.Password;
            m_DbPort = DatabaseConf.Port;
            m_DbBackupRoot = DatabaseConf.Backup;

            string connString = "Data Source = " + m_DbServer + ";"
                    + "port = " + m_DbPort + ";"
                    + "Database = " + m_DbDatabase + ";"
                    + "User Id = " + m_DbUserId + ";"
                    + "Password = " + m_DbPwd + ";"
                    + "CharSet = utf8";
            m_DbConn = new MySqlConnection(connString);
        }

        bool Open()
        {
            try
            {
                m_DbConn.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server. Contact administrator");
                        break;
                    case 1045:
                        Console.WriteLine("Invalid username/password,please try again");
                        break;
                }
            }
            return false;
        }
        bool Close()
        {
            try
            {
                m_DbConn.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public DataSet ExecuteQuery(string sql)
        {
            DataSet ds = new DataSet();
            if (Open())
            {
                try
                {
                    MySqlDataAdapter sqlDa = new MySqlDataAdapter(sql, m_DbConn);
                    sqlDa.Fill(ds);
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Close();
                }
            }
            return ds;
        }

        public MySqlDataReader ExecuteReader(string sql)
        {
            if (Open())
            {
                try
                {
                    MySqlCommand sqlCmd = new MySqlCommand(sql, m_DbConn);
                    MySqlDataReader reader = sqlCmd.ExecuteReader(CommandBehavior.CloseConnection);
                    return reader;
                }
                catch
                {
                    Close();
                }
            }
            return null;
        }

        public void ExecuteUpdate(string sql)
        {
            if (Open())
            {
                try
                {
                    MySqlCommand sqlCmd = new MySqlCommand(sql, m_DbConn);
                    sqlCmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Close();
                }
            }
        }
        public void Backup()
        {
            try
            {
                DateTime dt = DateTime.Now;
                int m_DbYear = dt.Year;
                int m_DbMonth = dt.Month;
                int m_DbDay = dt.Day;
                int m_DbHour = dt.Hour;
                int m_DbMinute = dt.Minute;
                int m_DbSecond = dt.Second;
                int m_DbMillisecond = dt.Millisecond;
                string m_DbBackupPath = m_DbBackupRoot + m_DbYear + "-" + m_DbMonth + "-" + m_DbDay + "-" + m_DbHour + "-" + m_DbMinute
                    + "-" + m_DbSecond + "-" + m_DbMillisecond + ".sql";
                StreamWriter m_DbBackupFile = new StreamWriter(m_DbBackupPath);

                ProcessStartInfo m_DbPsi = new ProcessStartInfo
                {
                    FileName = "mysqldump",
                    RedirectStandardInput = false,
                    RedirectStandardOutput = true,
                    Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", m_DbUserId, m_DbPwd, m_DbServer, m_DbDatabase),
                    UseShellExecute = false
                };

                Process m_DbProcess = Process.Start(m_DbPsi);
                string m_DbOutput = m_DbProcess.StandardOutput.ReadToEnd();
                m_DbBackupFile.WriteLine(m_DbOutput);
                m_DbProcess.WaitForExit();
                m_DbBackupFile.Close();
                m_DbProcess.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void Restore()
        {
            try
            {
                string m_DbPath = "C:\\MySqlBackup.sql";
                StreamReader m_DbFile = new StreamReader(m_DbPath);
                string m_DbInput = m_DbFile.ReadToEnd();
                m_DbFile.Close();

                ProcessStartInfo m_DbPsi = new ProcessStartInfo
                {
                    FileName = "mysql",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}", m_DbUserId, m_DbPwd, m_DbServer, m_DbDatabase),
                    UseShellExecute = false
                };

                Process m_DbProcess = Process.Start(m_DbPsi);
                m_DbProcess.StandardInput.WriteLine(m_DbInput);
                m_DbProcess.StandardInput.Close();
                m_DbProcess.WaitForExit();
                m_DbProcess.Close();
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public void ExecuteSQLFile(string fileName)
        {
            string sql = File.ReadAllText(fileName, Encoding.UTF8);
            MySqlCommand myCommand = new MySqlCommand(sql)
            {
                Connection = m_DbConn
            };
            if (Open())
            {
                myCommand.ExecuteNonQuery();
                Close();
            }
        }
    }
}

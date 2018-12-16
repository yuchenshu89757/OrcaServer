using System;
using System.Collections;
using System.IO;
using System.Text;

namespace OrcaServer.View
{
    public class Configure
    {
        Hashtable m_table;

        public bool Load(string conf_path, string conf_file) {
            string file = string.Format("{0}/{1}", conf_path, conf_file);
            if (!File.Exists(file)) {
                Console.WriteLine("Configure file [{0}] not exist.", file);
                return false;
            }
            m_table = new Hashtable();
            using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
            {
                while (!sr.EndOfStream) {
                    string text = sr.ReadLine();
                    string[] fields = text.Split(':');
                    if (fields.Length < 2) {
                        Console.WriteLine("Invalid line [{0}] exist.", text);
                        return false;
                    }
                    string key = fields[0].Trim();
                    if (m_table.ContainsKey(key)) {
                        Console.WriteLine("Depulicate key [{0}] exist.", key);
                        return false;
                    }
                    m_table[key] = fields[1].Trim();
                }
            }
            Console.WriteLine("Load Configure succ");
            return true;
        }

        public string this[string key]
        {
            get {
                if (!m_table.ContainsKey(key)) {
                    throw new ArgumentException("No such key [{0}] exist.", key);
                }
                return m_table[key].ToString();
            }
        }
    }
}

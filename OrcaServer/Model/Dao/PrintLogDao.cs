using System.Text;
using OrcaServer.Model.Database;
using OrcaServer.Model.Entity;

namespace OrcaServer.Model.Dao
{
    public class PrintLogDao
    {
        readonly IDatabaseHelper db;

        public PrintLogDao()
        {
            db = new MySqlDbHelper();
        }

        public void Add(PrintLog log)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("insert into {0} (pl_dtime, pl_pc, pl_mac, pl_ip, pl_advid) ")
                .Append("values ({1}, '{2}', '{3}', '{4}', {5}");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.PrintLogTable, log.PrintLogDispTime,
                                       log.PrintLogPcName, log.PrintLogMacAddr, log.PrintLogIpAddr, log.PrintLogAdvId);
            db.ExecuteUpdate(sql);
        }
    }
}

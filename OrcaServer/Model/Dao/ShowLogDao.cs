using System;
using System.Text;
using OrcaServer.Model.Database;
using OrcaServer.Model.Entity;

namespace OrcaServer.Model.Dao
{
    public class ShowLogDao
    {
        private readonly IDatabaseHelper db;

        public ShowLogDao()
        {
            db = new MySqlDbHelper();
        }

        public void Add(ShowLog log)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("insert into {0} (sl_dtime, sl_mac, sl_advid, sl_pic) ")
                .Append("values ({1}, '2}', {3}, '{4}')");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.ShowLogTable, log.ShowLogDispTime,
                                       log.ShowLogMacAddr, log.ShowLogAdvId, log.ShowLogPicture);
            db.ExecuteUpdate(sql);
        }
    }
}

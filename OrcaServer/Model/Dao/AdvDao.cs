using System.Text;
using System.Data;
using System.Collections.Generic;
using OrcaServer.Model.Database;
using OrcaServer.Model.Entity;

namespace OrcaServer.Model.Dao
{
    public class AdvDao
    {
        private readonly IDatabaseHelper db;

        public AdvDao()
        {
            db = new MySqlDbHelper();
        }

        public void Add(Adv adv)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("insert into {0} (adv_id, adv_ctime, adv_etime) ")
                .Append("values ({1}, {2}, {3})");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.AdvTable, 
                                       adv.AdvId, adv.AdvCreationTime, adv.AdvExpirationTime);
            db.ExecuteUpdate(sql);
        }

        public void Update(Adv adv)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("update {0} set adv_ctime = {1}, adv_etime = {2 where adv_id = {3}");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.AdvTable, adv.AdvCreationTime,
                                       adv.AdvExpirationTime, adv.AdvId);
            db.ExecuteUpdate(sql);
        }

        public void Delete(ulong adv_id)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("delete from {0} where adv_id = {1}");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.AdvTable, adv_id);
            db.ExecuteUpdate(sql);
        }

        List<Adv> GetAdvList(string sql)
        {
            List<Adv> list = new List<Adv>();
            DataSet ds = db.ExecuteQuery(sql);
            DataTable dt = ds.Tables[0];
            foreach (DataRow dr in dt.Rows)
            {
                list.Add(new Adv
                {
                    AdvId = uint.Parse(dr["adv_id"].ToString()),
                    AdvCreationTime = uint.Parse(dr["adv_ctime"].ToString()),
                    AdvExpirationTime = uint.Parse(dr["adv_etime"].ToString()),
                });
            }
            return list;
        }

        public List<Adv> GetAll()
        {
            string sql = string.Format("select * from {0}", DatabaseConf.AdvTable);
            return GetAdvList(sql);
        }

        public List<Adv> GetByCtime(uint ctime)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("select * from {0} where adv_ctime >= {1} and adv_etime <= {2}");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.AdvTable, ctime, ctime);
            return GetAdvList(sql);
        }
    }
}

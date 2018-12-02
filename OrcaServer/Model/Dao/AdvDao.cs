using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using OrcaServer.Model.Database;
using OrcaServer.Model.Entity;
using MySql.Data.MySqlClient;

namespace OrcaServer.Model.Dao
{
    public class AdvDao
    {
        readonly MySqlDbHelper db;

        public AdvDao()
        {
            db = new MySqlDbHelper();
        }

        public void Add(Adv adv)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("insert into {0} (adv_ctime, adv_etime, adv_paper_4_3_len, adv_wallpaper_4_3, ")
                .Append("adv_paper_16_9_len, adv_wallpaper_16_9, adv_paper_div_len, adv_divpaper)")
                .Append(" values ({1}, {2}, {3}, '{4}', {5}, '{6}', {7}, '{8}')");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.AdvTable, adv.AdvCreationTime,
                                       adv.AdvExpirationTime, adv.AdvWpaper1Len, adv.AdvWpaper1, 
                                       adv.AdvWpaper2Len, adv.AdvWpaper2, adv.AdvDivPaperLen, adv.AdvDivPaper);
            db.ExecuteUpdate(sql);
        }

        public void Update(Adv adv)
        {
            StringBuilder sqlBuilder = new StringBuilder()
                .Append("update {0} set adv_ctime = {1}, adv_etime = {2}, adv_paper_4_3_len = {3}, ")
                .Append("adv_wallpaper_4_3 = '{4}', adv_paper_16_9_len = {5}, adv_wallpaper_16_9 = '{6}', ")
                .Append("adv_paper_div_len = {7}, adv_divpaper = '{8}' where adv_id = {9}");
            string sql = string.Format(sqlBuilder.ToString(), DatabaseConf.AdvTable, adv.AdvCreationTime,
                                       adv.AdvExpirationTime, adv.AdvWpaper1Len, adv.AdvWpaper1, adv.AdvWpaper2Len,
                                       adv.AdvWpaper2, adv.AdvDivPaperLen, adv.AdvDivPaper, adv.AdvId);
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
            MySqlDataReader reader = db.ExecuteReader(sql);
            if (reader == null)
            {
                return null;
            }
            List<Adv> list = new List<Adv>();
            while (reader.Read())
            {
                Console.WriteLine("Enter while...");
                Adv adv = new Adv
                {
                    AdvId = reader.GetUInt32(0),
                    AdvCreationTime = reader.GetUInt32(1),
                    AdvExpirationTime = reader.GetUInt32(2)

                };

                adv.AdvWpaper1Len = reader.GetInt32(reader.GetOrdinal("adv_paper_4_3_len"));
                adv.AdvWpaper1 = new byte[adv.AdvWpaper1Len];
                reader.GetBytes(reader.GetOrdinal("adv_wallpaper_4_3"), 0, adv.AdvWpaper1, 0, adv.AdvWpaper1Len);

                adv.AdvWpaper2Len = reader.GetInt32(reader.GetOrdinal("adv_paper_16_9_len"));
                adv.AdvWpaper2 = new byte[adv.AdvWpaper2Len];
                reader.GetBytes(reader.GetOrdinal("adv_wallpaper_16_9"), 0, adv.AdvWpaper2, 0, adv.AdvWpaper2Len);

                adv.AdvDivPaperLen = reader.GetInt32(reader.GetOrdinal("adv_paper_div_len"));
                adv.AdvDivPaper = new byte[adv.AdvDivPaperLen];
                reader.GetBytes(reader.GetOrdinal("adv_divpaper"), 0, adv.AdvDivPaper, 0, adv.AdvDivPaperLen);
                list.Add(adv);
            }
            reader.Close();
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

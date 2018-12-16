using System;
using System.IO;
using System.Threading.Tasks;
using Com.Xiaoman;
using Google.Protobuf;
using OrcaServer.Model.Entity;

namespace OrcaServer.View
{
    public static class AdvManager
    {
        private static readonly int MAX_CACHE_SIZE = 20;
        private static readonly string FILE_ROOT_PATH = "./data";

        private static LRUCache<uint, OrcaAdv> _adv_cache;
        private static string _file_root_path;

        public static bool Init(Configure conf)
        {
            try
            {
                int max_cache_size = conf.Contains("CACHE.MAX_CACHE_SIZE") ?
                        int.Parse(conf["CACHE.MAX_CACHE_SIZE"]) : MAX_CACHE_SIZE;
                _file_root_path = conf.Contains("CACHE.ROOT_PATH") ?
                        conf["CACHE.ROOT_PATH"] : FILE_ROOT_PATH;
                _adv_cache = new LRUCache<uint, OrcaAdv>(max_cache_size);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static void Save(OrcaAdv adv)
        {
            string filename = string.Format("{0}/{1}.dat", _file_root_path, adv.AdvId);
            FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

            CodedOutputStream output = new CodedOutputStream(fs, true);
            output.WriteMessage(adv);
            output.Flush();

            fs.Flush();
            fs.Close();
        }

        private static OrcaAdv Load(uint adv_id)
        {
            string filename = string.Format("{0}/{1}.dat", _file_root_path, adv_id);
            if (!File.Exists(filename))
            {
                return null;
            }
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            CodedInputStream input = new CodedInputStream(fs, true);
            OrcaAdv adv = new OrcaAdv { AdvId = adv_id };
            input.ReadMessage(adv);
            fs.Close();

            return adv;
        }

        public static void Set(OrcaAdv adv)
        {
            Save(adv);
            lock (_adv_cache)
            {
                _adv_cache.Set(adv.AdvId, adv);
            }
        }

        public static OrcaAdv Get(uint adv_id)
        {
            lock (_adv_cache)
            {
                if (_adv_cache.Contains(adv_id))
                {
                    return _adv_cache.Get(adv_id);
                }
                OrcaAdv adv = Load(adv_id);
                _adv_cache.Set(adv_id, adv);
                return adv;
            }
        }

        public static void Drop(uint adv_id)
        {
            string filename = string.Format("{0}/{1}.dat", _file_root_path, adv_id);
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
        }
    }

    public static class AdvIdGenerator
    {
        private static uint base_id = Util.ToTimestamp(DateTime.Now);

        public async static Task<uint> GetAdvId()
        {
            await Task.Run(() => { base_id++; });
            return base_id;
        }
    }

    public static class AdvUtil
    {
        public static Adv CreateAdv(OrcaAdv adv)
        {
            return new Adv
            {
                AdvId = adv.AdvId,
                AdvCreationTime = adv.CreationTime,
                AdvExpirationTime = adv.ExpirationTime
            };
        }
    }
}

using System;
using System.Net;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using OrcaServer.Model.Database;
using OrcaServer.Model.Dao;
using OrcaServer.Model.Entity;

using com.xiaoman;

namespace OrcaServer.View
{
    public class Server
    {
        bool m_exit;
        int m_max_client;
        TcpListener m_listener;
        readonly uint MAX_BUFF_LEN = 20 * 1024 * 1024;
        readonly int BYTES_PER_READ = 1024;

        public bool Init(Configure conf)
        {
            try
            {
                string ip = conf["SERVER.IP"];
                int port = int.Parse(conf["SERVER.PORT"]);
                m_max_client = int.Parse(conf["SERVER.MAX_CLIENT"]);
                m_listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ip), port));
                if (!DatabaseConf.Init(conf))
                {
                    Console.WriteLine("Init Databaseconf failed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public void Uninit()
        {
            m_exit = false;
            m_max_client = 0;
        }

        bool QuerySimpleAds(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null)
            {
                Console.WriteLine("Request or response is null");
                return false;
            }
            AdvDao dao = new AdvDao();
            List<Adv> list = request.query_ads_request.creation_time != 0 ?
                          dao.GetByCtime(request.query_ads_request.creation_time) :
                          dao.GetAll();
            foreach (var adv in list)
            {
                OrcaAdv orca_adv = new OrcaAdv()
                {
                    adv_id = adv.AdvId,
                    creation_time = adv.AdvCreationTime,
                    expiration_time = adv.AdvExpirationTime,
                    wall_paper_4_to_3 = adv.AdvWpaper1,
                    wall_paper_16_to_9 = adv.AdvWpaper2,
                    div_paper = adv.AdvDivPaper
                };
                response.query_ads_response.ads.Add(orca_adv);
            }
            return true;
        }

        bool QueryComplexAds(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null)
            {
                Console.WriteLine("Request or response is null");
                return false;
            }
            Console.WriteLine("ctime={0}", request.query_ads_request.creation_time);
            AdvDao dao = new AdvDao();
            uint timestamp = request.query_ads_request.creation_time != 0 ?
                                    request.query_ads_request.creation_time :
                                    Util.ToTimestamp(DateTime.Now);
            List<Adv> list = dao.GetByCtime(timestamp);
            foreach (var adv in list)
            {
                OrcaAdv orca_adv = new OrcaAdv()
                {
                    adv_id = adv.AdvId,
                    div_paper = adv.AdvDivPaper
                };
                if (request.query_ads_request.resolution == OrcaRequest.QueryAdsRequest.Resolution.RES_4_TO_3)
                {
                    orca_adv.wall_paper_4_to_3 = adv.AdvWpaper1;
                }
                if (request.query_ads_request.resolution == OrcaRequest.QueryAdsRequest.Resolution.RES_16_TO_9)
                {
                    orca_adv.wall_paper_16_to_9 = adv.AdvWpaper2;
                }
                response.query_ads_response.ads.Add(orca_adv);
            }
            return true;
        }

        bool QueryAds(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null || request.query_ads_request == null)
            {
                Console.WriteLine("Invalid Request, don't have query_ads_request.");
                return false;
            }
            response.query_ads_response = new OrcaResponse.QueryAdsResponse();
            return request.query_ads_request.type == OrcaRequest.QueryAdsRequest.QueryType.QUERY_TYPE_SIMPLE
                ? QuerySimpleAds(request, response)
                : request.query_ads_request.type == OrcaRequest.QueryAdsRequest.QueryType.QUERY_TYPE_COMPLEX
                && QueryComplexAds(request, response);
        }


        bool UpdateAds(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null || request.update_ads_request == null)
            {
                Console.WriteLine("Invalid Request, don't have update_ads_request.");
                return false;
            }
            response.update_ads_response = new OrcaResponse.UpdateAdsResponse();
            AdvDao dao = new AdvDao();
            switch (request.update_ads_request.type)
            {
                case OrcaRequest.UpdateAdsRequest.UpdateType.UPDATE_TYPE_CREATE:
                    dao.Add(Util.CreateAdvWithoutId(request.update_ads_request.adv));
                    break;
                case OrcaRequest.UpdateAdsRequest.UpdateType.UPDATE_TYPE_UPDATE:
                    Adv adv = Util.CreateAdvWithoutId(request.update_ads_request.adv);
                    adv.AdvId = request.update_ads_request.adv.adv_id;
                    dao.Update(adv);
                    break;
                case OrcaRequest.UpdateAdsRequest.UpdateType.UPDATE_TYPE_DELETE:
                    dao.Delete(request.update_ads_request.adv.adv_id);
                    break;
                default:
                    Console.WriteLine("Unknown update type");
                    response.update_ads_response.succeed = false;
                    return false;
            }
            response.update_ads_response.succeed = true;
            return true;
        }

        bool InsertScreenshots(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null || request.insert_screenshots_request == null)
            {
                Console.WriteLine("Invalid Request, don't have insert_screenshots_request.");
                return false;
            }
            response.insert_screenshots_response = new OrcaResponse.InsertScreenshotsResponse();
            ShowLogDao dao = new ShowLogDao();
            foreach (var sl in request.insert_screenshots_request.logs)
            {
                ShowLog log = new ShowLog()
                {
                    ShowLogAdvId = sl.ad_id,
                    ShowLogMacAddr = sl.mac_addr,
                    ShowLogDispTime = sl.disptime,
                    ShowLogPicture = sl.picture
                };
                dao.Add(log);
                response.insert_screenshots_response.inserted_shots++;
            }
            return true;
        }

        bool InsertLogs(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null || request.insert_logs_request == null)
            {
                Console.WriteLine("Invalid Request, don't have insert_logs_request.");
                return false;
            }
            response.insert_logs_response = new OrcaResponse.InsertLogsResponse();
            PrintLogDao dao = new PrintLogDao();
            foreach (var pl in request.insert_logs_request.logs)
            {
                PrintLog log = new PrintLog()
                {
                    PrintLogAdvId = pl.ad_id,
                    PrintLogIpAddr = pl.ip_addr,
                    PrintLogPcName = pl.pc_name,
                    PrintLogMacAddr = pl.mac_addr,
                    PrintLogDispTime = pl.disptime
                };
                dao.Add(log);
                response.insert_logs_response.inserted_logs++;
            }
            return true;
        }

        public bool DispatchRequest(OrcaRequest request, OrcaResponse response)
        {
            if (request == null || response == null)
            {
                Console.WriteLine("Input Request or Response is null");
                return false;
            }
            switch (request.command)
            {
                case OrcaCommand.ORCA_COMMAND_QUERY_ADS:
                    return QueryAds(request, response);
                case OrcaCommand.ORCA_COMMAND_UPDATE_ADS:
                    return UpdateAds(request, response);
                case OrcaCommand.ORCA_COMMAND_INSERT_LOGS:
                    return InsertLogs(request, response);
                case OrcaCommand.ORCA_COMMAND_INSERT_SCREENSHOTS:
                    return InsertScreenshots(request, response);
                default:
                    return false;
            }
        }

        public void HandleRequest(object obj)
        {
            Console.WriteLine("Enter handle request...");
            Socket socket = (Socket)obj;
            try
            {
                // get buffer len
                byte[] rece_len_buff = new byte[sizeof(int)];
                socket.Receive(rece_len_buff, 0, sizeof(int), SocketFlags.None);
                int data_size = BitConverter.ToInt32(rece_len_buff, 0);
                // get buffer data
                int offset = 0;
                byte[] rece_buffer = new byte[MAX_BUFF_LEN];
                do{
                    offset += socket.Receive(rece_buffer, offset, BYTES_PER_READ, SocketFlags.None);
                } while (offset < data_size);
                Console.WriteLine("Receive data success, data size[{0}]", offset);
                // deserialize receive data
                OrcaRequest request = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(rece_buffer, 0, offset);
                    stream.Position = 0;
                    request = ProtoBuf.Serializer.Deserialize<OrcaRequest>(stream);
                }
                Console.WriteLine("Request type[{0}]", request.command);
                // handle request and get response
                OrcaResponse response = new OrcaResponse()
                {
                    command = request.command
                };
                if (!DispatchRequest(request, response))
                {
                    Console.WriteLine("Dispatch request failed.");
                    return;
                }
                // serialize data to response
                byte[] send_buffer = null;
                using (MemoryStream stream = new MemoryStream())
                {
                    ProtoBuf.Serializer.Serialize(stream, response);
                    byte[] len_buffer = BitConverter.GetBytes(Convert.ToInt32(stream.Length));
                    send_buffer = new byte[len_buffer.Length + stream.Length];
                    Array.Copy(len_buffer, send_buffer, len_buffer.Length);
                    stream.Position = 0;
                    stream.Read(send_buffer, len_buffer.Length, send_buffer.Length - len_buffer.Length);
                }
                Console.WriteLine("Return response, size[{0}]", send_buffer.Length);
                socket.Send(send_buffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                Console.WriteLine("Goodbye...");
            }
        }

        public void Run()
        {
            m_listener.Start();
            while (!m_exit)
            {
                try
                {
                    Thread service = new Thread(new ParameterizedThreadStart(HandleRequest))
                    {
                        IsBackground = true
                    };
                    service.Start(m_listener.AcceptSocket());
                    Console.WriteLine("A client come...");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            m_listener.Stop();
        }
    }
}

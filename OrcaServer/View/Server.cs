using System;
using System.IO;
using System.Collections.Generic;
using OrcaServer.Model.Dao;
using OrcaServer.Model.Entity;
using Com.Xiaoman;
using Grpc.Core;
using System.Threading.Tasks;

namespace OrcaServer.View
{
    public class GRPCImpl : gRPC.gRPCBase
    {
        public override async Task QueryAdvAsync(QueryAdvRequest request, IServerStreamWriter<QueryAdvResponse> responseStream, ServerCallContext context)
        {
            var responses = QueryAdv(request);
            foreach (var response in responses)
            {
                await responseStream.WriteAsync(response);
            }
        }

        public override Task<UpdateAdvResponse> UpdateAdv(UpdateAdvRequest request, ServerCallContext context)
        {
            UpdateAdvResponse response = new UpdateAdvResponse { UpdateSucc = true };
            AdvDao dao = new AdvDao();
            switch (request.Type)
            {
                case UpdateAdvRequest.Types.UpdateType.Create:
                    Console.WriteLine("a new create comme...");
                    request.Adv.AdvId = AdvIdGenerator.GetAdvId().Result;
                    AdvManager.Save(request.Adv);
                    dao.Add(AdvUtil.CreateAdv(request.Adv));
                    break;
                case UpdateAdvRequest.Types.UpdateType.Update:
                    AdvManager.Set(request.Adv);
                    dao.Update(AdvUtil.CreateAdv(request.Adv));
                    break;
                case UpdateAdvRequest.Types.UpdateType.Delete:
                    AdvManager.Drop(request.Adv.AdvId);
                    dao.Delete(request.Adv.AdvId);
                    break;
                default:
                    Console.WriteLine("Unknown update type");
                    response.UpdateSucc = false;
                    break;
            }
            return Task.FromResult(response);
        }

        public override Task<InsertLogResponse> InsertLog(InsertLogRequest request, ServerCallContext context)
        {
            InsertLogResponse response = new InsertLogResponse { InsertSucc = true };
            PrintLog log = new PrintLog
            {
                PrintLogAdvId = request.PrintLog.AdId,
                PrintLogIpAddr = request.PrintLog.IpAddr,
                PrintLogPcName = request.PrintLog.PcName,
                PrintLogMacAddr = request.PrintLog.MacAddr,
                PrintLogDispTime = request.PrintLog.Disptime
            };
            PrintLogDao dao = new PrintLogDao();
            dao.Add(log);
            return Task.FromResult(response);
        }

        public override Task<InsertScreenshotResponse> InsertScreenhot(InsertScreenhotRequest request, ServerCallContext context)
        {
            InsertScreenshotResponse response = new InsertScreenshotResponse() { InsertSucc = true };
            ShowLog log = new ShowLog
            {
                ShowLogAdvId = request.ShowLog.AdId,
                ShowLogMacAddr = request.ShowLog.MacAddr,
                ShowLogDispTime = request.ShowLog.Disptime,
                ShowLogPicture = request.ShowLog.Picture.ToByteArray()
            };
            ShowLogDao dao = new ShowLogDao();
            dao.Add(log);
            return Task.FromResult(response);
        }

        List<QueryAdvResponse> QuerySimpleAdv(QueryAdvRequest request)
        {
            AdvDao dao = new AdvDao();
            List<Adv> list = request.CreationTime != 0 ? dao.GetByCtime(request.CreationTime) : dao.GetAll();
            List<QueryAdvResponse> responses = new List<QueryAdvResponse>();
            foreach (var adv in list)
            {
                QueryAdvResponse response = new QueryAdvResponse
                {
                    Adv = AdvManager.Get(adv.AdvId)
                };
                responses.Add(response);
            }
            return responses;
        }

        List<QueryAdvResponse> QueryComplexAdv(QueryAdvRequest request)
        {
            AdvDao dao = new AdvDao();
            uint timestamp = request.CreationTime != 0 ? request.CreationTime : Util.ToTimestamp(DateTime.Now);
            List<Adv> list = dao.GetByCtime(timestamp);
            List<QueryAdvResponse> responses = new List<QueryAdvResponse>();
            foreach (var adv in list)
            {
                OrcaAdv orca_adv = AdvManager.Get(adv.AdvId);
                QueryAdvResponse response = new QueryAdvResponse
                {
                    Adv = new OrcaAdv
                    {
                        AdvId = adv.AdvId,
                        DivPaper = orca_adv.DivPaper
                    }
                };
                if (request.Resolution == QueryAdvRequest.Types.Resolution.Res4To3)
                {
                    response.Adv.WallPaper4To3 = orca_adv.WallPaper4To3;
                }
                if (request.Resolution == QueryAdvRequest.Types.Resolution.Res16To9)
                {
                    response.Adv.WallPaper16To9 = orca_adv.WallPaper16To9;
                }
                responses.Add(response);
            }
            return responses;
        }

        private List<QueryAdvResponse> QueryAdv(QueryAdvRequest request)
        {
            if (request.Type == QueryAdvRequest.Types.QueryType.Simple)
            {
                return QuerySimpleAdv(request);
            }
            return QueryComplexAdv(request);
        }
    }
}

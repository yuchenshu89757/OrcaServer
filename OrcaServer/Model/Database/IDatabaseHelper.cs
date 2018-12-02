using System.Data;

namespace OrcaServer.Model.Database
{
    public interface IDatabaseHelper
    {
        DataSet ExecuteQuery(string sql);
        void ExecuteUpdate(string sql);
    }
}

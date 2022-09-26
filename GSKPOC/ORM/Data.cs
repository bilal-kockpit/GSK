using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace GSKPOC.ORM
{
    public class Data
    {
        DataMethod LocalDM;
        DataSet _localDS;
        private string _ConnectionStr = string.Empty;
        public Data(string ConnectionStr, DataSet LocalDS)
        {
            this._ConnectionStr = ConnectionStr;
            this._localDS = LocalDS;
            LocalDM = new DataMethod(this._ConnectionStr, LocalDS);
        }

        public void ExecuteSelectCommand(string strQuery, string strTableAlias)
        {
            LocalDM.ExecuteSelectCommand(strQuery, null, strTableAlias, true);
        }
        public void ExecuteSelectCommand(string strQuery, NpgsqlParameter[] sqlpara, string strTableAlias)
        {
            LocalDM.ExecuteSelectCommand(strQuery, sqlpara, strTableAlias, true);
        }
        public bool ExecuteNonQuery(string strQuery)
        {
            return LocalDM.ExecuteNonQueryCommand(strQuery, null, true);
        }
        public bool ExecuteNonQuery(string strQuery, NpgsqlParameter[] sqlpara)
        {
            return LocalDM.ExecuteNonQueryCommand(strQuery, sqlpara, true);
        }
    }
}

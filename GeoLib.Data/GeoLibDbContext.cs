using GeoLib.Core;
using System;
using System.Data;

using System.Data.OracleClient;
using System.Configuration;

namespace GeoLib.Data
{
    public class GeoLibDbContext 
    {
        public static DataTable GetOracleData(string Sql)
        {
            DataTable dtData = new DataTable();
            OracleConnection cn = new OracleConnection();
            OracleDataAdapter OracleDataAdapter = default(OracleDataAdapter);            
             cn.ConnectionString = ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;
                cn.Open();
                OracleDataAdapter = new OracleDataAdapter(Sql, cn);
                OracleDataAdapter.Fill(dtData);
                cn.Close();
                return dtData;

            
        }
        public static void UpdateOracleData(string sql)
        {
            
            OracleConnection cn = new OracleConnection();
            OracleCommand cm = new OracleCommand(sql, cn);
            
            cn.ConnectionString = ConfigurationManager.ConnectionStrings["OracleConn"].ConnectionString;
            cn.Open();
            cm.ExecuteNonQuery();   
            cn.Close();
            
        }
    }
}

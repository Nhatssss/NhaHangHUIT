using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Helper xu ly ket noi SQL Server
    /// Doc chuoi ket noi tu App.config
    /// </summary>
    public static class DataService
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["NhaHangHuit"].ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(GetConnectionString());
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }

        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null)
                    cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
    }
}

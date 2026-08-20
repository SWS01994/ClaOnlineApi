using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WebAPI.Helpers
{
    public static class SqlHelper
    {
        private static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }

        public static SqlParameter InParam(string name, SqlDbType type, object value)
        {
            SqlParameter p = new SqlParameter(name, type);
            p.Direction = ParameterDirection.Input;
            p.Value = (value == null) ? System.DBNull.Value : value;
            return p;
        }

        public static SqlParameter OutParam(string name, SqlDbType type)
        {
            SqlParameter p = new SqlParameter(name, type);
            p.Direction = ParameterDirection.Output;
            return p;
        }

        public static DataSet ExecuteDataSet(string procName, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                DataSet ds = new DataSet();
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(ds);
                }
                return ds;
            }
        }

        public static int ExecuteNonQuery(string procName, SqlParameter[] parameters)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        public static int ExecuteReturnNewId(string procName, SqlParameter[] parameters, string outputParamName)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            using (SqlCommand cmd = new SqlCommand(procName, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                if (parameters != null) cmd.Parameters.AddRange(parameters);

                conn.Open();
                cmd.ExecuteNonQuery();

                object value = cmd.Parameters[outputParamName].Value;
                if (value == null || value == System.DBNull.Value) return 0;
                return System.Convert.ToInt32(value);
            }
        }
    }
}
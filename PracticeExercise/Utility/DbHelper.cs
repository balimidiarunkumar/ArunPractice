using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace PracticeExercise.Utility
{
    public class DbHelper
    {
        public DataSet ExecuteQuery(string sqlQuery, List<SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = OpenDbConnection();
                SqlCommand sqlCommand = null;
                SqlDataAdapter sqlDataAdapter = null;

                sqlCommand = new SqlCommand(sqlQuery, sqlConnection) { CommandType = CommandType.Text };
                sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                if (parameters != null)
                    sqlCommand.Parameters.AddRange(parameters.ToArray());

                sqlDataAdapter.Fill(ds);

                DisposeSqlCommand(sqlCommand);
                sqlCommand = null;
            }
            catch (Exception ex) { System.Console.WriteLine(ex.Message); }
            finally { CloseDbConnection(sqlConnection); }
            return ds;
        }
        public string ExecuteNonQuery(string sqlQuery, List<SqlParameter> parameters)
        {
            string strRes = string.Empty;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = OpenDbConnection();
                SqlCommand sqlCommand = null;

                sqlCommand = new SqlCommand(sqlQuery, sqlConnection) { CommandType = CommandType.Text };

                if (parameters != null)
                    sqlCommand.Parameters.AddRange(parameters.ToArray());

                sqlCommand.ExecuteNonQuery();

                DisposeSqlCommand(sqlCommand);
                sqlCommand = null;
            }
            catch (Exception ex) { strRes = ex.Message; }
            finally { CloseDbConnection(sqlConnection); }
            return strRes;
        }
        public string ExecuteNonStoredProcedure(string sqlQuery, List<SqlParameter> parameters)
        {
            string strRes = string.Empty;
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = OpenDbConnection();
                SqlCommand sqlCommand = null;

                sqlCommand = new SqlCommand(sqlQuery, sqlConnection) { CommandType = CommandType.StoredProcedure };

                if (parameters != null)
                    sqlCommand.Parameters.AddRange(parameters.ToArray());

                sqlCommand.ExecuteNonQuery();

                DisposeSqlCommand(sqlCommand);
                sqlCommand = null;
            }
            catch (Exception ex) { strRes = ex.Message; }
            finally { CloseDbConnection(sqlConnection); }
            return strRes;
        }
        public DataSet ExecuteStoredProcedure(string sqlQuery, List<SqlParameter> parameters)
        {
            DataSet ds = new DataSet();
            SqlConnection sqlConnection = null;
            try
            {
                sqlConnection = OpenDbConnection();
                SqlCommand sqlCommand = null;
                SqlDataAdapter sqlDataAdapter = null;

                sqlCommand = new SqlCommand(sqlQuery, sqlConnection) { CommandType = CommandType.StoredProcedure };

                if (parameters != null)
                    sqlCommand.Parameters.AddRange(parameters.ToArray());

                sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                sqlDataAdapter.Fill(ds);

                DisposeSqlCommand(sqlCommand);
                sqlCommand = null;
            }
            catch (Exception ex) { System.Console.WriteLine(ex.Message); }
            finally { CloseDbConnection(sqlConnection); }
            return ds;
        }
        public SqlConnection OpenDbConnection()
        {
            SqlConnection sqlConnection = null;
            try
            {
                String connectionString = WebConfigurationManager.AppSettings["connString"];
                sqlConnection = new SqlConnection(connectionString);
                sqlConnection.Open();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }
            return sqlConnection;
        }
        public void CloseDbConnection(SqlConnection sqlConnection)
        {
            try
            {
                if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        public void DisposeSqlCommand(SqlCommand sqlCommand)
        {
            try
            {
                if (sqlCommand != null)
                    sqlCommand.Dispose();
            }
            catch (Exception ex) { System.Console.WriteLine(ex.Message); }
        }
    }
}
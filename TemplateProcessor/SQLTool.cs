using System;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;

namespace TemplateProcessor
{
    public class SqlTools : IDisposable
    {
        private string server;
        private string database;
        private string username;
        private string password;
        private TextBox txtLog;

        // Add an optional parameter to trust the server certificate
        private bool trustServerCertificate;

        // Add a field to track whether the object has been disposed
        private bool disposed = false;

        public SqlTools(string server, string database, string username, string password, bool trustServerCertificate = true)
        {
            this.server = server;
            this.database = database;
            this.username = username;
            this.password = password;
            this.trustServerCertificate = trustServerCertificate;
        }

        private string GetConnectionString()
        {
            return $"Server={server};Database={database};User Id={username};Password={password};" +
                   $"TrustServerCertificate={trustServerCertificate};";
        }

        public void DeleteTextlistFromTable(string tableName, string whereColumn, string whereValue)
        {
            try
            {
                var connectionString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand($"DELETE FROM {tableName} WHERE {whereColumn} = @WhereValue", conn))
                    {
                        cmd.Parameters.AddWithValue("@WhereValue", whereValue);
                        cmd.ExecuteNonQuery();
                    }
                    HelpFunc.LogMessage($"Deleted rows from {tableName} where {whereColumn} = {whereValue}");
                }
            }
            catch (Exception e)
            {
                HelpFunc.LogMessage($"An error occurred: {e.Message}");
            }
        }

        public void DeleteFromTableById(string tableName, string whereColumn, string whereValue, string idColumn, int idValue)
        {
            try
            {
                var connectionString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // First check if the record exists
                    using (SqlCommand checkCmd = new SqlCommand($"SELECT COUNT(1) FROM {tableName} WHERE {whereColumn} = @WhereValue AND {idColumn} = @IdValue", conn))
                    {
                        checkCmd.Parameters.Add(new SqlParameter("@WhereValue", whereValue ?? (object)DBNull.Value));
                        checkCmd.Parameters.Add(new SqlParameter("@IdValue", idValue));

                        int recordCount = (int)checkCmd.ExecuteScalar();
                        if (recordCount > 0)
                        {
                            // Proceed with delete if record exists
                            using (SqlCommand cmd = new SqlCommand($"DELETE FROM {tableName} WHERE {whereColumn} = @WhereValue AND {idColumn} = @IdValue", conn))
                            {
                                cmd.Parameters.Add(new SqlParameter("@WhereValue", whereValue ?? (object)DBNull.Value));
                                cmd.Parameters.Add(new SqlParameter("@IdValue", idValue));
                                cmd.ExecuteNonQuery();

                                HelpFunc.LogMessage($"Deleted row from {tableName} where {whereColumn} = '{whereValue}' and {idColumn} = {idValue}");
                            }
                        }
                        else
                        {
                            HelpFunc.LogMessage($"No record found in {tableName} where {whereColumn} = '{whereValue}' and {idColumn} = {idValue}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HelpFunc.LogMessage($"An error occurred: {e.Message}");
            }
        }

        public void InsertUpdateTextLists(string textlistName, int textlistId, string l1Text, string l2Text, string l3Text, string l4Text, string l5Text, string tagName, bool inUse)
        {
            try
            {
                var connectionString = GetConnectionString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("dbo.SP_TextList_Write", conn))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@Textlist_Name", textlistName);
                        cmd.Parameters.AddWithValue("@Textlist_ID", textlistId);
                        cmd.Parameters.AddWithValue("@L1_Text", l1Text);
                        cmd.Parameters.AddWithValue("@L2_Text", l2Text);
                        cmd.Parameters.AddWithValue("@L3_Text", l3Text);
                        cmd.Parameters.AddWithValue("@L4_Text", l4Text);
                        cmd.Parameters.AddWithValue("@L5_Text", l5Text);
                        cmd.Parameters.AddWithValue("@TagName", tagName);
                        cmd.Parameters.AddWithValue("@InUse", inUse);

                        cmd.ExecuteNonQuery();
                    }
                    HelpFunc.LogMessage($"Inserted/updated row {textlistName} with ID {textlistId} and text {l1Text}");
                }
            }
            catch (Exception e)
            {
                HelpFunc.LogMessage($"An error occurred: {e.Message}");
            }
        }

        // Implement the Dispose method to release resources
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                }
                // Dispose unmanaged resources if any

                disposed = true;
            }
        }

        ~SqlTools()
        {
            Dispose(false);
        }
    }
}
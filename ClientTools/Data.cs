using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Npgsql;

namespace ClientTools
{
    public static class Data
    {
        public static class SqlServerEngine
        {
            private static SqlConnectionStringBuilder SetConnectionInformation()
            {
                SqlConnectionStringBuilder connectionBuilder = new SqlConnectionStringBuilder();
                connectionBuilder.DataSource = DatabaseSettings.DataSource;
                connectionBuilder.InitialCatalog = DatabaseSettings.InitialCatalog;
                connectionBuilder.IntegratedSecurity = DatabaseSettings.IntegratedSecurity;
                connectionBuilder.UserID = DatabaseSettings.UserId;
                connectionBuilder.Password = DatabaseSettings.Password;
                connectionBuilder.ApplicationName = DatabaseSettings.ApplicationName;
                connectionBuilder.ConnectTimeout = DatabaseSettings.TimeOut;
                connectionBuilder.Pooling = DatabaseSettings.ConnectionPooling;
                          
                return connectionBuilder;
            }

            public static List<Dictionary<string, string>> GetData(SqlQuery sqlQuery)
            {
                // This method assumes a sql statement that contains named parameter place holders.
                List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
                try
                {
                    SqlConnectionStringBuilder connectionBuilder = SetConnectionInformation();
                    using (SqlConnection sqlConnection = new SqlConnection(connectionBuilder.ToString()))
                    {
                        using (SqlCommand sqlCommand = new SqlCommand())
                        {
                            sqlCommand.Connection = sqlConnection;
                            sqlCommand.CommandText = sqlQuery.Statement;
                            foreach (KeyValuePair<string, string> parameter in sqlQuery.Parameters)
                            {
                                sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                            sqlCommand.CommandType = CommandType.Text;

                            sqlConnection.Open();
                            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

                            if (sqlDataReader.HasRows)
                            {
                                Dictionary<int, string> schema = new Dictionary<int, string>();
                                for (int column = 0; column < sqlDataReader.FieldCount; column++)
                                {
                                    schema.Add(column, sqlDataReader.GetName(column).ToString());
                                }

                                while (sqlDataReader.Read())
                                {
                                    Dictionary<string, string> record = new Dictionary<string, string>();
                                    foreach (KeyValuePair<int, string> column in schema)
                                    {
                                        var fieldName = sqlDataReader.GetName(column.Key);
                                        var fieldValue = sqlDataReader[fieldName].ToString();
                                        record.Add(fieldName, fieldValue);
                                    }
                                    records.Add(record);
                                }
                            }
                        }
                    }
                    return records;
                }
                catch (SqlException sqlException)
                {
                    throw sqlException;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }

            public static class DatabaseSettings
            {
                public static string DataSource { get; set; }
                public static string InitialCatalog { get; set; }
                public static bool IntegratedSecurity { get; set; }
                public static string UserId { get; set; }
                public static string Password { get; set; }
                public static string ApplicationName { get; set; }
                public static int TimeOut { get; set; }
                public static bool ConnectionPooling { get; set; }
            }
        }
        public static class SQLiteEngine
        {
            private static SqliteConnectionStringBuilder SetConnectionInformation()
            {
                SqliteConnectionStringBuilder connectionBuilder = new SqliteConnectionStringBuilder();
                connectionBuilder.DataSource = DatabaseSettings.SQLitePath;
                connectionBuilder.Mode = SqliteOpenMode.ReadWriteCreate;
                return connectionBuilder;
            }

            public static List<Dictionary<string, string>> GetData(SqlQuery sqlQuery)
            {
                // This method assumes a sql statement that contains named parameter place holders.
                List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
                try
                {
                    SqliteConnectionStringBuilder connectionBuilder = SetConnectionInformation();
                    using (SqliteConnection sqlConnection = new SqliteConnection(connectionBuilder.ToString()))
                    {
                        using (SqliteCommand sqlCommand = new SqliteCommand())
                        {
                            sqlCommand.Connection = sqlConnection;
                            sqlCommand.CommandText = sqlQuery.Statement;
                            foreach (KeyValuePair<string, string> parameter in sqlQuery.Parameters)
                            {
                                sqlCommand.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }
                            sqlCommand.CommandType = CommandType.Text;

                            sqlConnection.Open();
                            SqliteDataReader reader = sqlCommand.ExecuteReader();

                            if (reader.HasRows)
                            {
                                Dictionary<int, string> schema = new Dictionary<int, string>();
                                for (int column = 0; column < reader.FieldCount; column++)
                                {
                                    schema.Add(column, reader.GetName(column).ToString());
                                }

                                while (reader.Read())
                                {
                                    Dictionary<string, string> record = new Dictionary<string, string>();
                                    foreach (KeyValuePair<int, string> column in schema)
                                    {
                                        var fieldName = reader.GetName(column.Key);
                                        var fieldValue = reader[fieldName].ToString();
                                        record.Add(fieldName, fieldValue);
                                    }
                                    records.Add(record);
                                }
                            }
                        }
                    }
                    return records;
                }
                catch (SqliteException sqlException)
                {
                    throw sqlException;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            public static class DatabaseSettings
            {
                public static string DatabaseFileName { get; set; }
                public static string SQLitePath { get; set; }
            }
        }
        public static class PostgreSQLEngine
        {
            private static NpgsqlConnectionStringBuilder SetConnectionInformation()
            {
                NpgsqlConnectionStringBuilder connectionBuilder = new Npgsql.NpgsqlConnectionStringBuilder();
                connectionBuilder.Host = DatabaseSettings.Host;
                connectionBuilder.Username = DatabaseSettings.UserName;
                connectionBuilder.Password = DatabaseSettings.Password;
                connectionBuilder.PersistSecurityInfo = DatabaseSettings.PersistSecuritInfo;
                connectionBuilder.SearchPath = DatabaseSettings.SearchPath;
                connectionBuilder.Timeout = DatabaseSettings.TimeOut;
                connectionBuilder.ApplicationName = DatabaseSettings.ApplicationName;
                connectionBuilder.IntegratedSecurity = DatabaseSettings.IntegratedSecurity;
                connectionBuilder.Port = DatabaseSettings.Port;
                connectionBuilder.Database = DatabaseSettings.DatabaseName;
                connectionBuilder.SslMode = (SslMode)DatabaseSettings.SSLMode;
                connectionBuilder.TrustServerCertificate = DatabaseSettings.TrustCertificate;
                return connectionBuilder;
            }

            public static List<Dictionary<string, string>> GetData(SqlQuery sqlQuery)
            {
                // This method assumes a sql statement that contains named parameter place holders.
                List<Dictionary<string, string>> records = new List<Dictionary<string, string>>();
                try
                {
                    NpgsqlConnectionStringBuilder connectionBuilder = SetConnectionInformation();
                    var connString = connectionBuilder.ToString();
                    using (NpgsqlConnection sqlConnection = new NpgsqlConnection(connectionBuilder.ToString()))
                    {
                        using (NpgsqlCommand sqlCommand = new NpgsqlCommand())
                        {
                            sqlCommand.Connection = sqlConnection;
                            sqlCommand.CommandText = sqlQuery.Statement;
                            foreach (KeyValuePair<string, string> parameters in sqlQuery.Parameters)
                            {
                                sqlCommand.Parameters.AddWithValue(parameters.Key, parameters.Value);
                            }
                            sqlCommand.CommandType = CommandType.Text;

                            sqlConnection.Open();
                            NpgsqlDataReader reader = sqlCommand.ExecuteReader();

                            if (reader.HasRows)
                            {
                                Dictionary<int, string> schema = new Dictionary<int, string>();
                                for (int column = 0; column < reader.FieldCount; column++)
                                {
                                    schema.Add(column, reader.GetName(column).ToString());
                                }

                                while (reader.Read())
                                {
                                    Dictionary<string, string> record = new Dictionary<string, string>();
                                    foreach (KeyValuePair<int, string> column in schema)
                                    {
                                        var fieldName = reader.GetName(column.Key);
                                        var fieldValue = reader[fieldName].ToString();
                                        record.Add(fieldName, fieldValue);
                                    }
                                    records.Add(record);
                                }
                            }
                        }
                    }
                    return records;
                }
                catch (PostgresException sqlException)
                {
                    throw sqlException;
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            public static class DatabaseSettings
            {
                public static string Host { get; set; }
                public static string UserName { get; set; }
                public static string DatabaseName { get; set; }
                public static int Port { get; set; }
                public static string Password { get; set; }
                public static bool PersistSecuritInfo { get; set; }
                public static bool IntegratedSecurity { get; set; }
                public static string SearchPath { get; set; }
                public static int TimeOut { get; set; }
                public static string ApplicationName { get; set; }
                public static int SSLMode { get; set; }
                public static bool TrustCertificate { get; set; }
            }
        }
    }
}

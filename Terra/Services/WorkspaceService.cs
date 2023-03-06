using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Diagnostics;

namespace Terra.Services
{
	public class WorkspaceService 
	{
        private readonly string _connectionString = $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Terra.db")}";

        public WorkspaceService() 
        {
            CreateWorkspaceTable();
            CreatePlantTable();
        }

        // test
        private void DropEm(string tableName)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();
            var sql = $"DROP TABLE {tableName}";
            using SqliteCommand command = new(sql, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }

        // test
        private void SholveIt(string tableName, string columnName, string note)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();
            var a = $"INSERT INTO {tableName} (name, date_added, note) " +
                                    "VALUES (@name, @date_added, @note)";
            using SqliteCommand command = new(a, connection);
            command.Parameters.AddWithValue("@name", columnName);
            command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
            command.Parameters.AddWithValue("@note", note);
            command.ExecuteNonQueryAsync();
        }

        // test
        private void SpitIt(string tableName, string column)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.Open();
            var sql = $"SELECT {column} FROM {tableName}";
            using SqliteCommand cmd = new(sql, connection);
            using SqliteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"SSS: {reader.GetValue(0)}");
            }
        }

        /// <summary>
        /// Used to verify if a table or a column is existed
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private bool IsExist(string tableName, string column, SqliteConnection connection)
        {
            /*using SqliteConnection connection = new(_connectionString);
            connection.Open();*/ //enable for debugging
            var sql = $"SELECT {column} FROM {tableName}";
            using SqliteCommand command = new(sql, connection);
            try
            {
                using SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read()) 
                {
                    Console.WriteLine("SSS: Found it");
                    return true; // found target with content
                }
                Console.WriteLine("SSS: Found it, no content");
                return true; // found target, no content
                     
            }catch(SqliteException ex)
            {
                Console.WriteLine($"SSS: {ex}");
                return false; // target not found
            }        
        }

        /// <summary>
        /// Create workspace table in Terra db
        /// </summary>
        private void CreateWorkspaceTable()
        {
            // open connection
            using (SqliteConnection connection = new(_connectionString))
            {
                connection.OpenAsync().Wait();
                // sql for creating table workspace
                string sql = $"CREATE TABLE Workspace (" +
                                "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                                "name TEXT NOT NULL UNIQUE," +
                                "date_added TEXT NOT NULL," +
                                "note TEXT)";
                // add workspace table to Terra database
                if (!IsExist("Workspace","*",connection))
                {
                    Console.WriteLine("YES: hahaha");
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close(); // close connection
            }
        }

        // create plant table. Invoke once, never used again
        /* para: tableName
         *  + Used for verifying table existence
         */
        private void CreatePlantTable()
        {
            // open connection
            using (SqliteConnection connection = new(_connectionString))
            {
                connection.OpenAsync().Wait();
                // sql for creating table plant
                string sql = $"CREATE TABLE Plant (" +
                                "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                                "name TEXT NOT NULL UNIQUE," +
                                "date_added TEXT NOT NULL," +
                                "note TEXT," +
                                "water_module INTEGER NOT NULL," +
                                "light_module INTEGER NOT NULL," +
                                "workspaceID INTEGER," +
                                "FOREIGN KEY (workspaceID) REFERENCES Workspace(id))";
                // add workspace table to Terra database
                if (!IsExist("Plant", "*", connection))
                {
                    using (SqliteCommand command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close(); // close connection
            }
        }


        /// <summary>
        /// Test this bitch. It might mess up
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="workspaceName"></param>
        /// <param name="note"></param>
        public void InsertToTable(string tableName, string workspaceName, string note)
        {
            using (SqliteConnection connection = new(_connectionString)) 
            {
                connection.OpenAsync().Wait();
                if (!IsExist(tableName, workspaceName, connection))
                {
                    var a = $"INSERT INTO {tableName} (name, date_added, note) " +
                                           "VALUES (@name, @date_added, @note)";

                    using (SqliteCommand command = new(a, connection))
                    {
                        command.Parameters.AddWithValue("@name", workspaceName);
                        command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@note", note);
                        command.ExecuteNonQueryAsync();
                    }
                    Console.WriteLine("SSS: Entry received");
                } 
            }
        }

        /*// validate table based on its name, using existing connection. Assuming connection != null and is open
        private bool ValidateTable(string tableName, SqliteConnection connection)
        {
            string request = $"SELECT * FROM {tableName}";
            try
            {
                using (SqliteCommand command = new(request, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Requesting TABLE");

                            return true;
                        }
                    }
                }
            }
            catch (SqliteException e)
            {
                return false;
            }
            return false;
        }*/

        /*// validate integer column and check for duplication. Assuming tableName is correct
        private static bool validateUniqueColumn(string tableName, string column, int value, SQLiteConnection connection)
        {
            string request = $"SELECT {column} FROM {tableName} WHERE {column} = {value}";

            try
            {
                using (SQLiteCommand command = new SQLiteCommand(request, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                            return true;
                    }
                }
            }
            catch (SQLiteException e)
            {
                Console.WriteLine("95: Nothing like dat");
            }
            return false;
        }*/


        //// test queryope. Assuming table name exists
        //private bool testQuery(string tableName, SQLiteConnection connection)
        //{
        //    string request = $"SELECT id FROM JavaFern WHERE id = 3";
        //    using (SQLiteCommand command = new SQLiteCommand(request, connection))
        //    {
        //        using (SQLiteDataReader reader = command.ExecuteReader())
        //        {
        //            if (reader.Read())
        //                return true;
        //        }
        //    }
        //    return false;
        //}
    }
}


using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

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

        // test. Put it in the constructor
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

        /// <summary>
        /// Create plant table in Terra db
        /// </summary>
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
        /// insert to table 
        /// </summary>
        /// <param name="tableName"> name of table (workspace)</param>
        /// <param name="workspaceName"> name column </param>
        /// <param name="note"> note column</param>
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

        /// <summary>
        /// Return a list of workspaces' name
        /// </summary>
        /// <param name="tableName"> name of table (Workspace) </param>
        /// <returns></returns>
        public List<string> GetWorkspaces(string tableName)
        {
            List<string> workspaces = new();

            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();
            if (IsExist(tableName, "*", connection))
            {
                var sql = $"SELECT * from {tableName}";
                using SqliteCommand command = new(sql, connection);
                using SqliteDataReader reader = command.ExecuteReader();

                // get available workspaces and add to list
                while (reader.Read())
                {
                    workspaces.Add(reader.GetValue(1).ToString());
                }
                // since number of workspaces is limited to 4. Check if the list is
                // populated under 4 workspaces. Fill out the remaining slots with N/A
                for (int i = 0; i <= (4 - workspaces.Count); i++)
                {
                    workspaces.Add("N/A");
                }
                return new List<string>(workspaces);
            }
            else
            {
                // this is for error handling purposes, since in WokrspaceList.xml, it uses the index of List to get value, possible OutOfBounce
                return (new List<string>() { "Nothing", "Nothing", "Nothing", "Nothing" });
            }
                
        }

        /// <summary>
        /// attempt to remove a specified element
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="workspaceName"></param>
        /// <returns> number of lines affected in sqlite </returns>
        public void RemoveWorkspace(string tableName, string workspaceName)
        {
            object[] items = new object[5];
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();
            var sql = "SELECT rowid FROM Workspace";
            using SqliteCommand command = new(sql, connection);

            using SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                string name = reader["name"].ToString();
                int id = Convert.ToInt32(reader["id"]);
                Console.WriteLine($"Line 229:{_connectionString}");
            }
            
            
            
        }
    }
}


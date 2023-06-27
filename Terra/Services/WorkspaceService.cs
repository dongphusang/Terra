// Terra sqlite3 service. Not just workspace

using System;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Diagnostics;
using System.Collections.ObjectModel;

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
                Console.WriteLine($"SpitIt() {tableName}: {reader.GetValue(0)}");
            }
        }

        /// <summary>
        /// Verify if a table or column exists
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
                if (reader.Read()) return true; // found target table with content  
                else               return true; // found target table, no content                 
            }catch(SqliteException ex)
            {
                return false; // target not found
            }        
        }

        /// <summary>
        /// Verify if a cell exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="cellValue"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        private bool IsExist(string tableName, string column, string cellValue, SqliteConnection connection)
        {
            var sql = $"SELECT {column} FROM {tableName} WHERE {column} = @cellValue";
            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@cellValue", cellValue);
            try
            {
                using SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Console.WriteLine($"IsExist() {tableName}: Found it");
                    return true; // found target table, with content
                }
                Console.WriteLine($"IsExist() {tableName}: Found it, no content");
                return false; // found target table, no such cell

            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"IsExist() {tableName}: {ex}");
                return true; // target table not found, hence no such cell
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
                                "from_workspace TEXT NOT NULL)";
                // add workspace table to Terra database
                if (IsExist("Plant", "*", connection) is false)
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
        /// Open connection and add workspace entry to database.
        /// </summary>
        /// <param name="tableName"> name of table (workspace)</param>
        /// <param name="workspaceName"> name column </param>
        /// <param name="note"> note column</param>
        public void InsertToWorkspaceTable(string workspaceName, string note)
        {
            var table = "Workspace";
            var column = "name";
            using (SqliteConnection connection = new(_connectionString)) 
            {
                connection.OpenAsync().Wait();
                if (IsExist(table, column, workspaceName, connection) is false)
                {
                    var a = $"INSERT INTO Workspace (name, date_added, note) " +
                                           "VALUES (@name, @date_added, @note)";

                    using (SqliteCommand command = new(a, connection))
                    {
                        command.Parameters.AddWithValue("@name", workspaceName);
                        command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@note", note);
                        command.ExecuteNonQueryAsync();
                    }
                } 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="workspaceName"></param>
        /// <param name="plantName"></param>
        /// <param name="note"></param>
        public void InsertToPlantTable(string workspaceName, string plantName, string note)
        {
            var table = "Plant";
            var column = "name";
            Console.WriteLine($"InsertToPlantTable(): {workspaceName} {plantName} {note}");
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            if (IsExist(table, column, plantName, connection) is false)
            {
                var sql = $"INSERT INTO Plant (name, date_added, note, from_workspace) " +
                                         "VALUES (@name, @date_added, @note, @from_workspace)";

                using SqliteCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@name", plantName);
                command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                command.Parameters.AddWithValue("@note", note);
                command.Parameters.AddWithValue("@from_workspace", workspaceName);
                command.ExecuteNonQueryAsync();
            }
        }

        // return object that contains number of cells in a column
        public Task<object> CountColumnValues(string workspaceName)
        {
            Console.WriteLine($"CountColumnValues(): {workspaceName}");
            // open connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            var sql = $"SELECT COUNT(from_workspace) FROM Plant WHERE from_workspace = @nameValue";
            using SqliteCommand cmd = new(sql, connection);
            cmd.Parameters.AddWithValue("@nameValue", workspaceName);

            return cmd.ExecuteScalarAsync();
        }

        // retrieve plant entry from a workspace. Works for now since only one plant allowed per workspace.
        public Task<object> GetPlantName(string currentWorkspaceName)
        {               
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            var sql = $"SELECT name FROM Plant WHERE from_workspace = @nameValue";
            using SqliteCommand cmd = new(sql, connection);
            cmd.Parameters.AddWithValue("@nameValue", currentWorkspaceName);

            return cmd.ExecuteScalarAsync();
        }

        /// <summary>
        /// Return a list of workspaces' name and note
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
                    workspaces.Add(reader.GetValue(1).ToString()); // name
                    workspaces.Add(reader.GetValue(3).ToString()); // note
                }              
                return new List<string>(workspaces);
            }
            else
            {
                // this is for error handling purposes, since in WokrspaceList.xml, it uses the index of List to get value, possible OutOfBounce
                return (new List<string>() { "NA", "NA", "NA", "NA", "NA", "NA", "NA", "NA" });
            }                
        }

        // get number of current workspaces in Workspace table
        public Task<object> GetNumberofWorkspaces()
        {
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            var sql = "SELECT COUNT(name) FROM Workspace";
            using SqliteCommand cmd = new(sql, connection);

            return cmd.ExecuteScalarAsync();
        }


        public void DeleteWorkspace(string workspaceName)
        {
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = "DELETE FROM Workspace WHERE name = @nameValue";
            using SqliteCommand cmd = new(sql, connection);
            cmd.Parameters.AddWithValue("@nameValue", workspaceName);

            // if name isn't an empty string, remove that instance from available list along with plant members
            if (workspaceName != null)
            {
                cmd.ExecuteNonQuery();
                DeletePlant(connection, workspaceName);
            }
        }

        private Task DeletePlant(SqliteConnection connection, string workspaceName)
        {
            var sql = "DELETE FROM Plant WHERE from_workspace = @nameValue";
            using SqliteCommand cmd = new(sql, connection);
            cmd.Parameters.AddWithValue("@nameValue", workspaceName);

            return cmd.ExecuteNonQueryAsync();
        }
    }
}


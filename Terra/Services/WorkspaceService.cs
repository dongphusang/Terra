// read and write access to Terra db. Responsible for Plant and Workspace

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
                else return true; // found target table, no content                 
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"IsExist() {tableName}: {ex}");
                return false; // target not found
            }
        }

        /// <summary>
        /// Verify if a cell exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="column"></param>
        /// <param name="cellValue"> User provided. </param>
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
                if (reader.Read()) return true;  // found target table, with content
                else return false; // found target table, no such cell
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"IsExist() {tableName}: {ex}");
                return true; // target table not found, hence no such cell
            }
        }

        /// <summary>
        /// Open connection and create Workspace table in db.
        /// </summary>
        private void CreateWorkspaceTable()
        {
            // open connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // sql for creating table workspace
            string sql = $"CREATE TABLE Workspace (" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                            "name TEXT NOT NULL UNIQUE," +
                            "date_added TEXT NOT NULL," +
                            "note TEXT)";
            // add workspace table to Terra database
            if (IsExist("Workspace","*",connection) is false)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Create plant table in Terra db
        /// </summary>
        private void CreatePlantTable()
        {
            // open connection
            using SqliteConnection connection = new(_connectionString);
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
        }

        /// <summary>
        /// Open connection and add workspace entry to table.
        /// </summary>
        /// <param name="workspaceName"> name of workspace </param>
        /// <param name="note"> note of workspace </param>
        /// <returns> Return true if workspace is added. Return false otherwise. </returns>
        public bool InsertToWorkspaceTable(string workspaceName, string note)
        {
            var table = "Workspace";
            var column = "name";

            // init connection to db
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var a = $"INSERT INTO Workspace (name, date_added, note) " +
                                        "VALUES (@name, @date_added, @note)";

            // add member if it doesn't exist
            if (IsExist(table, column, workspaceName, connection) is false)
            {
                using SqliteCommand command = new(a, connection);
                command.Parameters.AddWithValue("@name", workspaceName);
                command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                command.Parameters.AddWithValue("@note", note);

                command.ExecuteNonQuery();

                return true; // affirmative that member is added 
            }

            return false; // affirmative that member already existed
        }

        /// <summary>
        /// Open connection and add email entry to table.
        /// </summary>
        /// <param name="workspaceName"></param>
        /// <param name="plantName"></param>
        /// <param name="note"></param>
        /// <returns> Return true if email is added. Return false otherwise. </returns>
        public bool InsertToPlantTable(string workspaceName, string plantName, string note)
        {
            var table = "Plant";
            var column = "name";

            // init connection to db
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = $"INSERT INTO Plant (name, date_added, note, from_workspace) " +
                                         "VALUES (@name, @date_added, @note, @from_workspace)";

            // add member if it doesn't exist
            if (IsExist(table, column, plantName, connection) is false)
            {
                using SqliteCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@name", plantName);
                command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                command.Parameters.AddWithValue("@note", note);
                command.Parameters.AddWithValue("@from_workspace", workspaceName);

                command.ExecuteNonQuery();

                return true; // affirmative that member is added
            }

            return false; // affirmative that member already existed
        }

        // return object that contains number of cells in a column
        public Task<object> CountColumnValues(string workspaceName)
        {
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
            // open connection
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

            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = $"SELECT * from {tableName}";

            // get members if table exists
            if (IsExist(tableName, "*", connection))
            {
                using SqliteCommand command = new(sql, connection);
                using SqliteDataReader reader = command.ExecuteReader();

                // get available workspaces and add to list
                while (reader.Read())
                {
                    workspaces.Add(reader.GetValue(1).ToString()); // name
                    workspaces.Add(reader.GetValue(3).ToString()); // note
                }              

                return new List<string>(workspaces); // values found
            }
            else
            {
                // this is for error handling purposes, since in WokrspaceList.xml, it uses the index of List to get value, possible OutOfBounce
                return new List<string>() { "NA", "NA" }; // no names and notes were found
            }                
        }

        // get number of current workspaces in Workspace table
        public Task<object> GetNumberofWorkspaces()
        {
            // open connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = "SELECT COUNT(name) FROM Workspace";
            using SqliteCommand cmd = new(sql, connection);

            return cmd.ExecuteScalarAsync();
        }

        /// <summary>
        /// Remove workspace from table.
        /// </summary>
        /// <param name="workspaceName"> name of workspace (not user provided). </param>
        /// <returns> Return true if workspace is removed. Return false otherwise. </returns>
        public bool DeleteWorkspace(string workspaceName)
        {
            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = "DELETE FROM Workspace WHERE name = @nameValue";

            // if name isn't an empty string, remove that instance from available list along with plant members
            if (workspaceName != null)
            {
                using SqliteCommand cmd = new(sql, connection);
                cmd.Parameters.AddWithValue("@nameValue", workspaceName);

                cmd.ExecuteNonQuery();

                DeletePlant(connection, workspaceName); // remove plant members

                return true; // workspace and its plants are removed
            }

            return false; // nothing was removed
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


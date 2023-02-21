using System;
using System.Data.SQLite;

namespace Terra.Services
{
	public class WorkspaceService
	{
		private static readonly string _connectionString = "Data Source=Terra.db";
        
        // insert new entry. 
        // everything is auto populated except for tableName and tankNumber
        public static bool InsertToTable(string tableName, string workspaceName, string note)
        {
            using (SQLiteConnection connection = new SQLiteConnection(_connectionString))
            {
                connection.OpenAsync();
                if (validateTable(tableName, connection))
                {
                    string sql = $"INSERT INTO {tableName} (name, date_added, note) " +
                                               "VALUES (@name, @date_added, @note)";

                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", workspaceName);
                        command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                        command.Parameters.AddWithValue("@note", note);
                        command.ExecuteNonQueryAsync();
                    }
                    Console.WriteLine("Entry received");
                    
                    return true;
                }

                connection.CloseAsync();
                return false;
            }
        }

        // validate table based on its name, using existing connection. Assuming connection != null and is open
        private static bool validateTable(string tableName, SQLiteConnection connection)
        {
            string request = $"SELECT * FROM {tableName}";

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
                return false;
            }
            return false;
        }

        // validate integer column and check for duplication. Assuming tableName is correct
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
        }

        public async static Task LoadMauiAsset()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("Terra.db");
            using var reader = new StreamReader(stream);

            var contents = reader.ReadToEnd();
        }

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


// read and write access to Terra db. Responsible for emai list

using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Services
{    
    public class EmailListDBService
    {
        private readonly string _connectionString = $"Data Source={Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Terra.db")}";

        public EmailListDBService()
        {
            CreateEmailTable();
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
                else               return false; // found target table, no such cell
            }
            catch (SqliteException ex)
            {
                Console.WriteLine($"IsExist() {tableName}: {ex}");
                return true; // target table not found, hence no such cell
            }
        }

        /// <summary>
        /// Open connection and create EmailTable in db.
        /// </summary>
        private void CreateEmailTable()
        {
            // open connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // sql for creating table workspace
            string sql = $"CREATE TABLE EmailTable (" +
                            "id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL UNIQUE," +
                            "email TEXT NOT NULL UNIQUE," +
                            "date_added TEXT NOT NULL," +
                            "note TEXT)";

            // add EmailTable to Terra database
            if (IsExist("EmailTable", "*", connection) is false)
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Open connection and add email entry to table.
        /// </summary>
        /// <param name="emailName"> name of email </param>
        /// <param name="note"> note of email </param>
        /// <returns> Return true if email is added. Return false otherwise.</returns>
        public bool PostToEmailTable(string emailName, string note)
        {
            var table = "EmailTable";
            var column = "name";

            // init connection to db
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = $"INSERT INTO EmailTable (email, date_added, note) " +
                                           "VALUES (@email, @date_added, @note)";

            // add member if it doesn't exist
            if (IsExist(table, column, emailName, connection) is false)
            {
                using SqliteCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@email", emailName);
                command.Parameters.AddWithValue("@date_added", DateTime.Now.ToString());
                command.Parameters.AddWithValue("@note", note);

                command.ExecuteNonQuery();

                return true; // affirmative that member is added
            }

            return false; // affirmative that member already existed
        }

        /// <summary>
        /// Get List of emails from table.
        /// </summary>
        /// <returns> Return List of emails from table. Return empty List if table not found </returns>
        public List<string> GetFromEmailTable()
        {
            var table = "EmailTable";
            var emails = new List<string>();

            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            //construct sql
            var sql = $"SELECT * from EmailTable";

            // get members if table exists
            if (IsExist(table, "*", connection))
            {
                using SqliteCommand command = new(sql, connection);
                using SqliteDataReader reader = command.ExecuteReader();
                
                // get available emails
                while (reader.Read())
                {
                    emails.Add(reader.GetValue(1).ToString());
                }

                return new List<string>(emails); // emails found
            }

            return new List<string>(); // no emails found in table
        }

        /// <summary>
        /// Remove email from table.
        /// </summary>
        /// <param name="emailName"> name of email (user provided by clicking). </param>
        /// <returns> Return true if email is removed. Return false otherwise. </returns>
        public bool DeleteEmail(string emailName)
        {
            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = "DELETE FROM EmailTable WHERE name = @emailName";

            // run removal sql if the name provided isn't null
            if (emailName is not null)
            {
                using SqliteCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@emailName", emailName);

                command.ExecuteNonQuery();

                return true; // email removed from table
            }

            return false; // no email was removed
        }

    }
}

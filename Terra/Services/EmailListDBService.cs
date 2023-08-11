// read and write access to Terra db. Responsible for email list

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
                            "email TEXT NOT NULL UNIQUE," + // full email format
                            "user TEXT NOT NULL UNIQUE," +  // email after stripped of @___.com
                            "bucket TEXT)";          // influx db bucket name, representing a plant database (for now)

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
        /// <param name="mail"> name of email (user provided). </param>
        /// <returns> Return true if email is added. Return false otherwise.</returns>
        public bool PostToEmailTable(string mail)
        {
            var table = "EmailTable";
            var column = "email";

            // init connection to db
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = $"INSERT INTO EmailTable (email, user) " +
                                           "VALUES (@email, @user)";

            // add member if it doesn't exist
            if (IsExist(table, column, mail, connection) is false)
            {
                using SqliteCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@email", mail);
                command.Parameters.AddWithValue("@user", mail[..mail.IndexOf("@")]);

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
                    emails.Add(reader.GetValue(0).ToString());
                }

                return new List<string>(emails.Distinct().ToList()); // emails found
            }

            return new List<string>(); // no emails found in table
        }

        /// <summary>
        /// Remove email from table.
        /// </summary>
        /// <param name="email"> name of email (user provided by clicking). </param>
        /// <returns> Return true if email is removed. Return false otherwise. </returns>
        public bool DeleteEmail(string emailAddress)
        {
            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = "DELETE FROM EmailTable WHERE email = @email_address";

            // run removal sql if the name provided isn't null
            if (emailAddress is not null && IsExist("EmailTable", "email", emailAddress, connection))
            {
                using SqliteCommand command = new(sql, connection);
                command.Parameters.AddWithValue("@email_address", emailAddress);

                command.ExecuteNonQuery();

                return true; // email removed from table
            }

            return false; // no email was removed
        }

        // take an active email, return associated buckets of that email
        // STATUS: DORMANT method (for future use)
        /*public Task<object> GetBucketsForMail(string email_address)
        {
            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = "SELECT bucket FROM EmailTable WHERE email = @email_address";

            // retrieve buckets
            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@email_address", email_address);

            return command.ExecuteScalarAsync();
        }*/

        // associate subscribed plant to the email, saving it in sqlite db
        // STATUS: DORMANT method (for future use)
        /*public async Task SaveBucketsForMail(string email_address, string plant)
        {
            // init connection
            using SqliteConnection connection = new(_connectionString);
            connection.OpenAsync().Wait();

            // construct sql
            var sql = $"UPDATE EmailTable " +
                        "SET bucket = @buckets " +
                        "WHERE email = @email_address";

            // modify entry of target email address, saving buckets associated with the email
            using SqliteCommand command = new(sql, connection);
            command.Parameters.AddWithValue("@buckets", plant);
            command.Parameters.AddWithValue("@email_address", email_address);

            await command.ExecuteScalarAsync();
        }*/



    }
}

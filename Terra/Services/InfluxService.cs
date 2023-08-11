
using InfluxDB.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Terra.Services
{
    public class InfluxService
    {
        private IConfigurationRoot _config; // configuration object
        private InfluxDBClient _client;

        List<string> records; // storing results from InfluxDB query

        // connection attributes
        readonly string BUCKET;
        readonly string ORG;
        readonly string TOKEN;
        readonly string URL;

        public InfluxService()
        {
            _config = GetConfiguration(); // establish config file
            
            // get info from config files for connection to InfluxDB
            BUCKET = _config.GetSection("InfluxDB:BUCKET").Value;
            TOKEN = _config.GetSection("InfluxDB:READ_TOKEN").Value;
            ORG = _config.GetSection("InfluxDB:ORG").Value;
            URL = _config.GetSection("InfluxDB:URL").Value;

            _client = new InfluxDBClient(URL, TOKEN); // establish InfluxDB client
        }

        /// <summary>
        /// Establish configuration file.
        /// </summary>
        /// <returns> configuration builder </returns>
        private static IConfigurationRoot GetConfiguration()
        {        
            Assembly assembly = Assembly.GetExecutingAssembly(); // get current assembly
            using Stream stream = assembly.GetManifestResourceStream("Terra.appconfig.json"); 
            using StreamReader reader = new(stream);
            var json = reader.ReadToEnd();
            var builder = new ConfigurationBuilder().AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json))).Build();
            
            return builder;
        }

        /// <summary>
        /// Query data frame from InfluxDB from the last minute.
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetData()
        {
            records = new();
            var flux = $"from(bucket:\"{BUCKET}\") |> range(start: -1m)";
            var fluxTables = await _client.GetQueryApi().QueryAsync(flux, ORG);
            foreach (var fluxTable in fluxTables)
            {
                var fluxRecords = fluxTable.Records;
                foreach (var record in fluxRecords)
                {
                    records.Add(record.GetValue().ToString());
                }
            }
            return records[^1];           
        }

    }
}

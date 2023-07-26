using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadNetwork;
using System.Reflection;
using System.Net.Http;

namespace Terra.Services
{
    class PlantAPIService
    {
        private IConfigurationRoot _config; // configuration object
        private HttpClient _httpClient;
        private HttpResponseMessage _response;

        List<string> result; // result query

        // connection attributes
        readonly string KEY;

        public PlantAPIService()
        {
            _config = GetConfiguration(); // establish config file
            _httpClient = new HttpClient();

            // get info from config files for connection to InfluxDB
            KEY = _config.GetSection("PlantAPI:KEY").Value;
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


        public async Task ProcessRepositoriesAsync(HttpClient client)
        {
            result = _httpClient.GetAsync()
        }
    }
}

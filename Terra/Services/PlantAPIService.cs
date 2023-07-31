using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Nodes;
using Terra.Models;

namespace Terra.Services
{
    class PlantAPIService
    {
        private IConfigurationRoot _config; // configuration object
        private HttpClient _httpClient;
        // connection attributes
        readonly string KEY;
        const string URL = "https://perenual.com/api/species-list";

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

        /// <summary>
        /// Get plant names from API based user's input
        /// </summary>
        /// <param name="plantName"> Name of plant. </param>
        /// <returns> Dictionary on plant's information. </returns>
        public async Task<List<string>> GetPlantOptions(string query)
        {
            
            // construct URI
            var uri = new Uri(URL + "?key=" +KEY + "&q=" + query);
            // get resource from api
            HttpResponseMessage result = _httpClient.GetAsync(uri).Result;
            // if succeeded
            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<List<APIPlant>>(JObject.Parse(response)["data"].ToString());
                var filteredList = deserialized.Select(apiPlant => apiPlant.Common_name).ToList();

                return new List<string>(filteredList);
            }

            return new List<string>() { "empty"};
        }
    }
}

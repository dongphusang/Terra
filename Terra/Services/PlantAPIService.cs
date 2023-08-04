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
using System.Net.Sockets;

namespace Terra.Services
{
    class PlantAPIService
    {
        private IConfigurationRoot _config; // configuration object
        private HttpClient _httpClient;
        // connection attributes
        readonly string KEY;
        const string URL = "https://perenual.com/api/species-list";
        const string SPECIEDEET_URL = "https://perenual.com/api/species/details/";

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
                // read response as string
                var response = await result.Content.ReadAsStringAsync();

                var deserializedList = new List<APIPlant>(); // declare deserialized json var
                var filteredList = new List<string>();   // declare a filtered list from json content

                try
                {
                    // deserialize json
                    deserializedList = JsonConvert.DeserializeObject<List<APIPlant>>(JObject.Parse(response)["data"].ToString());
                    // filter content and convert that into a List
                    filteredList = deserializedList.Select(apiPlant => apiPlant.Common_name).Distinct().ToList();
                }
                catch (JsonSerializationException ex) // this happens when user's entry belongs to paid content (currently using free tier api)
                {
                    return new List<string>();
                }
                return new List<string>(filteredList);
            }

            return new List<string>();
        }

        public async Task<APIPlant> GetPlantDetails(int plantID)
        {
            // construct URI
            var uri = new Uri(SPECIEDEET_URL + plantID + "?key=" + KEY);
            // get resource from api
            HttpResponseMessage result = _httpClient.GetAsync(uri).Result;

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var deserializedModel = JsonConvert.DeserializeObject<APIPlant>(JObject.Parse(response).ToString());

                return deserializedModel;
            }
            return new APIPlant();
        }

        public async Task<int> GetPlantID(string plantName)
        {
            // construct URI
            var uri = new Uri(URL + "?key=" + KEY + "&q=" + plantName);
            // get resource from api
            HttpResponseMessage result = _httpClient.GetAsync(uri).Result;

            if (result.IsSuccessStatusCode)
            {
                var response = await result.Content.ReadAsStringAsync();
                var deserializedModel = JsonConvert.DeserializeObject<List<APIPlant>>(JObject.Parse(response)["data"].ToString())[0];

                return deserializedModel.Id;
            }
            return -1;
        }
    }
}

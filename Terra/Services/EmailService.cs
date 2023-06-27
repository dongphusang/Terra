using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Terra.Services
{
    public class EmailService
    {
        private IConfigurationRoot _config; // configuration object
        private MailMessage _email;

        // connection attributes
        readonly string API_KEY;

        public EmailService()
        {
            _config = GetConfiguration();

            API_KEY = _config.GetSection("MailChimp:API_KEY").Value;
            
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
    }
}

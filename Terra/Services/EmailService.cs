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
        private SmtpClient _smtpClient;

        readonly string SENDER;
        readonly string SENDER_PASS;
        readonly int PORT;
        readonly string SERVER;

        public EmailService()
        {
            _config = GetConfiguration();

            SENDER      = _config.GetSection("SMTP:ROOT").Value;
            SENDER_PASS = _config.GetSection("SMTP:ROOT_PASSWORD").Value;
            PORT        = int.Parse(_config.GetSection("SMTP:PORT").Value);
            SERVER      = _config.GetSection("SMTP:SMTP_SERVER").Value;

            _email = new();
            _smtpClient = new(SERVER, PORT);
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


        public void Send(List<string> emails)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            _email.From = new MailAddress(SENDER);

            foreach (var recipient in emails)
            {
                _email.To.Add(recipient);
            }
            _email.Subject = "This is a test";
            _email.Body = "You passed! Now put an html body";

            _smtpClient.Credentials = new NetworkCredential(SENDER, SENDER_PASS);
            _smtpClient.EnableSsl = true;
            _smtpClient.Send(_email);

            _smtpClient.Dispose();
        }
    }
}

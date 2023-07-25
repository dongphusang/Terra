using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Terra.Models;
using Terra.Services;

namespace Terra.ViewModels
{

    public partial class EmailSubViewModel : ObservableObject
    {
        // models
        [ObservableProperty]
        public TerraEmail emailModel;

        // services
        private EmailService _emailService;
        private EmailListDBService _emailListDBService;

        // email list
        [ObservableProperty]
        public List<string> emails;

        // email selected for removal
        [ObservableProperty]
        public string selectedEmail;

        public EmailSubViewModel()
        {
            Emails = new();
            EmailModel = new();
            _emailService = new();
            _emailListDBService = new();
        }

        /// <summary>
        /// Retrieve mails from sqlite database and update view
        /// </summary>
        public void UpdateEmails()
        {
            Emails = _emailListDBService.GetFromEmailTable();
        }

        /// <summary>
        /// Add mail to database and update view
        /// </summary>
        [RelayCommand]
        public void AddEmail()
        {           
            if (EmailModel.Email is not null)
            {
                if (EmailModel.Email.Trim() is not "")
                {
                    _emailListDBService.PostToEmailTable(EmailModel.Email);
                    UpdateEmails();
                }                
            }        
        }

        /// <summary>
        /// Remove mail from database and update view
        /// </summary>
        /// <param name="email"> Mail to be removed. </param>
        [RelayCommand]
        public void DeleteEmail(string email)
        {
            _emailListDBService.DeleteEmail(email);
            UpdateEmails();
        }

    }
}

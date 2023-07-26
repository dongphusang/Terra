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
        private WorkspaceService _workspaceService;

        // email list
        [ObservableProperty]
        public List<string> emails; // emails that are available
        [ObservableProperty]
        public ObservableCollection<string> activeEmails; // emails that subscribe for a particular plant, receiving its information

        // email selected for removal
        [ObservableProperty]
        public string selectedEmail;

        // plant and workspace name
        [ObservableProperty]
        public string currentWorkspaceName;
        [ObservableProperty]
        public string currentPlantName;

        public EmailSubViewModel()
        {
            Emails = new();
            ActiveEmails = new();
            EmailModel = new();
            _emailService = new();
            _emailListDBService = new();
            _workspaceService = new();

            CurrentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            CurrentPlantName = Unwrap(Task.Run(() => _workspaceService.GetPlantName(CurrentWorkspaceName)));
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

        // activate subscription for one email
        [RelayCommand]
        public void ModifySubscription()
        {
            if (ActiveEmails.Contains(SelectedEmail))
            {
                ActiveEmails.Remove(SelectedEmail);
                SelectedEmail = null;
            }
            else
            {
                ActiveEmails.Add(SelectedEmail);
                SelectedEmail = null;
            }
        }

        // check if object returned from Task.Run() is null. Return non-null value. Usually used for sqlite operations
        private string Unwrap(Task<object> obj)
        {
            var result = obj.Result;
            if (result is null)
            {
                return "N/A";
            }
            return result.ToString();
        }

    }
}

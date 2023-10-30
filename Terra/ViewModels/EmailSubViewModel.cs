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
using Terra.TerraConstants;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using System.ComponentModel;

namespace Terra.ViewModels
{

    public partial class EmailSubViewModel : ObservableObject
    {
        // models
        [ObservableProperty]
        public TerraEmail emailModel;

        // services
        private EmailListDBService _emailListDBService; // add and remove emails from sqlitedb
        private WorkspaceService _workspaceService;     // get workspace associated with current plant
        private FirestoreService _firestoreService;     // manipulate email list on firestore

        // email list
        [ObservableProperty]
        public List<string> emails; // email pool
        [ObservableProperty]
        public ObservableCollection<string> activeEmails; // emails that subscribed for a particular plant

        // email selected in collection view
        [ObservableProperty]
        public string selectedEmail;

        // currently viewing workspace, plant, mcu
        private string _currentWorkspaceName;
        private string _currentPlantName;
        private string _currentMCU;       


        public EmailSubViewModel()
        {
            Emails = new();
            ActiveEmails = new();
            EmailModel = new();
            _emailListDBService = new();
            _workspaceService = new();
            _firestoreService = new();
          
            GetWorkspaceDetails();
            Console.WriteLine($"SANG: {_currentWorkspaceName}, {_currentPlantName}, {_currentMCU}");
        }

        /// <summary>
        /// Get plant name and microcontroller relevant to a workspace.
        /// </summary>
        /// <returns></returns>
        private Task GetWorkspaceDetails()
        {
            _currentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            _currentPlantName = (string) _workspaceService.GetPlantName(_currentWorkspaceName).Result;
            _currentMCU = (string) _workspaceService.GetWorkspaceMCU(_currentWorkspaceName).Result;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieve mails from sqlite database and update view
        /// </summary>
        public async Task UpdateEmails()
        {
            Emails = _emailListDBService.GetFromEmailTable();

            ConvertToDisplayableList(await _firestoreService.GetValues(_currentPlantName, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS));
        }

        /// <summary>
        /// Add mail to database, update view, and upload mail to firestore
        /// </summary>
        [RelayCommand]
        public Task AddEmail()
        {           
            if (EmailModel.Email is not null)
            {
                if (EmailModel.Email.Trim() is not "")
                {
                    _emailListDBService.PostToEmailTable(EmailModel.Email);
                    Task.Run(UpdateEmails);
                    return _firestoreService.PostMerge(FormatForFirestore(EmailModel.Email), "Terra", FirestoreConstant.SUBSCRIPTION, FirestoreConstant.INACTIVE_EMAILS);
                }                
            }        
            return Task.CompletedTask;
        }

        /// <summary>
        /// Add email to local active email list for subscribing to a plant.
        /// </summary>
        [RelayCommand]
        public Task ModifySubscription()
        {
            if (ActiveEmails.Contains(SelectedEmail) && SelectedEmail is not null)
            {
                ActiveEmails.Remove(SelectedEmail);
                SelectedEmail = null;
            }
            else if (ActiveEmails.Contains(SelectedEmail) is false && SelectedEmail is not null)
            {
                ActiveEmails.Add(SelectedEmail);
                SelectedEmail = null;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Change email status between active and inactive. 
        /// Active meaning the email subscribes to a plant, and pushed to "active" document;
        /// Inactive meaning the email not subscribe to a plant, and pushed to "inactive" document.
        /// </summary>
        [RelayCommand]
        public Task CommitSubscriptions()
        {
            // add email to active document in firestore
            if (ActiveEmails.Count > 0) 
                return ActivateSubscription();
            // remove subscription if no emails are subscribed to current plant
            else 
                return _firestoreService.Remove(_currentPlantName, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);
        }

        /// <summary>
        /// Remove email from database, update view, and remove mail from firestore
        /// [Used: EmailSubPage -> remove button]
        /// </summary>
        /// <param name="email"> Mail to be removed. </param>
        [RelayCommand]
        public Task DeleteEmail(string email)
        {
            _emailListDBService.DeleteEmail(email);
            Task.Run(UpdateEmails);

            return _firestoreService.RemoveFromParentCollection(email, FormatForFirestore(email));
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

        // strip off "@gmail.com" from firestore operations
        private string FormatForFirestore(string target)
        {
            return target[..target.IndexOf('@')];
        }

        // copy List<object>'s content to List<string> for UI display (List<object> can't display strings on UI)
        private void ConvertToDisplayableList(List<object> usernames)
        {
            ActiveEmails = new();
            for (int i = 0; i < usernames.Count; i++) 
            {
                ActiveEmails.Add(usernames[i].ToString());
            }
        }

        // add subscribing section for current plant in Active collection
        private Task ActivateSubscription()
        {
            _firestoreService.PostMerge(_currentPlantName, ActiveEmails, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);

            return Task.CompletedTask;
        }
    }
}

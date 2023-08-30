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

namespace Terra.ViewModels
{

    public partial class EmailSubViewModel : ObservableObject
    {
        // models
        [ObservableProperty]
        public TerraEmail emailModel;

        // services
        private EmailListDBService _emailListDBService;
        private WorkspaceService _workspaceService;
        private FirestoreService _firestoreService;

        // email list
        [ObservableProperty]
        public List<string> emails; // emails that haven't subscribed to anything
        [ObservableProperty]
        public ObservableCollection<string> activeEmails; // emails that subscribe for a particular plant

        // email selected in collection view
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
            _emailListDBService = new();
            _workspaceService = new();
            _firestoreService = new();
            
            CurrentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            CurrentPlantName = Unwrap(Task.Run(() => _workspaceService.GetPlantName(CurrentWorkspaceName)));
        }

        /// <summary>
        /// Retrieve mails from sqlite database and update view
        /// </summary>
        public async void UpdateEmails()
        {
            Emails = _emailListDBService.GetFromEmailTable();
            ConvertToDisplayableList(await _firestoreService.GetValue(CurrentPlantName, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS));
        }

        /// <summary>
        /// Add mail to database, update view, and upload mail to firestore
        /// [Used: EmailSubPage -> add button]
        /// </summary>
        [RelayCommand]
        public Task AddEmail()
        {           
            if (EmailModel.Email is not null)
            {
                if (EmailModel.Email.Trim() is not "")
                {
                    _emailListDBService.PostToEmailTable(EmailModel.Email);
                    UpdateEmails();
                    return _firestoreService.Post(FormatForFirestore(EmailModel.Email), "Terra", FirestoreConstant.SUBSCRIPTION, FirestoreConstant.INACTIVE_EMAILS);
                }                
            }        
            return Task.CompletedTask;
        }

        /// <summary>
        /// Add email to local active email list for subscribing to a plantm
        /// [Used: EmailSubPage -> "Email List" collection view]
        /// </summary>
        [RelayCommand]
        public void ModifySubscription()
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
        }

        /// <summary>
        /// Change email status between active and inactive. 
        /// Active meaning the email subscribes to a plant.
        /// Inactive meaning the email not subscribe to a plant.
        /// </summary>
        [RelayCommand]
        public async Task CommitSubscriptions()
        {
            // add email to active document in firestore
            if (ActiveEmails.Count > 0) await ActivateSubscription();
            // remove subscription if no emails are subscribed to current plant
            else await _firestoreService.Remove(CurrentPlantName, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);
        }

        /// <summary>
        /// Remove email from database, update view, and remove mail from firestore
        /// [Used: EmailSubPage -> remove button]
        /// </summary>
        /// <param name="email"> Mail to be removed. </param>
        [RelayCommand]
        public async Task DeleteEmail(string email)
        {
            _emailListDBService.DeleteEmail(email);
            UpdateEmails();

            await _firestoreService.RemoveFromParentCollection(email, FormatForFirestore(email));
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
            //_firestoreService.Remove(FormatForFirestore(email), FirestoreConstant.SUBSCRIPTION, FirestoreConstant.INACTIVE_EMAILS);
            _firestoreService.Post(CurrentPlantName, ActiveEmails, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);

            return Task.CompletedTask;
        }

        // remove subscribing section of current plant from Active collection
        private Task DeactivateSubscription()
        {
            _firestoreService.Remove(CurrentPlantName, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);

            return Task.CompletedTask;
        }

    }
}

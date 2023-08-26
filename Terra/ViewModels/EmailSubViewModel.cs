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
            ActiveEmails = (await _firestoreService.GetKeysAsCollection(FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS)).ToObservableCollection();
            FormatForEmail(ActiveEmails);
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
        /// Remove email from database, update view, and remove mail from firestore
        /// [Used: EmailSubPage -> remove button]
        /// </summary>
        /// <param name="email"> Mail to be removed. </param>
        [RelayCommand]
        public Task DeleteEmail(string email)
        {
            _emailListDBService.DeleteEmail(email);
            UpdateEmails();

            return TotalRemoveFromFirestore(email);
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
            if (ActiveEmails.Count > 0)
            {
                foreach (var email in ActiveEmails)
                {
                    await ActivateSubscription(email);
                }
                
            }
            // remove email from active document in firestore
            foreach (var email in Emails.Except(ActiveEmails))
            {
                await DeactivateSubscription(email);
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

        // strip off "@gmail.com" from firestore operations
        private string FormatForFirestore(string target)
        {
            return target[..target.IndexOf('@')];
        }

        // add "@gmail.com" to string
        private void FormatForEmail(ObservableCollection<string> usernames)
        {
            for (int i = 0; i < usernames.Count; i++) 
            {
                usernames[i] = $"{usernames[i]}@gmail.com";
            }
        }

        // remove email from both active and inactive documents in firestore
        private Task TotalRemoveFromFirestore(string email)
        {
            _firestoreService.Remove(FormatForFirestore(email), FirestoreConstant.SUBSCRIPTION, FirestoreConstant.INACTIVE_EMAILS);
            _firestoreService.Remove(FormatForFirestore(email), FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);

            return Task.CompletedTask;
        }

        // move emails from inactive document to active document
        private Task ActivateSubscription(string email)
        {
            _firestoreService.Remove(FormatForFirestore(email), FirestoreConstant.SUBSCRIPTION, FirestoreConstant.INACTIVE_EMAILS);
            _firestoreService.Post(FormatForFirestore(email), CurrentPlantName, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);

            return Task.CompletedTask;
        }

        // move emails from active document to inactive document
        private Task DeactivateSubscription(string email)
        {
            _firestoreService.Remove(FormatForFirestore(email), FirestoreConstant.SUBSCRIPTION, FirestoreConstant.ACTIVE_EMAILS);
            _firestoreService.Post(FormatForFirestore(email), "Terra", FirestoreConstant.SUBSCRIPTION, FirestoreConstant.INACTIVE_EMAILS);

            return Task.CompletedTask;
        }

    }
}

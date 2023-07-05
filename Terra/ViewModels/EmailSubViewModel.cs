using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public List<string> emailList;

        // UI attributes
        [ObservableProperty]
        public bool isMaxCapacity;
        [ObservableProperty]
        public string maxCapacityWarning;

        public EmailSubViewModel()
        {
            EmailList = new();
            EmailModel = new();
            _emailService = new();
            _emailListDBService = new();
        }

        public void UpdateEmails()
        {
            EmailList = _emailListDBService.GetFromEmailTable();
        }

        [RelayCommand]
        public Task NavigateToRemoval() => Shell.Current.GoToAsync(nameof(RemoveEmailPage));

        [RelayCommand]
        public void AddEmail()
        {
            IsMaxCapacity = _emailListDBService.IsMaxCapacity();

            if (IsMaxCapacity is false)
            {
                _emailListDBService.PostToEmailTable(EmailModel.Email);
                UpdateEmails();
                MaxCapacityWarning = string.Empty;
            }
            else
            {
                MaxCapacityWarning = "Can't add more emails. Max capacity reached.";
            }
        }

        [RelayCommand]
        public Task DeleteEmail()
        {
            _emailListDBService.DeleteEmail(EmailModel.Email);
            UpdateEmails();
            
            return Shell.Current.GoToAsync(nameof(EmailSubPage));
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Services;

namespace Terra.ViewModels
{
    public partial class EmailSubViewModel : ObservableObject
    {
        private EmailService _emailService;
        public ObservableCollection<string> EmailList { get; set; }

        public EmailSubViewModel()
        {
            EmailList = new();

            _emailService = new();
        }

        [RelayCommand]
        public void AddEmail()
        {
            EmailList.Add("lololol");
        }
    }
}

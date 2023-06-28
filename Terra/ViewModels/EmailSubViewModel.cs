using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Services;

namespace Terra.ViewModels
{
    public partial class EmailSubViewModel : ObservableObject
    {
        private EmailService _emailService;

        public EmailSubViewModel()
        {
            _emailService = new();
        }
    }
}

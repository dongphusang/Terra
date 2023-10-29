using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Views;

namespace Terra.Interfaces
{
    interface IPopupService
    {
        void ShowPopup(Popup popup);
        void ClosePopup(Popup popup);
    }
}

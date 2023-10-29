/* This class provides access to Popups in ViewModel
 * 
 * 
 */

using Terra.Interfaces;
using CommunityToolkit.Maui.Views;

namespace Terra.Services
{
    class PopupService :IPopupService
    {
        public void ShowPopup(Popup popup)
        {
            Page page = Application.Current?.MainPage ?? throw new NullReferenceException();
            page.ShowPopup(popup);
        }       

        public void ClosePopup(Popup popup)
        {
            popup.Close();
        }
    }
}

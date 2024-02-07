using Microsoft.Maui.Controls;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Terra.Services;
using Terra.TerraConstants;

namespace Terra.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{
        [ObservableProperty]
        public string nextWateringSchedule;

        private FirestoreService _firestoreService;

		public MainPageViewModel()
		{
            // used to retrieve next watering schedule
            _firestoreService = new ();

            UpdateEventsOnAppearing();
		}

        public void UpdateEventsOnAppearing()
        {
            NextWateringSchedule = Task.Run(() => _firestoreService.GetValue(FirestoreConstant.MASK_NEXT_WATER_SCHEDULE, FirestoreConstant.COLLECTION_SUBSCRIPTION, FirestoreConstant.DOC_ESP32_1)).Result;
        }

        [RelayCommand]
        async Task ToAddWorkspacePage() => await Shell.Current.GoToAsync(nameof(AddWorkspacePage), false);

        [RelayCommand]
        async Task ToWorkspaceList() => await Shell.Current.GoToAsync(nameof(WorkspaceList), false);

        [RelayCommand]
        async Task ToGraphicalView() => await Shell.Current.GoToAsync(nameof(GraphicalView), false);

        [RelayCommand]
        async Task ToEmailSubPage() => await Shell.Current.GoToAsync(nameof(EmailSubPage), false);

        

    }
}


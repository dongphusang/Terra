using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terra.Models;
using Terra.Services;
using Terra.TerraConstants;

namespace Terra.ViewModels
{
    partial class OperatingModeViewModel : ObservableObject
    {
        // declaring models
        [ObservableProperty]
        public Schedule scheduleModel;

        // declaring relevant services
        private WorkspaceService _workspaceService;     // get workspace associated with current plant
        private FirestoreService _firestoreService;     // manipulate email list on firestore

        // declaring privates
        private string _currentMCU;             // target microcontroller associating with current workspace
        private string _currentWorkspaceName;   // target workspace
        private string _currentPlantName;       // target plant within the workspace      

        // on selections changed 
        [ObservableProperty]
        public int wateringScheduleOpacity;
        [ObservableProperty]
        public bool wateringScheduleStatus;

        // declaring binding properties
        [ObservableProperty]
        public bool isLightingAuto;     // Lighting is in auto mode by default; System decides light settings
        [ObservableProperty]
        public bool isWateringAuto;
        [ObservableProperty]            // Watering is in auto mode by default; System decides when to water
        public ObservableCollection<string> schedules;
        [ObservableProperty]
        public string pickedSchedule; // a schedule picked from ObservableCollection
        [ObservableProperty]
        public List<string> days = new() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

        public OperatingModeViewModel()
        {
            ScheduleModel = new();
            _workspaceService = new WorkspaceService();
            _firestoreService = new FirestoreService();
            
            GetWorkspaceDetails().ConfigureAwait(false);
            GetAutoConfigAsync().ConfigureAwait(false);
        }
        
        /// <summary>
        /// Get plant name and microcontroller relevant to a workspace.
        /// </summary>
        /// <returns></returns>
        private Task GetWorkspaceDetails()
        {
            _currentWorkspaceName = Preferences.Get("CurrentWorkspace", string.Empty); // get value from preferences (which assigned in WorkspaceViewModel)
            _currentPlantName = (string) _workspaceService.GetPlantName(_currentWorkspaceName).Result;
            _currentMCU = (string)_workspaceService.GetWorkspaceMCU(_currentWorkspaceName).Result;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get config for automation mode.
        /// </summary>
        /// <returns></returns>
        private async Task GetAutoConfigAsync()
        {
            IsWateringAuto = Convert.ToBoolean(await _firestoreService.GetValue(_currentMCU, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.WATERMOD));
        }

        /// <summary>
        /// Save inputs to ObservableCollection and update schedules view.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public Task AddSchedule()
        {
            // format input time from 00:00:00 to 12:00 AM
            var formattedTime = DateTime.Today.Add(ScheduleModel.Time).ToString("hh:mm tt");
            // concat time and weekday into schedule
            var schedule = $"{ScheduleModel.WeekDay}, {formattedTime}";

            // add new schedule to list of schedules
            if (Schedules.Contains(schedule) is false && ScheduleModel.WeekDay is not null && IsWateringAuto is true)
            {
                Schedules.Add(schedule);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Remove inputs from ObservableCollection and update schedules view.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public Task RemoveSchedule()
        {
            if (Schedules.Contains(PickedSchedule) && IsWateringAuto is true)
            {
                Schedules.Remove(PickedSchedule);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get schedules from firestore and replace the local ObservableCollection schedules.
        /// </summary>
        /// <returns></returns>
        public async Task UpdateSchedules()
        {
            // pull schedules
            Schedules = new();
            var firestoreSchedule = await _firestoreService.GetValues(_currentMCU, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.SCHEDULE);

            foreach (var item in firestoreSchedule)
            {
                Schedules.Add((string)item);
            }
        }

        [RelayCommand]
        public Task CommitSchedules()
        {
            _firestoreService.PostOverride(_currentMCU, Schedules, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.SCHEDULE);
            _firestoreService.PostOverride(_currentMCU, IsWateringAuto, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.WATERMOD);
            Console.WriteLine("MADE: "+_currentMCU);
            Console.WriteLine("MADE: "+_currentPlantName);
            return Toast.Make("Changes Made!", ToastDuration.Short).Show();
        }        
    }
}

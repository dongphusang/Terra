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
        Schedule _schedule;

        // declaring relevant services
        private WorkspaceService _workspaceService;     // get workspace associated with current plant
        private FirestoreService _firestoreService;     // manipulate email list on firestore

        // declaring privates
        private string _currentMCU;             // target microcontroller associating with current workspace
        private string _currentWorkspaceName;   // target workspace
        private string _currentPlantName;       // target plant within the workspace

        // declaring binding properties
        [ObservableProperty]
        public bool isLightingAuto;     // Lighting is in auto mode by default; System decides light settings
        [ObservableProperty]
        public bool isWateringAuto;
        [ObservableProperty]            // Watering is in auto mode by default; System decides when to water
        public ObservableCollection<string> schedules;
        [ObservableProperty]
        public string pickedSchedule; // a schedule picked from ObservableCollection

        public OperatingModeViewModel()
        {
            _schedule = new();
            _workspaceService = new WorkspaceService();
            _firestoreService = new FirestoreService();
            
            GetWorkspaceDetails().ConfigureAwait(false);
            GetAutoConfigAsync().ConfigureAwait(false);
            Schedules = new();
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
            IsLightingAuto = Convert.ToBoolean(await _firestoreService.GetValue(_currentMCU, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.WATERMOD));
            IsWateringAuto = true;
        }

        /// <summary>
        /// Save inputs to ObservableCollection and update schedules view.
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public Task AddSchedule()
        {
            // add users' schedule 
            var schedule = $"{_schedule.WeekDay}, {_schedule.Time}";

            if (Schedules.Contains(schedule) is false && _schedule.WeekDay is not null)
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
            if (Schedules.Contains(PickedSchedule) && PickedSchedule is not null)
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
            Schedules = new();
            var firestoreSchedule = await _firestoreService.GetValues(_currentMCU, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.SCHEDULE);

            foreach (var item in firestoreSchedule)
            {
                Schedules.Add((string)item);
            }
        }

        [RelayCommand]
        public Task CommitSchedules() => _firestoreService.PostOverride(_currentMCU, Schedules, FirestoreConstant.SUBSCRIPTION, FirestoreConstant.SCHEDULE);
        

        





    }
}

﻿using Microsoft.Maui.Controls;
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Terra.ViewModels
{
	public partial class MainPageViewModel : ObservableObject
	{

		public MainPageViewModel()
		{
		}

        [RelayCommand]
        Task Navigate() => Shell.Current.GoToAsync("AddWorkspacePage");

        void OnViewClicked()
        {

        }

        void OnRemoveClicked()
        {

        }

        

    }
}


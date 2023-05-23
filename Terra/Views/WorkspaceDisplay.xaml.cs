using Terra.ViewModels;

namespace Terra;

public partial class WorkspaceDisplay : ContentPage
{
	private WorkspaceViewModel _viewModel; // viewmodel object
	private Task _backgroundThread; // background task to fetch data
	private CancellationTokenSource _backgroundTokenSource; // cancellation token source (cancelling task)


    public WorkspaceDisplay()
	{
		// initializing
		InitializeComponent();
		_viewModel = new WorkspaceViewModel();
        _backgroundTokenSource = new CancellationTokenSource();
        BindingContext = _viewModel;
		// Current Workspace
		_viewModel.GetCurrentWorkspace();
		// start background task
		_backgroundThread = Task.Run(() => FetchDataInflux(_backgroundTokenSource.Token));
	}

	/// <summary>
	/// Continuously fetch data in background
	/// </summary>
	/// <param name="cancellationToken"> if set to cancelled, terminate the loop (data fetching in background) </param>
	private void FetchDataInflux(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			_viewModel.GetDataFromInflux();
			Thread.Sleep(1000);
		}
	}

	/// <summary>
	/// Upon leaving the page, cancel the cancellationToken, terminate background data 
	/// fetching.
	/// </summary>
	/// <param name="args"></param>
    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
		_backgroundTokenSource.Cancel();
		//await _backgroundThread; causing bug and no need to wait for it to finish
    }
}
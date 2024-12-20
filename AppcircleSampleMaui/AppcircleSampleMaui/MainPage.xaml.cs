using AppcircleSampleMaui.Helpers;

namespace AppcircleSampleMaui;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await UpdateControl();
    }
    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
       
        if (count == 1)
            CounterBtn.Text = $"Clicked {count} time";
        else
            CounterBtn.Text = $"Clicked {count} times";

        SemanticScreenReader.Announce(CounterBtn.Text);
    }
    
    public async Task UpdateControl()
    {
        var currentVersion = AppInfo.VersionString;
        var updateInfo = await InAppUpdateHelper.CheckForUpdate(currentVersion, "USER_EMAIL");
		
        if (updateInfo?.DownloadUrl != null && await Launcher.CanOpenAsync(updateInfo.DownloadUrl))
        {
            bool result = await DisplayAlert("Update Available",$"{updateInfo.Version} version is available.", "Update","Cancel");
            if (result)
            {
                await Launcher.OpenAsync(updateInfo.DownloadUrl);
            }
        }
    }
}
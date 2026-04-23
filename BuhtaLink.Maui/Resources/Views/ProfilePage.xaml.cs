using BuhtaLink.ViewModels;

namespace BuhtaLink.Resources.Views;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Loaded += async (s, e) => await viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
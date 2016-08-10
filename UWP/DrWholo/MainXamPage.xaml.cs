using Adept;
using DrWholoLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace DrWholo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainXamPage : Page
    {
        private DrWholoService service;
        private MainViewModel vm;

        public MainXamPage()
        {
            // Initialize page
            this.InitializeComponent();

            // Get ViewModel
            vm = DataContext as MainViewModel;

            // Get services
            service = DrWholoService.Instance;

            // Subscribe to events
            this.Loaded += MainXamPage_Loaded;
            service.SignedIn += Service_SignedIn;
        }


        private async Task AuthenticateAsync()
        {
            // Authenticate
            try
            {
                service.AllowInteractiveSignIn = true;
                await service.SignInAync();
                // service.SignOut();
            }
            catch (Exception)
            {
                // Switch to Auth view (replacing current view)
                await AppViewManager.Views["Auth"].SwitchAsync();
            }
        }

        private async void MainXamPage_Loaded(object sender, RoutedEventArgs e)
        {
            await AuthenticateAsync();
        }

        private async void Service_SignedIn(object sender, EventArgs e)
        {
            await vm.LoadDataAsync();
        }
    }
}

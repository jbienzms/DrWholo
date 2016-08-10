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
using Windows.UI.Popups;
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
    public sealed partial class AuthPage : Page
    {
        private DrWholoService drWholoService;

        public AuthPage()
        {
            this.InitializeComponent();
            drWholoService = DrWholoService.Instance;
        }

        public async Task DoSignInAsync()
        {
            SignInButton.IsEnabled = false;
            try
            {
                drWholoService.AllowInteractiveSignIn = true;
                await drWholoService.SignInAync();

                // Switch back to Main view (replacing current view)
                await AppViewManager.Views["Main"].SwitchAsync();
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
                SignInButton.IsEnabled = true;
            }
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            await DoSignInAsync();
        }
    }
}

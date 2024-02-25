using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NetClient;

namespace AvaloniaNetworkClient
{
    public partial class LoginWindow : Window
    {
        public Client netClient = new Client();

        public LoginWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            RunLogin();
            
        }
        private async void RunLogin()
        {
            var address = this.FindControl<TextBox>("AddressInput").Text;
            var port = this.FindControl<TextBox>("PortInput").Text;
            var user = this.FindControl<TextBox>("UserInput").Text;
            var pass = this.FindControl<TextBox>("PasswordInput").Text;
            await netClient.Connect(address, port);
            await netClient.RequestAuthenticate(user, pass);
            netClient.onAuthenticate += StartMainWindow;
        }

        private void StartMainWindow(bool obj)
        {
            if (obj)
            {
                Application.Current.Run(new MainWindow(netClient));
            }
        }
    }
}
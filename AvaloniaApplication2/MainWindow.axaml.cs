using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;
using NetClient;
namespace NetworkManagerAvalonia
{
    public partial class MainWindow : Window
    {
        public Client netClient = new Client();

        public MainWindow()
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
        
        private async Task RequestUserList()
        {
            netClient.RequestUserList();
        }
        
        private async void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
        {
            await RequestUserList();
        }
        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            RunLogin();
            await RequestUserList();
        }
        private async void RunLogin()
        {
            var address = this.FindControl<TextBox>("AddressInput").Text;
            var port = this.FindControl<TextBox>("PortInput").Text;
            var user = this.FindControl<TextBox>("UserInput").Text;
            await netClient.Connect(address, port);
            await netClient.RequestAuthenticate(user, user);
        }
    }
}
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using NetClient;

namespace AvaloniaNetworkClient;

public partial class MainWindow : Window
{
    private Client netClient;
    public MainWindow(Client client)
    {
        netClient = client;
        RequestUserList();
        InitializeComponent();
    }
    private async Task RequestUserList()
    {
        netClient.RequestUserList();
    }
        
    
    private async void RefreshUsersButton_Click(object sender, RoutedEventArgs e)
    {
        await RequestUserList();
    }
}
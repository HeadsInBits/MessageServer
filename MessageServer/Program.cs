using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MessageServer.Models;

try
{
    var server = WebSocketServer.Instance;
    await server.Start();

    Console.ReadLine();
    await server.Stop();
}
catch (TypeInitializationException ex)
{
    Console.WriteLine($"Error initializing WebSocketServer: {ex.InnerException.Message}");
}
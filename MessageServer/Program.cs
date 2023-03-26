using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using MessageServer.Models;

var server = WebSocketServer.Instance;
await server.Start();

Console.ReadLine();
await server.Stop();
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class SimpleTcpServer
{
    static async Task Main(string[] args)
    {
        int port = 5000;
        var listener = new TcpListener(IPAddress.Any, port);

        listener.Start();
        Console.WriteLine($"Server started. Listening on port {port}...");

        while (true)
        {
            Console.WriteLine("Waiting for a client to connect...");
            TcpClient client = await listener.AcceptTcpClientAsync();
            Console.WriteLine("Client connected.");

            await HandleClientAsync(client);          }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        using (NetworkStream stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            try
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected.");
                    return;
                }

                string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received from client: {clientMessage}");

                string response = $"Server received your message: '{clientMessage}'";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                Console.WriteLine("Response sent to client.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }

        Console.WriteLine("Client connection closed.");
    }
}
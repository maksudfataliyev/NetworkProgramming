namespace ConsoleApp10;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        Console.Write("Введите свой порт (для приема входящих сообщений): ");
        int localPort = int.Parse(Console.ReadLine());

        Console.Write("Введите IP собеседника: ");
        string remoteIp = Console.ReadLine();

        Console.Write("Введите порт собеседника: ");
        int remotePort = int.Parse(Console.ReadLine());

        Thread serverThread = new Thread(() => StartServer(localPort));
        serverThread.IsBackground = true;
        serverThread.Start();

        StartClient(remoteIp, remotePort);
    }

    static void StartServer(int port)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"Сервер запущен на порту {port}. Ожидание сообщений...");

        while (true)
        {
            try
            {
                using (TcpClient client = listener.AcceptTcpClient())
                using (NetworkStream stream = client.GetStream())
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string message = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nСООБЩЕНИЕ: {message}");
                        Console.ResetColor();
                        Console.Write("> ");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка сервера: " + ex.Message);
            }
        }
    }

    static void StartClient(string ip, int port)
    {
        Console.WriteLine("Вы можете начать отправлять сообщения. Введите `exit` для выхода.");
        while (true)
        {
            Console.Write("> ");
            string message = Console.ReadLine();

            if (message.ToLower() == "exit")
                break;

            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(IPAddress.Parse(ip), port);
                    using (NetworkStream stream = client.GetStream())
                    using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                    {
                        writer.WriteLine(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка клиента: " + ex.Message);
            }
        }

        Console.WriteLine("Вы вышли из чата.");
        Environment.Exit(0);
    }
}

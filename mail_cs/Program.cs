using System;
using System.Net;
using System.Net.Mail;

class EmailMessage
{
    public string From { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}

class EmailSender
{
    public void Send(EmailMessage message)
    {
        try
        {
            Console.WriteLine("Чтение настроек SMTP из переменных окружения...");
            string smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST");
            string smtpPortStr = Environment.GetEnvironmentVariable("SMTP_PORT");
            string smtpUser = Environment.GetEnvironmentVariable("SMTP_USER");
            string smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS");
            string smtpSsl = Environment.GetEnvironmentVariable("SMTP_SSL");

            if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpPortStr) ||
                string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
            {
                Console.WriteLine("Ошибка: не заданы переменные окружения.");
                return;
            }

            int smtpPort = int.Parse(smtpPortStr);
            bool enableSsl = smtpSsl?.ToLower() == "true";

            Console.WriteLine("Подключение к SMTP-серверу...");

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = enableSsl
            };

            var mail = new MailMessage(message.From, message.To, message.Subject, message.Body);

            Console.WriteLine("Отправка сообщения...");
            client.Send(mail);

            Console.WriteLine("Сообщение успешно отправлено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке: {ex.Message}");
        }
    }
}

class Program
{
    static void Main()
    {
        var message = new EmailMessage();

        Console.Write("От кого (email): ");
        message.From = Console.ReadLine();

        Console.Write("Кому (email): ");
        message.To = Console.ReadLine();

        Console.Write("Тема: ");
        message.Subject = Console.ReadLine();

        Console.Write("Сообщение: ");
        message.Body = Console.ReadLine();

        var sender = new EmailSender();
        sender.Send(message);
    }
}

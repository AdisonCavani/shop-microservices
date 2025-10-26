using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace NotificationService.Services;

public class EmailService
{
    public async Task SendEmailAsync(
        string receiverName,
        string receiverEmail,
        string subject,
        string bodyHtml,
        CancellationToken cancellationToken = default)
    {
        MimeMessage message = new();
        message.From.Add(new MailboxAddress("Papercut", "mail@papercut.com"));
        message.To.Add(new MailboxAddress(receiverName, receiverEmail));
        message.Subject = subject;

        message.Body = new TextPart(TextFormat.Html)
        {
            Text = bodyHtml
        };

        var client = new SmtpClient();
        await client.ConnectAsync("localhost", 25, false, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
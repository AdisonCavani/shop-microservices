using Fluid;
using MailKit.Net.Smtp;
using Microsoft.Extensions.FileProviders;
using MimeKit;
using MimeKit.Text;

namespace NotificationService.Services;

public class EmailService
{
    private readonly FluidParser _fluidParser;
    private readonly IFileProvider _fileProvider;

    public EmailService(FluidParser fluidParser, IFileProvider fileProvider)
    {
        _fluidParser = fluidParser;
        _fileProvider = fileProvider;
    }

    public async Task<bool> SendEmailAsync(
        string receiverName,
        string receiverEmail,
        string subject,
        object model,
        CancellationToken cancellationToken = default)
    {
        try
        {
            MimeMessage message = new();
            message.From.Add(new MailboxAddress("Papercut", "mail@papercut.com"));
            message.To.Add(new MailboxAddress(receiverName, receiverEmail));
            message.Subject = subject;

            message.Body = new TextPart(TextFormat.Html)
            {
                Text = await TemplateFromFileAsync(model.GetType().Name, model, cancellationToken)
            };

            var client = new SmtpClient();
            await client.ConnectAsync("localhost", 25, false, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> TemplateFromFileAsync(
        string templateName,
        object model,
        CancellationToken cancellationToken = default)
    {
        var fileInfo = _fileProvider.GetFileInfo($"{templateName}.liquid");
        await using var stream = fileInfo.CreateReadStream();
        using var sr = new StreamReader(stream);
        var content = await sr.ReadToEndAsync(cancellationToken);

        if (!_fluidParser.TryParse(content, out var template, out var errors))
            throw new Exception(string.Join(Environment.NewLine, errors));

        // TODO: upgrade fluid package
        return await template.RenderAsync(new(model));
    }
}
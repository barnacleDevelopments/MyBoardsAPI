using System.Net;
using System.Net.Mail;

namespace MyBoardsAPI.Services;

// You will need to add a reference to this library:

public class MailService
{
    private readonly IConfiguration _config;

    public MailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendMail(string recipient, string subject, string message)
    {
        var client = new SmtpClient("smtp-mail.outlook.com");
        var sender = _config["MailSettings:SenderAddress"];
        var password = _config["MailSettings:SenderPassword"];

        client.Port = 587;
        client.DeliveryMethod = SmtpDeliveryMethod.Network;
        client.UseDefaultCredentials = false;
        var credentials = new NetworkCredential(sender, password);
        client.EnableSsl = true;
        client.Credentials = credentials;
        var mail = new MailMessage(sender.Trim(), recipient.Trim());
        mail.Subject = subject;
        mail.IsBodyHtml = true;
        mail.Body = message;
        await client.SendMailAsync(mail);
    }
}
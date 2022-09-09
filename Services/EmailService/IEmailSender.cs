namespace TeduMicroservices.IDP.Services.EmailService;

public interface IEmailSender
{
    void SendEmail(string recipient, string subject, string body, bool isBodyHtml = false, string sender = null);
}
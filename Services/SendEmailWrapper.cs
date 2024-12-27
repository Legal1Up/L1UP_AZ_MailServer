using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using Services.Models;
namespace Services;

public class SendEmailWrapper
{
    private readonly string _apiKey;

    public SendEmailWrapper(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public async Task<(bool success, string message)> SendTemplateEmail(EmailRequest request)
    {
        try 
        {
            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(request.fromEmail, request.fromName));
            msg.SetTemplateId(request.templateId);
            msg.SetTemplateData(request.templateData);
            msg.AddTo(new EmailAddress(request.toEmail, request.toName));
            msg.SetSubject(request.subject);
            
            var response = await client.SendEmailAsync(msg);
            
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted 
                && response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var body = await response.Body.ReadAsStringAsync();
                return (false, $"SendGrid Error: {response.StatusCode} - {body}");
            }

            return (true, "Email sent successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Exception: {ex.Message}");
        }
    }

    public async Task<(bool success, string message)> SendHTMLEmail(EmailRequest request)
    {
        try 
        {
            var client = new SendGridClient(_apiKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress(request.fromEmail, request.fromName));
            msg.SetSubject(request.subject);
            msg.AddContent(MimeType.Html, request.body);
            msg.AddTo(new EmailAddress(request.toEmail, request.toName));
            msg.SetReplyTo(new EmailAddress(request.replyToEmail, request.replyToName));

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted 
                && response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var body = await response.Body.ReadAsStringAsync();
                return (false, $"SendGrid Error: {response.StatusCode} - {body}");
            }

            return (true, "Email sent successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Exception: {ex.Message}");
        }
    }

}

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

    private void AddAttachments(SendGridMessage msg, List<AttachmentData>? attachments)
    {
        if (attachments == null || !attachments.Any()) return;

        foreach (var attachment in attachments)
        {
            var bytes = Convert.FromBase64String(attachment.base64Content);
            var content = Convert.ToBase64String(bytes);
            //Si pesa mas de 10mb, se debe enviar por ftp
            if (bytes.Length < 10 * 1024 * 1024)
              {
                msg.AddAttachment(
                    attachment.fileName,
                    content,
                    attachment.contentType ?? "application/octet-stream"
                );
            }
            else
            {
                //Excepcion de archivo pesado
                throw new Exception("Heavy file");
            }
        }
    }

    private void AddRecipients(SendGridMessage msg, EmailRequest request)
    {
        // Añadir destinatarios principales (To)
        var tos = request.to.Select(r => new EmailAddress(r.email, r.name)).ToList();
        msg.AddTos(tos);

        // Añadir CC si existen
        if (request.cc != null && request.cc.Any())
        {
            var ccs = request.cc.Select(r => new EmailAddress(r.email, r.name)).ToList();
            msg.AddCcs(ccs);
        }
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
            AddRecipients(msg, request);
            msg.SetSubject(request.subject);
            
            if (request.attachments != null && request.attachments.Any())
            {
                AddAttachments(msg, request.attachments);
            }

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
            AddRecipients(msg, request);
            msg.SetReplyTo(new EmailAddress(request.replyToEmail, request.replyToName));
            
            if (!string.IsNullOrEmpty(request.replyToEmail))
            {
                msg.SetReplyTo(new EmailAddress(request.replyToEmail, request.replyToName));
            }

            if (request.attachments != null && request.attachments.Any())
            {
                AddAttachments(msg, request.attachments);
            }

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

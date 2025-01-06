namespace Services.Models;

public class EmailRequest
{
    public required string fromEmail { get; set; }
    public required string fromName { get; set; }
    public required List<EmailRecipient> to { get; set; }
    public List<EmailRecipient>? cc { get; set; }
    public string? templateId { get; set; }
    public Dictionary<string, string>? templateData { get; set; }
    public string? subject { get; set; }
    public string? body { get; set; }
    public string? replyToEmail { get; set; }
    public string? replyToName { get; set; }
    public List<AttachmentData>? attachments { get; set; }
}

public class EmailRecipient
{
    public required string email { get; set; }
    public string? name { get; set; }
}

public class AttachmentData
{
    public required string fileName { get; set; }
    public required string base64Content { get; set; }
    public string? contentType { get; set; }
}
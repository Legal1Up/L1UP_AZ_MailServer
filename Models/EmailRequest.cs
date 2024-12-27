namespace Services.Models;

public class EmailRequest
{
    public required string fromEmail { get; set; }
    public required string fromName { get; set; }
    public required string toEmail { get; set; }
    public required string toName { get; set; }
    public string? templateId { get; set; }
    public Dictionary<string, string>? templateData { get; set; }
    public string? subject { get; set; }
    public string? body { get; set; }
    public string? replyToEmail { get; set; }
    public string? replyToName { get; set; }
    
}
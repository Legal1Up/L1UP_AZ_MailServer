using Microsoft.Azure.Functions.Worker.Extensions.Timer;
using Microsoft.Azure.Functions.Worker;
using Services.Models;
using Services;


class TimerTriggeredEmailSender
{
 private readonly SendEmailWrapper _emailWrapper;

    // Constructor that receives the dependency
    public TimerTriggeredEmailSender(SendEmailWrapper emailWrapper)
    {
        _emailWrapper = emailWrapper;
    }


    [Function("WeeklyEmailTrigger")]
    public async Task RunWeekly(
        [TimerTrigger("0 0 9 * * TUE")] // Runs every Tuesday at 9:00 AM
        Microsoft.Azure.Functions.Worker.TimerInfo timerInfo)
    {
        try
        {
            var emailRequest = new EmailRequest
            {
                fromEmail = "your-email@domain.com", // Configure according to your needs
                fromName = "Weekly Automated Email",
                to = new List<EmailRecipient> 
                {
                    new EmailRecipient { email = "recipient@domain.com", name = "Recipient Name" }
                },
                subject = "Weekly Automated Email",
                body = "<h1>Weekly Report</h1><p>This is your weekly automated email.</p>"
            };

            var (success, message) = await _emailWrapper.SendHTMLEmail(emailRequest);
            if (!success)
            {
                throw new Exception($"Failed to send weekly email: {message}");
            }
        }
        catch (Exception ex)
        {
            // Consider implementing proper logging here
            throw new Exception($"Weekly email trigger failed: {ex.Message}");
        }
    }

    [Function("MonthlyEmailTrigger")]
    public async Task RunMonthly(
        [TimerTrigger("0 0 9 2 * *")] // Runs on the 2nd day of every month at 9:00 AM
        Microsoft.Azure.Functions.Worker.TimerInfo timerInfo)
    {
        try
        {
            var emailRequest = new EmailRequest
            {
                fromEmail = "your-email@domain.com", // Configure according to your needs
                fromName = "Monthly Automated Email",
                to = new List<EmailRecipient> 
                {
                    new EmailRecipient { email = "recipient@domain.com", name = "Recipient Name" }
                },
                subject = "Monthly Automated Email",
                body = "<h1>Monthly Report</h1><p>This is your monthly automated email.</p>"
            };

            var (success, message) = await _emailWrapper.SendHTMLEmail(emailRequest);
            if (!success)
            {
                throw new Exception($"Failed to send monthly email: {message}");
            }
        }
        catch (Exception ex)
        {
            // Consider implementing proper logging here
            throw new Exception($"Monthly email trigger failed: {ex.Message}");
        }
    }
}

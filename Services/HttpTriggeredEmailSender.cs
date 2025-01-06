using System.Net;
using System.Text.Json;
using Services.Models;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Services;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using Microsoft.Azure.Functions.Worker.Extensions.Timer;


public class SendEmailFunction
{
    private readonly SendEmailWrapper _emailWrapper;

    // Constructor that receives the dependency
    public SendEmailFunction(SendEmailWrapper emailWrapper)
    {
        _emailWrapper = emailWrapper;
    }

    [Function("SendTemplateEmail")]
    [OpenApiOperation(operationId: "SendTemplateEmail", tags: new[] { "Email" })]
    [OpenApiRequestBody("application/json", typeof(EmailRequest))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", 
        bodyType: typeof(SendEmailResponse), Description = "The OK response")]
    public async Task<HttpResponseData> SendTemplateEmail(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var emailRequest = JsonSerializer.Deserialize<EmailRequest>(requestBody)
                ?? throw new ArgumentNullException("RequestEmail", "Request body cannot be null");

            //Revisar que tenga el templateId y el templateData
            if (string.IsNullOrEmpty(emailRequest.templateId) || emailRequest.templateData == null
                || emailRequest.to == null || !emailRequest.to.Any())
            {
                throw new ArgumentNullException("TemplateId, TemplateData or Recipients cannot be null");
            }

            var (success, message) = await _emailWrapper.SendTemplateEmail(emailRequest);
            var response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { success, message });
            return response;
        }
        catch (Exception ex)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { 
                success = false, 
                error = ex.Message,
            });
            return response;
        }
    }

    [Function("SendHTMLEmail")]
    [OpenApiOperation(operationId: "SendHTMLEmail", tags: new[] { "Email" })]
    [OpenApiRequestBody("application/json", typeof(EmailRequest))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", 
        bodyType: typeof(SendEmailResponse), Description = "The OK response")]
    public async Task<HttpResponseData> SendHTMLEmail(
        [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        try
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var emailRequest = JsonSerializer.Deserialize<EmailRequest>(requestBody)
                ?? throw new ArgumentNullException("RequestEmail", "Request body cannot be null");
            
            if (string.IsNullOrEmpty(emailRequest.subject) || 
                string.IsNullOrEmpty(emailRequest.body) ||
                emailRequest.to == null || !emailRequest.to.Any())
            {
                throw new ArgumentNullException("Subject, Body or Recipients cannot be null");
            }

            var (success, message) = await _emailWrapper.SendHTMLEmail(emailRequest);
            var response = req.CreateResponse(success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { success, message });
            return response;
        }
        catch (Exception ex)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { 
                success = false, 
                error = ex.Message,
            });
            return response;

        }
    }
    [Function("TestEmail")]
    [OpenApiOperation(operationId: "TestEmail", tags: new[] { "Email" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", 
        bodyType: typeof(SendEmailResponse), Description = "The OK response")]
    public async Task<HttpResponseData> TestEmail(
        [HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        try
        {
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { 
                success = true, 
                message = "Email service is running" 
            });
            return response;
        }
        catch (Exception ex)
        {
            var response = req.CreateResponse(HttpStatusCode.BadRequest);
            await response.WriteAsJsonAsync(new { 
                success = false, 
                error = ex.Message,
            });
            return response;
        }
    }



}

public class SendEmailResponse
{
    public bool Success { get; set; }
}
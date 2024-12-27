using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Services;

var apiKey = Environment.GetEnvironmentVariable("SendGridApiKey") 
    ?? throw new ArgumentNullException("SendGridApiKey");

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddSingleton(sp => new SendEmailWrapper(apiKey));
    })
    .ConfigureOpenApi()
    .Build();

host.Run();

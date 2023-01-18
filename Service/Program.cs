
const int HttpPort = 5000;
const int NetTcpPort = 8089;

var builder = WebApplication.CreateBuilder();

builder.WebHost
.UseKestrel(options =>
{
    options.ListenAnyIP(HttpPort);
})
.UseNetTcp(NetTcpPort);

//Enable CoreWCF Services, with metadata (WSDL) support
builder.Services.AddServiceModelServices()
    .AddServiceModelMetadata()
    .AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>()
    .AddSingleton<CalculatorService>();

var app = builder.Build();

app.UseServiceModel(builder =>
{
    // Add the Calculator Service
    builder.AddService<CalculatorService>(serviceOptions =>
    {
        // Set the default host name:port in generated WSDL and the base path for the address 
        serviceOptions.BaseAddresses.Add(new Uri($"http://localhost:{HttpPort}/CalculatorService/netTcp"));
    })
    // Add NetTcpBinding endpoint
    .AddServiceEndpoint<CalculatorService, ICalculatorService>(new NetTcpBinding(), $"net.tcp://localhost:{NetTcpPort}/CalculatorService/netTcp");

    // Configure WSDL to be available
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.Run();

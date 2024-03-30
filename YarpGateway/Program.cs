using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Yarp.ReverseProxy.Configuration;
using YarpGateway.Helpers;
using YarpGateway.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var yarpConfigs = new YarpConfigs
{
    Routes = new Dictionary<string, YarpGateway.Helpers.Route>
    {
        ["movement-route"] = new YarpGateway.Helpers.Route
        {
            ClusterId = "movement-cluster",
            Match = new YarpGateway.Helpers.Match
            {
                Path = "/movements/{**catch-all}"
            },
            Transforms = new List<TransformsItem>
            {
                new TransformsItem
                {
                    PathPattern = "{**catch-all}"
                }
            },
            RateLimiterPolicy = "fixed"
            // AuthorizationPolicy = "authenticated"
        }
    },
    Clusters = new Dictionary<string, Cluster>
    {
        ["movement-cluster"] = new Cluster
        {
            Destinations = new Destinations
            {
                destination1 = new Destination1
                {
                    Address = "http://localhost:5247/api/v1/Movement"
                }
            }
        }
    }
};



// Add Yarp
//builder.Services.AddReverseProxy()
//    .LoadFromConfig(builder.Configuration.GetSection("YarpConfigs"));
builder.Services.AddSingleton<IProxyConfigProvider, CustomProxyConfigProvider>()
            .AddReverseProxy();




// Add Yarp Rate Limiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(5);
        options.PermitLimit = 2;
    });
});

// Add Authentication to Yarp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Add Authorization to Yarp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("authenticated", policy =>
        policy.RequireAuthenticatedUser());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIpChecker();

app.UseApiKeyChecker();

app.UseAuthentication();

app.UseAuthorization();

app.UseRateLimiter();

app.MapReverseProxy();

app.MapControllers();

app.Run();

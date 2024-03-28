using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Yarp
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("Yarp"));

// Add Yarp Rate Limiting
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromSeconds(5);
        options.PermitLimit = 2;
    });
});


/*// Add JWT authentication services
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Set to true if you want to validate the JWT issuer
            ValidateAudience = true, // Set to true if you want to validate the JWT audience
            ValidateLifetime = true, // Set to true if you want to validate the token expiration
            ValidateIssuerSigningKey = true,
            ValidIssuer = "yourIssuer", // Replace with the issuer of your JWT tokens
            ValidAudience = "yourAudience", // Replace with the audience of your JWT tokens
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey")) // Replace with your JWT secret key
        };
    });


// Add Authorization to Yarp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("secure", policy =>
    policy.RequireAuthenticatedUser());
});*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.MapControllers();

app.UseRateLimiter();

app.Run();

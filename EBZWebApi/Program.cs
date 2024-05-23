using EBZWebApi;
using EBZWebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IJWTAuthService,JWTAuthService>();
builder.Services.AddSingleton<IActiveUsersService,ActiveUsersService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(settings =>
{
    settings.AddDefaultPolicy
    (
        builder => builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
        .AllowCredentials()
        .AllowAnyHeader()
        .AllowAnyMethod()
    );
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration.GetValue<string>("JWTAuth:Issuer"),
        ValidAudience = builder.Configuration.GetValue<string>("JWTAuth:Audience"),
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWTAuth:Key")!)),
        ValidateIssuerSigningKey = true
    };
});

var app = builder.Build();

app.UseCors();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

Initialization.Initialize();

app.Run();



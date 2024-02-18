using cmangos_web_api.Configs;
using cmangos_web_api.Helpers;
using cmangos_web_api.Repositories;
using Services.Services;
using Data.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Services.Repositories;
using System.Reflection;
using System.Security.Claims;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var info = new OpenApiInfo
    {
        Version = "v1.0",
        Contact = new OpenApiContact()
        {
            Name = "cMaNGOS",
            Url = new Uri("https://cmangos.net"),
            Email = ""
        },
        Title = "cMaNGOS CMS API v1.0",
        TermsOfService = new Uri("https://example.com/terms")
    };
    options.SwaggerDoc("v1", info);
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Common.xml"));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Configs.xml"));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Data.xml"));
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Services.xml"));
    options.IncludeXmlCommentsFromInheritDocs();
    options.AddEnumsWithValuesFixFilters();
    options.EnableAnnotations();
});

// Allow cors any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("AuthConfig"));
builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("EmailConfig"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User!);
builder.Services.AddDbContext<RealmdDbContext>();
var connectionStringCms = builder.Configuration.GetValue<string>("ConnectionStrings:Cms");
builder.Services.AddDbContext<CmsDbContext>(options => options.UseMySql(connectionStringCms, ServerVersion.AutoDetect(connectionStringCms))
                .LogTo(Console.WriteLine, LogLevel.Debug)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope()!)
{
    serviceScope.ServiceProvider.GetRequiredService<CmsDbContext>().Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.UseCors("CorsPolicy");

app.Run();

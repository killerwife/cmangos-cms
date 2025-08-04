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
using cmangos_web_api.Auth;
using Configs;
using Services.Repositories.World;
using DBFileReaderLib;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using Services.Repositories.Cms;

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
builder.Services.Configure<WebsiteConfig>(builder.Configuration.GetSection("WebsiteConfig"));
builder.Services.Configure<DbcConfig>(builder.Configuration.GetSection("DbcConfig"));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ClaimsPrincipal>(s => s.GetService<IHttpContextAccessor>()?.HttpContext?.User!);
builder.Services.AddDbContext<RealmdDbContext>();
builder.Services.AddDbContext<WorldDbContext>();
var connectionStringCms = builder.Configuration.GetValue<string>("ConnectionStrings:Cms");
builder.Services.AddDbContext<CmsDbContext>(options => options.UseMySql(connectionStringCms, ServerVersion.AutoDetect(connectionStringCms), options => options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: System.TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserProvider, UserProvider>();
builder.Services.AddScoped<IWorldRepository, WorldRepository>();
builder.Services.AddScoped<IEntityExt, EntityExt>();
builder.Services.AddSingleton<IWorldMapRepository, WorldMapRepository>();
builder.Services.AddSingleton<DBCRepository>();

if (builder.Configuration.GetValue<string>("AllowForwarding") == "true")
    builder.Services.Configure<ForwardedHeadersOptions>(options =>
    {
        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
        options.RequireHeaderSymmetry = false;
        options.ForwardLimit = null;
        if (builder.Configuration.GetValue<string>("IgnoreProxyCheck") == "true")
        {
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        }
        else
        {
            options.KnownProxies.Add(IPAddress.Parse(builder.Configuration.GetValue<string>("KnownProxy")));
        }
    });

var app = builder.Build();

using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope()!)
{
    serviceScope.ServiceProvider.GetRequiredService<CmsDbContext>().Database.Migrate();
}

if (builder.Configuration.GetValue<bool>("AllowForwarding") == true)
    app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("AllowSwagger") == true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors("CorsPolicy");

var dbcRepo = app.Services.GetRequiredService<DBCRepository>();
dbcRepo.Load();

app.Run();

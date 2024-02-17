using cmangos_web_api.Helpers;
using cmangos_web_api.Repositories;
using cmangos_web_api.Services;
using Microsoft.EntityFrameworkCore;
using Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow cors any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
    );
});

builder.Services.AddDbContext<RealmdDbContext>();
var connectionStringCms = builder.Configuration.GetValue<string>("ConnectionStrings:Cms");
builder.Services.AddDbContext<CmsDbContext>(options => options.UseMySql(connectionStringCms, ServerVersion.AutoDetect(connectionStringCms))
                .LogTo(Console.WriteLine, LogLevel.Debug)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors());
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

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

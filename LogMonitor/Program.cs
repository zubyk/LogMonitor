using LogMonitor;
using Models = LogMonitor.Models;
using LogMonitor.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.DirectoryServices.AccountManagement;
using System.Globalization;
using Microsoft.AspNetCore.Authentication.Negotiate;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext(), true, false);

builder.Services.Configure<ApplicationConfigOptions>(builder.Configuration.GetSection(ApplicationConfigOptions.SectionName));

builder.Services.AddSingleton<ProductInfoService>();
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<MailService>();

#pragma warning disable CA1416 // Validate platform compatibility
builder.Services.AddScoped((provider) =>
{
    try
    {
        return new PrincipalContext(ContextType.Domain);
    }
    catch when (builder.Environment.IsDevelopment())
    {
        return new PrincipalContext(ContextType.Machine);
    }
});
#pragma warning restore CA1416 // Validate platform compatibility

builder.Services.AddDbContext<LogMonitor.Data.DataContext, LogMonitor.Data.SqlServerDataContext>(options => 
{
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(true);
        options.EnableDetailedErrors(true);
    }

    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddTransient<IClaimsTransformation, ADClaimsTransformation>();

//builder.Services.AddSignalR().AddHubOptions<ReportHub>(options =>
//{
//    if (builder.Environment.IsDevelopment())
//    {
//        options.EnableDetailedErrors = true;
//    }
//});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<OperationCancelledExceptionFilter>();
}).AddRazorRuntimeCompilation();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SendReportPolicy", policy => policy.RequireClaim(LogMonitorClaimTypes.UserRole, Models.UserRoleEnum.Administrator.ToRoleName(), Models.UserRoleEnum.Supervisor.ToRoleName(), Models.UserRoleEnum.Operator.ToRoleName()));
    options.AddPolicy(Models.UserRoleEnum.Administrator.ToPolicyName(), policy => policy.RequireClaim(LogMonitorClaimTypes.UserRole, Models.UserRoleEnum.Administrator.ToRoleName()));
    options.AddPolicy(Models.UserRoleEnum.Operator.ToPolicyName(), policy => policy.RequireClaim(LogMonitorClaimTypes.UserRole, Models.UserRoleEnum.Operator.ToRoleName()));
    options.AddPolicy(Models.UserRoleEnum.Supervisor.ToPolicyName(), policy => policy.RequireClaim(LogMonitorClaimTypes.UserRole, Models.UserRoleEnum.Supervisor.ToRoleName()));
});

var app = builder.Build();

app.UseSerilogRequestLogging();

if (!builder.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ru-RU"),
    SupportedCultures = new[] { new CultureInfo("ru-RU") },
    SupportedUICultures = new[] { new CultureInfo("ru-RU") }
});

app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

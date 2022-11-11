using Blazored.Modal;
using Blazored.SessionStorage;
using Blazored.Toast;
using LV_QLKS.Data;
using LV_QLKS.Hubs;
using LV_QLKS.Service;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Radzen;
using System.Security.Claims;
using Tewr.Blazor.FileReader;
var builder = WebApplication.CreateBuilder(args);

//Add GG 
Microsoft.Extensions.Configuration.ConfigurationManager configuration = builder.Configuration;
builder.Services.AddAuthentication("Cookies")
    .AddCookie(opt =>
    {
        opt.Cookie.Name = "TryingoutGooogleOAuth";
        opt.LoginPath = "/auth/google-login";
    })
    .AddGoogle(opt =>
    {

        opt.ClientId = configuration["Authentication:Google:ClientId"];
        opt.ClientSecret = configuration["Authentication:Google:ClientSecret"];
        opt.Scope.Add("profile");
        opt.Events.OnCreatingTicket = context =>
        {
            string picurl = context.User.GetProperty("picture").GetString();
            context.Identity.AddClaim(new Claim("picture", picurl));
            return Task.CompletedTask;
        };

    });
// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers().AddNewtonsoftJson();
//SignalR
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(option =>
{
    option.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat
    (
        new[] { "application/octet-stream" }
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "LV_QLKS",
        builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

//Add Radzen
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ContextMenuService>();

//Add Blazor Toast
builder.Services.AddBlazoredToast();

//Add Blazor Session
builder.Services.AddBlazoredSessionStorage();

//Add Http to upload Image
builder.Services.AddScoped<HttpClient>();
builder.Services.AddFileReaderService();
builder.Services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);


builder.Services.AddHttpContextAccessor();

//builder.Services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
//SignalR
app.UseResponseCompression();
app.UseHttpsRedirection();

//GG
app.UseAuthentication();
app.UseAuthorization();
//GG
app.UseStaticFiles();

app.UseRouting();

//hub
app.MapHub<HotelBrHub>("/HotelBrHub");
//hub

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
});
app.UseMvcWithDefaultRoute();

app.Run();

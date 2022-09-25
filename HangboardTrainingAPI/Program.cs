using System.Text;
using HangboardTrainingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using MyBoardsAPI;
using MyBoardsAPI.Data;
using MyBoardsAPI.Models.Auth;
using MyBoardsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Register DB Context
var myBoardsConnectionString = builder.Configuration.GetConnectionString("POSTGRES_MYBOARDS");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<MyBoardsDbContext>(opt =>
        opt.UseNpgsql(myBoardsConnectionString));

// Adding Authentication
// TODO: Setup separate DB context for a user database
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MyBoardsDbContext>()
    .AddDefaultTokenProviders();

// TODO: Duel Authentication Schemes (https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "JWT_OR_COOKIE";
        options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddCookie("Cookies", options =>
    {
        options.Cookie.Name = ".AspNetCore.Identity.Application";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/login";
    })
    .AddJwtBearer("Bearer", options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
        };
    }).AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
    {
        // runs on each request
        options.ForwardDefaultSelector = context =>
        {
            // filter by auth type
            string authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                return "Bearer";

            // otherwise always check for cookie auth
            return "Cookies";
        };
    });
;

// Add Controllers
builder.Services.AddControllers();

// Configure Serverside Blazor
builder.Services.AddServerSideBlazor();

// Configure MVC Views
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

builder.Services.AddHostedService(sp => new NpmWatchHostedService(
    sp.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
    sp.GetRequiredService<ILogger<NpmWatchHostedService>>()));

// Add Services
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<MailService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
    endpoints.MapBlazorHub();
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // enable for production

app.MapControllers();

app.Run();
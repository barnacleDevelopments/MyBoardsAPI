using System.Text;
using MyBoardsAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBoardsAPI;
using MyBoardsAPI.Data;
using MyBoardsAPI.Models.Auth;
using MyBboardsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Register DB Context
var myBoardsConnectionString = builder.Configuration.GetConnectionString("POSTGRES_MYBOARDS");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<MyBoardsDbContext>(opt =>
        opt.UseNpgsql(myBoardsConnectionString));

// Adding Authentication
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MyBoardsDbContext>()
    .AddDefaultTokenProviders();

// TODO: Duel Authentication Schemes (https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET)
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
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
    });

// Add Controllers
builder.Services.AddControllers();

// Configure MVC Views
builder.Services.AddControllersWithViews();

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
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // enable for production

app.MapControllers();

app.Run();
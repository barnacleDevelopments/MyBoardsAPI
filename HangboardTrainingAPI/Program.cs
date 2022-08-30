using HangboardTrainingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyBoardsAPI.Data;
using MyBoardsAPI.Models.Auth;
using System.Text;
using MyBoardsAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Register DB Context
string myBoardsConnectionString = builder.Configuration.GetConnectionString("POSTGRES_MYBOARDS");

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<MyBoardsDbContext>(opt =>
    opt.UseNpgsql(myBoardsConnectionString));

// TODO: Do I need access to the Http Context?
builder.Services.AddHttpContextAccessor();

// For Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<MyBoardsDbContext>()
    .AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
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

// Configure Serverside Blazor
builder.Services.AddServerSideBlazor();

// Add Controllers
builder.Services.AddControllers();

// Configure MVC Views
builder.Services.AddControllersWithViews();

// Add Services
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<MailService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapBlazorHub();
});

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); // enable for production

app.MapControllers();

app.Run();

using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Build.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Project_Ecommerce.Data_Access.Data;
using Project_Ecommerce.Data_Access.Repository;
using Project_Ecommerce.Data_Access.Repository.IRepository;
using Project_Ecommerce.Utility;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("conStr");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

builder.Services.AddIdentity<IdentityUser,IdentityRole>().
    AddDefaultTokenProviders().AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

builder.Services.Configure<StripeSetting>(builder.Configuration.GetSection("StripeSettings"));
builder.Services.Configure<TwilioSettings>(builder.Configuration.GetSection("Twilio"));

//Fetching Models Through Repositories;

//builder.Services.AddScoped<ICategoryRepository, CateogryRepository>();
//builder.Services.AddScoped<ICoverTypeRepository, CoverTypeRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.LoginPath = $"/Identity/Account/Login";
    Options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    Options.LogoutPath = $"/Identity/Account/Logout";
});
builder.Services.Configure<FacebookOptions>(builder.Configuration.GetSection("Authentication:Facebook"));
builder.Services.Configure<GoogleOptions>(builder.Configuration.GetSection("Authentication:Google"));
builder.Services.Configure<TwitterOptions>(builder.Configuration.GetSection("Authentication:Twitter"));

builder.Services.AddAuthentication().AddFacebook(Options =>
{
    var facebookSettings = builder.Configuration.GetSection("Authentication:Facebook").Get<FacebookOptions>();
    Options.AppId = facebookSettings.AppId;
    Options.AppSecret = facebookSettings.AppSecret;
});


builder.Services.AddAuthentication().AddGoogle(Options =>
{
    var googleSettings = builder.Configuration.GetSection("Authentication:Google").Get<GoogleOptions>();
    Options.ClientId = googleSettings.ClientId;
    Options.ClientSecret = googleSettings.ClientSecret;

});


builder.Services.AddAuthentication().AddTwitter(Options =>
{
    var twitterSettings = builder.Configuration.GetSection("Authentication:Twitter").Get<TwitterOptions>();
    Options.ConsumerKey = twitterSettings.ConsumerKey;
    Options.ConsumerSecret = twitterSettings.ConsumerSecret;
    Options.RetrieveUserDetails = twitterSettings.RetrieveUserDetails;
});



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


//Middle Layer
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("StripeSettings")["Secretkey"];

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

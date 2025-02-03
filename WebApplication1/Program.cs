using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Stripe;
using WebApplication1.Data;
using WebApplication1.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register DataDbContext
builder.Services.AddDbContext<DataDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Register the cleanup service
builder.Services.AddHostedService<CartCleanupService>();

// Register MacAddressHelper as a singleton
builder.Services.AddSingleton<MacAddressHelper>();

builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));
StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];
builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Configure endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();

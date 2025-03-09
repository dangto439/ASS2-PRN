using Microsoft.EntityFrameworkCore;
using NewsManagementSystem.DataAccess.Models;
using Microsoft.AspNetCore.Builder;
using NewsManagementSystem.Pages.Hubs;
using NewsManagementSystem.BusinessLogic.Services;
using NewsManagementSystem.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<FunewsManagementContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStrings:DefaultConnection")));
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<NewsService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.MapHub<NewsHub>("/newsHub");

app.MapRazorPages();
app.MapGet("/", () => Results.Redirect("/Authencation/Index"));

app.Run();

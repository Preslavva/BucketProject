using BucketProject.DAL.Data.InterfacesRepo;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.Infrastructure.AI;

using BucketProject.DAL.Data.Repositories;
using BucketProject.BLL.Business_Logic.Mapping;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using Microsoft.Extensions.Configuration;
using System.Runtime;




var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGoalRepo,GoalRepo>();

builder.Services.AddScoped<IUserRepo,UserRepo>();

builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<GoalService>(); 


builder.Services.AddScoped<IAIClient,AIClient>();
builder.Services.AddHttpClient();




builder.Services.AddAutoMapper(typeof(AutoMapperProfile));



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=LogIn}/{id?}");

app.Run();

using BucketProject.Data.InterfacesRepo;
using BucketProject.Business_Logic.Services;
using BucketProject.Business_Logic.InterfacesService;
using BucketProject.Data.Repositories;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGoalRepo,GoalRepo>();
builder.Services.AddScoped<VbRepo>();
builder.Services.AddScoped<IUserRepo,UserRepo>();

builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IUserService,UserService>();


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

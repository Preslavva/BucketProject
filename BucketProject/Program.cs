using BucketProject.BLL.Business_Logic.InterfacesRepo;
using BucketProject.BLL.Business_Logic.Services;
using BucketProject.BLL.Business_Logic.InterfacesService;
using BucketProject.UI.BucketProject.Mapping;
using BucketProject.DAL.Data.Repositories;
using BucketProject.BLL.Business_Logic.Mapping;
using BucketProjetc.BLL.Business_Logic.InterfacesService;
using BucketProject.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IGoalRepo,GoalRepo>();

builder.Services.AddScoped<IUserRepo,UserRepo>();
builder.Services.AddScoped<ISocialRepo, SocialRepo>();


builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<ISocialService, SocialService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<IManagerRepo, ManagerRepo>();


builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<INotificationService,NotificationService>();
builder.Services.AddScoped<GoalService>();

builder.Services.AddScoped<IUserRepo, UserRepo>();

builder.Services.AddScoped<IAIClient,AIClient>();
builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<AutoMapperBL>();
    cfg.AddProfile<AutoMapperUI>();
});
builder.Services.AddScoped<IGoalInviteRepo, GoalInviteRepo>();

builder.Services.AddScoped<GlobalExceptionFilter>();

builder.Services
    .AddControllersWithViews(options =>
    { 
        options.Filters.AddService<GlobalExceptionFilter>();
    });



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


